using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SpendManagementHelpers.Helpers;
using AjaxControlToolkit;

namespace SpendManagementHelpers
{
    /// <summary>
    /// A control for creating a jsTree within an aspx page (www.jstree.com)
    ///  - with an associated drop area of a specified kind
    /// </summary>
    public class TreeCombo : WebControl, INamingContainer
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public TreeCombo() : base(HtmlTextWriterTag.Div)
        {
            this.ClientIDMode = ClientIDMode.AutoID;
        }

        #endregion Constructors



        #region Fields

        Tree _tree;
        ITreeDrop _drop;

        ControlsToRender _layout = ControlsToRender.TreeAndColumns;
        bool _showMenu = false;
        Unit _leftWidth = Unit.Pixel(522);
        Unit _leftPanelWidth = Unit.Pixel(522);
        Unit _rightPanelWidth = Unit.Pixel(300);
        string _leftTitle = string.Empty;
        string _rightTitle = string.Empty;
        string _titleCssClass = "sectiontitle";
        const string _dataFieldCssClass = "inputs";
        const string _fieldContainerCssClass = "twocolumn";
        const string _mandatoryCssClass = "mandatory";
        const string _iconCssClass = "inputicon";
		const string _tooltipCssClass = "tooltipicon";
        const string _validatorCssClass = "inputvalidatorfield";
        bool _allowDuplicatesInDrop = false;
        bool _filterPanelRendered = false;

        string _staticLibraryPath = "/static/";  // poor on the decoupling aspect, images could be part of the control
        string _themesPath = "../themes/";
        string _webServicePath = "~/PathTo/svcServiceName.asmx";
        string _getInitialTreeNodesWebServiceMethodName = "GetInitialTreeNodes";
        string _getBranchNodesWebServiceMethodName = "GetBranchNodes";
        string _getSelectedNodesWebServiceMethodName = "GetSelectedNodes";

        Image _moveNodeToDropIcon = new Image();
        Image _removeAllNodesIcon = new Image();
        Image _removeNodeIcon = new Image();
        Image _moveNodeUpIcon = new Image();
        Image _moveNodeDownIcon = new Image();

        Panel _filterPanel;
        Image _filterModalHelpIcon;
        Label _filterLabel;
        DropDownList _filterCriteria;
        HtmlGenericControl _span;
        HtmlGenericControl _div;
    	bool _renderFilterModal = true;

        string _filterValidationGroup = string.Empty;
        #endregion Fields



        #region Properties

        /// <summary>
        /// The client id of the contained tree control
        /// </summary>
        public string TreeClientID
        {
            get { return _tree.ClientID; }
        }

        /// <summary>
        /// The client id of the contained tree drop control
        /// </summary>
        public string TreeDropClientID
        {
            get { return _drop.ClientID; }
        }

        /// <summary>
        /// Determines which controls to include in this composite
        /// </summary>
        public ControlsToRender ComboType
        {
            get { return _layout; }
            set { _layout = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return _showMenu; }
            set { _showMenu = value; }
        }

        /// <summary>
        /// Allow the drop area to contain duplicate nodes from the same tree node
        /// </summary>
        public bool AllowDuplicatesInDrop
        {
            get { return _allowDuplicatesInDrop; }
            set { _allowDuplicatesInDrop = value; }
        }

        /// <summary>
        /// The width of the left panel, defaults to 50%, can be a pixel width
        /// </summary>
        public Unit LeftPanelWidth
        {
            get { return _leftWidth; }
            set { _leftWidth = value; }
        }

        /// <summary>
        /// The title to display on the left section
        /// </summary>
        public string LeftTitle
        {
            get { return _leftTitle; }
            set { _leftTitle = value; }
        }

        /// <summary>
        /// The title to display on the right section
        /// </summary>
        public string RightTitle
        {
            get { return _rightTitle; }
            set { _rightTitle = value; }
        }

        /// <summary>
        /// The title css class
        /// </summary>
        public string TitleCssClass
        {
            get { return _titleCssClass; }
            set { _titleCssClass = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return _staticLibraryPath; }
            set { _staticLibraryPath = value; }
        }

        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return _themesPath; }
            set { _themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return _webServicePath; }
            set { _webServicePath = ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial collection of jsTreeData
        /// </summary>
        public string WebServiceInitialTreeNodesMethod
        {
            get { return _getInitialTreeNodesWebServiceMethodName; }
            set { _getInitialTreeNodesWebServiceMethodName = value; }
        }
        /// <summary>
        /// The name of the method that will return the branch nodes when one is "opened"
        /// </summary>
        public string WebServiceBranchNodesMethod
        {
            get { return _getBranchNodesWebServiceMethodName; }
            set { _getBranchNodesWebServiceMethodName = value; }
        }
        /// <summary>
        /// The name of the method that will return the initial select nodes for a drop area
        /// </summary>
        public string WebServiceSelectedNodesMethod
        {
            get { return _getSelectedNodesWebServiceMethodName; }
            set { _getSelectedNodesWebServiceMethodName = value; }
        }

        /// <summary>
        /// Set the validation group for the
        /// </summary>
        public string FilterValidationGroup
        {
            get { return _filterValidationGroup; }
            set { _filterValidationGroup = value; }
        }

		/// <summary>
		/// Set this to FALSE to stop a filter modal being rendered with this control. TRUE by default.
		/// </summary>
    	public bool RenderFilterModal
    	{
    		get { return _renderFilterModal; }
			set { _renderFilterModal = value; }
    	}

        #endregion Properties



        #region Enums

        /// <summary>
        /// Control collection types
        /// </summary>
        public enum ControlsToRender
        {
            /// <summary>
            /// The tree and columns - default style.
            /// </summary>
            TreeAndColumns = 0,

            /// <summary>
            /// The tree and report columns - only used for reports.
            /// </summary>
            TreeAndReportColumns = 1,

            /// <summary>
            /// The tree and filters - Used to filter "greenlights".
            /// </summary>
            TreeAndFilters = 2,

            /// <summary>
            /// The tree and look up display fields - Used for Lookup Display Fields Only.
            /// </summary>
            TreeAndLookUpDisplayFields = 3
        }

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void CreateChildControls()
        {
            #region calculate widths
            Unit totalPanelWidth = new Unit(832, UnitType.Pixel);
            if (base.Width.Type == UnitType.Pixel && base.Width.IsEmpty == false)
            {
                totalPanelWidth = base.Width;
            }

            _leftPanelWidth = new Unit(522, UnitType.Pixel);
            if (_leftWidth.Type == UnitType.Pixel && _leftWidth.IsEmpty == false && _leftWidth.Value < ((int)totalPanelWidth.Value - 10))
            {
                _leftPanelWidth = _leftWidth;
            }

            _rightPanelWidth = new Unit((totalPanelWidth.Value - 10 - _leftPanelWidth.Value), UnitType.Pixel);
            #endregion calculate widths

            #region calculate heights
            Unit totalHeight = (base.Height.Type == UnitType.Pixel && base.Height.IsEmpty == false) ? base.Height : Unit.Pixel(300);
            Unit panelHeight = (_showMenu == true) ? Unit.Pixel((int)totalHeight.Value - 30) : totalHeight;
            #endregion calculate heights

            #region section spans
            HtmlGenericControl leftSection = new HtmlGenericControl("span");
            leftSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "inline-block");
            leftSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.VerticalAlign, "top");
            leftSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, _leftPanelWidth.ToString());
            leftSection.Attributes.CssStyle.Add("-moz-inline-block", "");

            HtmlGenericControl rightSection = new HtmlGenericControl("span");
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "inline-block");
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.VerticalAlign, "top");
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, _rightPanelWidth.ToString());
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, Unit.Pixel(10).ToString());
            rightSection.Attributes.CssStyle.Add("-moz-inline-block", "");
            #endregion section spans

            #region panel titles
            Panel leftTitle = new Panel();
            Panel rightTitle = new Panel();

            Literal leftTitleText = new Literal();
            Literal rightTitleText = new Literal();

            leftTitleText.Text = _leftTitle;
            rightTitleText.Text = _rightTitle;

            leftTitle.Controls.Add(leftTitleText);
            rightTitle.Controls.Add(rightTitleText);

            if (_titleCssClass != string.Empty)
            {
                leftTitle.CssClass = _titleCssClass;
                rightTitle.CssClass = _titleCssClass;
            }

            leftSection.Controls.Add(leftTitle);
            rightSection.Controls.Add(rightTitle);
            #endregion panel titles

            #region Trees

            #region tree control

            _tree = new Tree
                        {
                            ID = "tcTree",
                            Height = panelHeight,
                            ShowButtonMenu = _showMenu,
                            CssClass = "treediv",
                            OutputResources = false,
                            ThemesPath = _themesPath,
                            StaticLibraryPath = _staticLibraryPath,
                            WebServicePath = _webServicePath,
                            WebServiceInitialTreeNodesMethod = _getInitialTreeNodesWebServiceMethodName,
                            WebServiceBranchNodesMethod = _getBranchNodesWebServiceMethodName
                        };
            
            #endregion tree control

            #region drop control

            switch (_layout)
            {
                case ControlsToRender.TreeAndColumns:
                    _drop = new TreeColumnDrop();
                    _drop.Attributes.Add("nodenoun", "columns");
                    break;
                case ControlsToRender.TreeAndFilters:
                    _drop = new TreeFilterDrop();
                    _drop.Attributes.Add("nodenoun", "filters");
                    break;
                case ControlsToRender.TreeAndReportColumns:
                    _drop = new TreeReportColumnDrop();
                    _drop.Attributes.Add("nodenoun", "columns");
                    break;
                case ControlsToRender.TreeAndLookUpDisplayFields:
                    _drop = new TreeColumnDrop();
                    _drop.Attributes.Add("nodenoun", "display fields");
                    break;
            }
            _drop.ID = "tcDrop";
            _drop.Height = panelHeight;
            _drop.ShowButtonMenu = _showMenu;
            _drop.CssClass = "treediv" + (_layout == ControlsToRender.TreeAndFilters ? " treediv-filters" : "");
            _drop.OutputResources = false;
            _drop.ThemesPath = _themesPath;
            _drop.StaticLibraryPath = _staticLibraryPath;
            _drop.WebServicePath = _webServicePath;
            _drop.WebServiceSelectedNodesMethod = _getSelectedNodesWebServiceMethodName;

            #endregion drop control

            #endregion Trees
            
            #region button menus

            if (_showMenu)
            {
                Panel leftMenu = new Panel();
                Panel rightMenu = new Panel();

                leftMenu.CssClass = "treemenuleft";
                rightMenu.CssClass = "treemenu";

                leftMenu.Style.Add(HtmlTextWriterStyle.Position, "relative");
                rightMenu.Style.Add(HtmlTextWriterStyle.Position, "relative");

                _moveNodeToDropIcon.ImageUrl = _staticLibraryPath + "icons/16/plain/navigate_right.png";
                _removeAllNodesIcon.ImageUrl = _staticLibraryPath + "icons/16/plain/navigate_left2.png";
                _removeNodeIcon.ImageUrl = _staticLibraryPath + "icons/16/plain/navigate_left.png";
                _moveNodeUpIcon.ImageUrl = _staticLibraryPath + "icons/16/plain/navigate_open.png";
                _moveNodeDownIcon.ImageUrl = _staticLibraryPath + "icons/16/plain/navigate_close.png";

                _moveNodeToDropIcon.CssClass = "btn";
                _removeAllNodesIcon.CssClass = "btn";
                _removeNodeIcon.CssClass = "btn";
                _moveNodeUpIcon.CssClass = "btn";
                _moveNodeDownIcon.CssClass = "btn";

                _moveNodeToDropIcon.AlternateText = "Move selection to " + _rightTitle;
                _removeAllNodesIcon.AlternateText = "Remove all";
                _removeNodeIcon.AlternateText = "Remove selection";
                _moveNodeUpIcon.AlternateText = "Move selection up";
                _moveNodeDownIcon.AlternateText = "Move selection down";

                //imgMoveNodeToDrop.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                _moveNodeUpIcon.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                _moveNodeDownIcon.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                //imgMoveNodeToDrop.Style.Add("right", "5px");
                _moveNodeUpIcon.Style.Add("right", "23px");
                _moveNodeDownIcon.Style.Add("right", "5px");

                leftMenu.Controls.Add(_moveNodeToDropIcon);
                rightMenu.Controls.Add(_removeAllNodesIcon);
                rightMenu.Controls.Add(_removeNodeIcon);
                rightMenu.Controls.Add(_moveNodeUpIcon);
                rightMenu.Controls.Add(_moveNodeDownIcon);

                leftSection.Controls.Add(leftMenu);
                rightSection.Controls.Add(rightMenu);
            }

            #endregion button menus

            #region add the trees

            leftSection.Controls.Add(_tree);
            switch (_layout)
            {
                case ControlsToRender.TreeAndColumns:
                    rightSection.Controls.Add((TreeColumnDrop)this._drop);
                    break;
                case ControlsToRender.TreeAndFilters:
                    rightSection.Controls.Add((TreeFilterDrop)this._drop);
                    break;
                case ControlsToRender.TreeAndReportColumns:
                    rightSection.Controls.Add((TreeReportColumnDrop)this._drop);
                    break;
                case ControlsToRender.TreeAndLookUpDisplayFields:
                    rightSection.Controls.Add((TreeColumnDrop)this._drop);
                    break;
            }

            #endregion add the trees

            #region JavaScript

            //StringBuilder sJS = new StringBuilder("<script type=\"text/javascript\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(oTree.GenerateJavaScript()).AppendLine(oTreeDrop.GenerateJavaScript()).AppendLine(SharedTreeMethods.GenerateJavaScript(_themesPath)).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>");

            //Literal litJS = new Literal {Text = sJS.ToString()};

            #endregion JavaScript

            #region CSS

            Literal litCSS = new Literal {Text = SharedTreeMethods.GenerateCSS()};

            #endregion CSS

            this.Controls.Add(leftSection);
            this.Controls.Add(rightSection);

            #region Filter Modal Panel

            if (_layout == ControlsToRender.TreeAndFilters && _renderFilterModal)
            {
                _filterPanel = new Panel { ID = "pnlFilter", CssClass = "modalpanel formpanel formpanelsmall" };
                _filterPanel.Attributes.CssStyle.Add("display", "none");
                _filterModalHelpIcon = new Image { ID = "imgFilterModalHelp", ClientIDMode = ClientIDMode.Static, ImageUrl = StaticLibraryPath + "icons/24/plain/information.png", AlternateText = "Show Filter Editor Information" };
                _filterModalHelpIcon.Attributes.CssStyle.Add("display", "none");
                _filterModalHelpIcon.Attributes.CssStyle.Add("position", "absolute");
                _filterPanel.Controls.Add(_filterModalHelpIcon);

                _div = cTreeControls.CreateGenericControl("div", "divFilterModalHeading", cssClass: _titleCssClass, innerText: "Filter Details");

                _filterPanel.Controls.Add(_div);

                _filterCriteria = new DropDownList { ID = "ddlFilter" };
                _filterCriteria.Items.Add(new ListItem("[None]", "0"));
                _filterCriteria.Attributes.Add("onchange", "SEL.Trees.Filters.FilterModal.ChangeFilterCriteria(false);");

                _filterLabel = new Label { ID = "lblCriteriaFilter", AssociatedControlID = _filterCriteria.ID, CssClass = _mandatoryCssClass, Text = "Filter criteria*" };

                _div = cTreeControls.CreateGenericControl("div", cssClass: _fieldContainerCssClass, contentControls: new List<Control> { _filterLabel });

                _div.Controls.Add(cTreeControls.CreateGenericControl("span", cssClass: _dataFieldCssClass, contentControls: new List<Control> { _filterCriteria }));

                _div.Controls.Add(cTreeControls.CreateGenericControl("span", cssClass: _iconCssClass));

                _div.Controls.Add(cTreeControls.CreateGenericControl("span", cssClass: _tooltipCssClass, contentControls: new List<Control> { cTreeControls.CreateTooltip(cssClass: _tooltipCssClass, tooltipid: "033EC02E-AA57-4B3A-A0B5-FCB3459AEC00") }));

                _span = cTreeControls.CreateGenericControl("span", cssClass: _validatorCssClass);
                RequiredFieldValidator reqVal = new RequiredFieldValidator { ID = "reqFilter", ValidationGroup = _filterValidationGroup, ControlToValidate = _filterCriteria.ID, ErrorMessage = "Please select a value for Filter criteria.", Text = "*" };
                _span.Controls.Add(reqVal);
                _div.Controls.Add(_span);
                _filterPanel.Controls.Add(_div);

                _div = cTreeControls.CreateGenericControl("div", "divCriteria1", cssClass: _fieldContainerCssClass);
                
                _filterLabel = new Label { ID = "lblFilterValue1", CssClass = _mandatoryCssClass, Text = "Value 1*" };
                _filterLabel.Attributes.Add("onclick", "SEL.Trees.Filters.FilterModal.ToggleCalendar(1)");
                
                _span = cTreeControls.CreateGenericControl("span", cssClass: _dataFieldCssClass);
                _span.Controls.Add(new TextBox { ID = "txtFilterCriteria1", CssClass = "fillspan", MaxLength = 150 });
                _span.Controls.Add(cTreeControls.CreateGenericControl("span", id: "Criteria1Spacer", innerHtml: "&nbsp;"));
                _span.Controls.Add(new TextBox { ID = "txtTimeCriteria1", CssClass = "fillspan", MaxLength = 5 });
                _filterCriteria = new DropDownList { ID = "cmbFilterCriteria1" };
                _filterCriteria.Items.Add(new ListItem("[None]", "0"));
                _filterCriteria.Attributes.Add("onchange", "SEL.Trees.Filters.FilterModal.UpdateDropdownValidatorIfNoneSelected(SEL.Trees.DomIDs.Filters.FilterModal.Criteria1DropDown, SEL.Trees.DomIDs.Filters.FilterModal.RequiredValidator1);");
                _filterLabel.AssociatedControlID = _filterCriteria.ID; // which control doesn't matter, just to create 'for' attribute
                _span.Controls.Add(_filterCriteria);

                _div.Controls.Add(_filterLabel);
                _div.Controls.Add(_span);

                _span = cTreeControls.CreateGenericControl("span", cssClass: _iconCssClass);
                HtmlGenericControl img = cTreeControls.CreateGenericControl("img", "imgFilter1Calc");
                img.Attributes.Add("alt", "Open the date picker");
                img.Attributes.Add("src", "../images/icons/cal.gif");
                img.Attributes.Add("onclick", "SEL.Trees.Filters.FilterModal.ToggleCalendar(1);");
                _span.Controls.Add(img);
                _div.Controls.Add(_span);

                _div.Controls.Add(cTreeControls.CreateGenericControl("span", cssClass: _tooltipCssClass, contentControls: new List<Control> { cTreeControls.CreateTooltip(cssClass: _tooltipCssClass, tooltipid: "D8B49CC8-95E9-48B7-9C90-13A9C2E5CC0B") }));

                _span = cTreeControls.CreateGenericControl("span", "spanCriteria1Validator", cssClass: _validatorCssClass);
                reqVal = new RequiredFieldValidator { ID = "reqFilterCriteria1", Text = "*", ValidationGroup = FilterValidationGroup, ControlToValidate = "txtFilterCriteria1", ErrorMessage = "Please enter a Value.", Display = ValidatorDisplay.Dynamic, Enabled = false };
                CompareValidator cmpVal = new CompareValidator { ID = "cmpFilterCriteria1DataType", CultureInvariantValues = true, ValidationGroup = FilterValidationGroup, ControlToValidate = "txtFilterCriteria1", Type = ValidationDataType.Integer, Operator = ValidationCompareOperator.DataTypeCheck, Enabled = false, ErrorMessage = "Please enter an integer value for Value 1.", Text = "*", Display = ValidatorDisplay.Dynamic };
                CustomValidator custVal = new CustomValidator { ID = "cvTime1", ErrorMessage = "Please enter a valid time for Time 1 (hh:mm).", ValidationGroup = FilterValidationGroup, Text = "*", Enabled = true, ClientValidationFunction = "SEL.Trees.Filters.FilterModal.ValidateTime", Display = ValidatorDisplay.Dynamic };
                RangeValidator rngVal = new RangeValidator { ID = "rngIntFilterID1", ControlToValidate = "txtFilterCriteria1", ValidationGroup = FilterValidationGroup, CultureInvariantValues = true, Display = ValidatorDisplay.Dynamic, Enabled = false, ErrorMessage = " must be between -2,147,483,674 and 2,147,483,674.", Text = "*", Type = ValidationDataType.Integer, MaximumValue = "2147483647", MinimumValue = "-2147483647" };
                _span.Controls.Add(reqVal);
                _span.Controls.Add(cmpVal);
                _span.Controls.Add(custVal);
                _span.Controls.Add(rngVal);
                _div.Controls.Add(_span);
                _filterPanel.Controls.Add(_div);

                _div = cTreeControls.CreateGenericControl("div", "divCriteria2", cssClass: _fieldContainerCssClass);
                
                _filterLabel = new Label { ID = "lblFilterValue2", CssClass = _mandatoryCssClass, Text = "Value 2*" };
                _filterLabel.Attributes.Add("onclick", "SEL.Trees.Filters.FilterModal.ToggleCalendar(2)");
                
                _span = cTreeControls.CreateGenericControl("span", cssClass: _dataFieldCssClass);
                TextBox txt = new TextBox { ID = "txtFilterCriteria2", CssClass = "fillspan", MaxLength = 150 };
                _span.Controls.Add(txt);
                _span.Controls.Add(cTreeControls.CreateGenericControl("span", id: "Criteria2Spacer", innerHtml: "&nbsp;"));
                _span.Controls.Add(new TextBox { ID = "txtTimeCriteria2", CssClass = "fillspan", MaxLength = 5 });
                
                _filterLabel.AssociatedControlID = txt.ID; // which control doesn't matter, just to create 'for' attribute
                _div.Controls.Add(_filterLabel);
                _div.Controls.Add(_span);

                _span = cTreeControls.CreateGenericControl("span", cssClass: _iconCssClass);
                img = cTreeControls.CreateGenericControl("img", "imgFilter2Calc");
                img.Attributes.Add("alt", "Open the date picker");
                img.Attributes.Add("src", "../images/icons/cal.gif");
                img.Attributes.Add("onclick", "SEL.Trees.Filters.FilterModal.ToggleCalendar(2);"); // NEEDS LOCAL JS CALL
                _span.Controls.Add(img);
                _div.Controls.Add(_span);

                _div.Controls.Add(cTreeControls.CreateGenericControl("span", cssClass: _tooltipCssClass, contentControls: new List<Control> { cTreeControls.CreateTooltip(cssClass: _tooltipCssClass, tooltipid: "4104F301-F852-4EE1-9755-B0BBFB02C16C") }));

                _span = cTreeControls.CreateGenericControl("span", "spanCriteria2Validator", cssClass: _validatorCssClass);
                reqVal = new RequiredFieldValidator { ID = "reqFilterCriteria2", Text = "*", ValidationGroup = FilterValidationGroup, ControlToValidate = "txtFilterCriteria2", ErrorMessage = "Please enter a Value.", Display = ValidatorDisplay.Dynamic, Enabled = false };
                _span.Controls.Add(reqVal);

                cmpVal = new CompareValidator { ID = "cmpFilterCriteria2DataType", CultureInvariantValues = true, ValidationGroup = FilterValidationGroup, ControlToValidate = "txtFilterCriteria2", Type = ValidationDataType.Integer, Operator = ValidationCompareOperator.DataTypeCheck, Enabled = false, ErrorMessage = "Please enter an integer value for Value 2.", Text = "*", Display = ValidatorDisplay.Dynamic };
                _span.Controls.Add(cmpVal);

                cmpVal = new CompareValidator { ID = "cmpFilterCriteriaRange", CultureInvariantValues = true, ValidationGroup = FilterValidationGroup, ControlToValidate = "txtFilterCriteria2", Type = ValidationDataType.Integer, Operator = ValidationCompareOperator.GreaterThan, Enabled = false, ErrorMessage = "Please enter a value greater than X for Value 2.", Text = "*", Display = ValidatorDisplay.Dynamic, ControlToCompare = "txtFilterCriteria1" };
                _span.Controls.Add(cmpVal);

                custVal = new CustomValidator { ID = "cvTime2", ErrorMessage = "Please enter a valid time for Time 2 (hh:mm).", ValidationGroup = FilterValidationGroup, Text = "*", Enabled = true, ClientValidationFunction = "SEL.Trees.Filters.FilterModal.ValidateTime", Display = ValidatorDisplay.Dynamic };
                _span.Controls.Add(custVal);

                custVal = new CustomValidator { ID = "cvDateTime", ErrorMessage = "Please enter a valid date greater than Date and time 1 for Date and time 2.", ValidationGroup = FilterValidationGroup, Text = "*", Enabled = true, ClientValidationFunction = "SEL.Trees.Filters.FilterModal.ValidateDateTime", Display = ValidatorDisplay.Dynamic };
                _span.Controls.Add(custVal);

                custVal = new CustomValidator { ID = "cvTimeRange", ErrorMessage = "Please enter a valid time greater than Time 1 for Time 2.", ValidationGroup = FilterValidationGroup, Text = "*", Enabled = true, ClientValidationFunction = "SEL.Trees.Filters.FilterModal.ValidateTimeRange", Display = ValidatorDisplay.Dynamic };
                _span.Controls.Add(custVal);

                rngVal = new RangeValidator { ID = "rngIntFilterID2", ControlToValidate = "txtFilterCriteria2", ValidationGroup = FilterValidationGroup, CultureInvariantValues = true, Display = ValidatorDisplay.Dynamic, Enabled = false, ErrorMessage = " must be between -2,147,483,674 and 2,147,483,674.", Text = "*", Type = ValidationDataType.Integer, MaximumValue = "2147483647", MinimumValue = "-2147483647" };
                _span.Controls.Add(rngVal);

                _div.Controls.Add(_span);
                _filterPanel.Controls.Add(_div);

                _div = cTreeControls.CreateGenericControl("div", "divFilterList");
				_div.ClientIDMode = ClientIDMode.Static;
                _div.Attributes.CssStyle.Add("width", "400px");
                _div.Attributes.CssStyle.Add("height", "300px");
                _div.Attributes.CssStyle.Add("display", "none");
                _filterCriteria = new DropDownList { ID = "cmbFilterList", CssClass = "multiselect" };
                _filterCriteria.Attributes.CssStyle.Add("width", "200px");
                _filterCriteria.Attributes.Add("multiple", "multiple");
                _filterCriteria.Attributes.Add("name", "cmbFilterList");
                _div.Controls.Add(_filterCriteria);
                _filterPanel.Controls.Add(_div);

                _div = cTreeControls.CreateGenericControl("div", cssClass: "formbuttons");
                CSSButton filterSaveButton = new CSSButton { ID = "btnFilterSave", Text = "save", OnClientClick = "SEL.Trees.Filters.FilterModal.Save('" + FilterValidationGroup + "');return false;", ButtonSize = CSSButtonSize.Standard, UseSubmitBehavior = false };
                CSSButton filterCloseButton = new CSSButton { ID = "btnFilterClose", Text = "cancel", OnClientClick = "SEL.Trees.Filters.FilterModal.Cancel();return false;", ButtonSize = CSSButtonSize.Standard, UseSubmitBehavior = false };
                _div.Controls.Add(filterSaveButton);
                _div.Controls.Add(filterCloseButton);
                _filterPanel.Controls.Add(_div);

                this.Controls.Add(_filterPanel);

				// output a span to contain list item information
				_span = cTreeControls.CreateGenericControl("span", "tcPopupListItemContainer");
				_span.ClientIDMode = ClientIDMode.Static;
				Controls.Add(_span);

                // output a modal popup extender
                LinkButton lnk = new LinkButton { ID = "lnkFilter" };
                lnk.Attributes.CssStyle.Add("display", "none");
                this.Controls.Add(lnk);			

                ModalPopupExtender modExt = new ModalPopupExtender { ID = "modFilter", TargetControlID = "lnkFilter", PopupControlID = "pnlFilter", BackgroundCssClass = "modalBackground" };
                this.Controls.Add(modExt);

                _filterPanelRendered = true;
            }

            #endregion Filter Modal Panel

            //this.Controls.Add(litJS);
            //this.Controls.Add(litCSS);

            base.CreateChildControls();
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (_showMenu)
            {
                _moveNodeToDropIcon.Attributes.Add("onclick", new StringBuilder().Append("SEL.Trees.Node.Move.Between('").Append(_tree.ClientID).Append("', '")
                                                                .Append(_drop.ClientID).Append("', ")
                                                                .Append(_allowDuplicatesInDrop.ToString(CultureInfo.InvariantCulture).ToLower()).Append(");")
                                                                .ToString());
                _removeAllNodesIcon.Attributes.Add("onclick", "SEL.Trees.Node.Remove.All('" + _drop.ClientID + "');");
                _removeNodeIcon.Attributes.Add("onclick", "SEL.Trees.Node.Remove.Selected('" + _drop.ClientID + "');");
                _moveNodeUpIcon.Attributes.Add("onclick", "SEL.Trees.Node.Move.Up('" + _drop.ClientID + "');");
                _moveNodeDownIcon.Attributes.Add("onclick", "SEL.Trees.Node.Move.Down('" + _drop.ClientID + "');");
            }

            _tree.Attributes.Add("associatedcontrolid", _drop.ClientID);
            _drop.Attributes.Add("associatedcontrolid", _tree.ClientID);

            base.RenderControl(writer);

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "treeCombo" + this.ID, new StringBuilder("$(document).ready(function(){").Append(oTree.GenerateJavaScript()).AppendLine("});").Append("$(document).ready(function(){").Append(oTreeDrop.GenerateJavaScript()).Append(SharedTreeMethods.GenerateJavaScript(_themesPath)).AppendLine("});").ToString(), true);

            writer.Write(new StringBuilder("<script language=\"javascript\" type=\"text/javascript\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(_filterPanelRendered ? GenerateFilterDomIDs() : string.Empty).AppendLine(_tree.GenerateJavaScript()).AppendLine(_drop.GenerateJavaScript()).AppendLine(SharedTreeMethods.GenerateJavaScript(_themesPath)).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>").ToString());

            writer.Write(SharedTreeMethods.GenerateCSS());

            writer.Write(new StringBuilder("<!--[if lt IE 7]><style type=\"text/css\">#").Append(_tree.ClientID).Append(" { width: ").Append(this._leftPanelWidth.Value - 2).Append("px; } #").Append(_drop.ClientID).Append(" { width: ").Append(this._rightPanelWidth.Value - 2).Append("px; }</style><![endif]-->"));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        #endregion Method Overrides



        #region Methods

        private string GetHTML()
        {
            //<div><span style="display: inline-block; vertical-align: top; width: 522px;">
            //            <div class="sectiontitle">Attributes</div>
            //            <div style="background-color: #cccccc; border: 1px solid #999999; border-bottom-width: 0; text-align: right; padding: 1px 5px; font-size: 2px;">
            //                <asp:Image ID="imgNL" runat="server" AlternateText="Select attribute" onclick="selectField();" CssClass="btn" ImageUrl="/icons/16/plain/navigate_right.png" />
            //            </div>
            //            <helpers:Tree ID="treeViewFields" runat="server" style="height: 270px; overflow: auto; border: 1px solid #999999;">
            //        </helpers:Tree></span><span style="display: inline-block; vertical-align: top; margin-left: 10px; width: 300px;">
            //            <div class="sectiontitle">Chosen Columns</div>
            //            <div style="background-color: #cccccc; border: 1px solid #999999; border-bottom-width: 0; padding: 1px 5px; font-size: 2px;">
            //                <asp:Image ID="imgRA" runat="server" AlternateText="Remove all attributes" onclick="removeAllFields();" CssClass="btn" ImageUrl="/icons/16/plain/navigate_left2.png" />
            //                <asp:Image ID="imgRS" runat="server" AlternateText="Remove selected attribute" onclick="removeSelectedField();" CssClass="btn" ImageUrl="/icons/16/plain/navigate_left.png" />
            //                <span style="display: inline-block; font-size: 2px; width: 20px;"></span>
            //                <asp:Image ID="imgMU" runat="server" AlternateText="Move attribute up" onclick="moveFieldUp();" CssClass="btn" ImageUrl="/icons/16/plain/navigate_open.png" />
            //                <asp:Image ID="imgMD" runat="server" AlternateText="Move attribute down" onclick="moveFieldDown();" CssClass="btn" ImageUrl="/icons/16/plain/navigate_close.png" />
            //            </div>
            //            <helpers:Tree ID="treeViewColumns" runat="server" style="height: 270px; overflow: auto; border: 1px solid #999999;">
            //        </helpers:Tree></span></div>
            return new StringBuilder().ToString();
        }

        private string GenerateFilterDomIDs()
        {
            StringBuilder str = new StringBuilder();
            if (_filterPanelRendered)
            {
                str.Append("(function (t) {");
                str.Append("t.Panel = '" + _filterPanel.ClientID + "';\n");
                str.Append("t.Tree = '" + _tree.ClientID + "';\n");
                str.Append("t.Drop = '" + _drop.ClientID + "';\n");
                str.Append("t.FilterModalObj = '" + _filterPanel.FindControl("modFilter").ClientID + "';\n");
                str.Append("t.FilterModal.Heading = '" + _filterPanel.FindControl("divFilterModalHeading").ClientID + "';\n");
                str.Append("t.FilterModal.FilterDropDown ='" + _filterPanel.FindControl("ddlFilter").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1 = '" + _filterPanel.FindControl("txtFilterCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2 = '" + _filterPanel.FindControl("txtFilterCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1DropDown = '" + _filterPanel.FindControl("cmbFilterCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaListDropDown = '" + _filterPanel.FindControl("cmbFilterList").ClientID + "';\n");
                str.Append("t.FilterModal.FilterRequiredValidator = '" + _filterPanel.FindControl("reqFilter").ClientID + "';\n");
                str.Append("t.FilterModal.RequiredValidator1 = '" + _filterPanel.FindControl("reqFilterCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.RequiredValidator2 = '" + _filterPanel.FindControl("reqFilterCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.DataTypeValidator1 = '" + _filterPanel.FindControl("cmpFilterCriteria1DataType").ClientID + "';\n");
                str.Append("t.FilterModal.DataTypeValidator2 = '" + _filterPanel.FindControl("cmpFilterCriteria2DataType").ClientID + "';\n");
                str.Append("t.FilterModal.CompareValidator = '" + _filterPanel.FindControl("cmpFilterCriteriaRange").ClientID + "';\n");
                str.Append("t.FilterModal.TimeValidator1 = '" + _filterPanel.FindControl("cvTime1").ClientID + "';\n");
                str.Append("t.FilterModal.TimeValidator2 = '" + _filterPanel.FindControl("cvTime2").ClientID + "';\n");
                str.Append("t.FilterModal.DateAndTimeCompareValidator = '" + _filterPanel.FindControl("cvDateTime").ClientID + "';\n");
                str.Append("t.FilterModal.TimeRangeCompareValidator = '" + _filterPanel.FindControl("cvTimeRange").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaRow1 = '" + _filterPanel.FindControl("divCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaRow2 = '" + _filterPanel.FindControl("divCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaListRow = '" + _filterPanel.FindControl("divFilterList").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1ImageCalc = '" + _filterPanel.FindControl("imgFilter1Calc").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2ImageCalc = '" + _filterPanel.FindControl("imgFilter2Calc").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1Time = '" + _filterPanel.FindControl("txtTimeCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2Time = '" + _filterPanel.FindControl("txtTimeCriteria2").ClientID + "';\n");
                //str.Append("t.FilterModal.List = null;\n");
                str.Append("t.FilterModal.Criteria1Spacer = '" + _filterPanel.FindControl("Criteria1Spacer").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2Spacer = '" + _filterPanel.FindControl("Criteria2Spacer").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1Label = '" + _filterPanel.FindControl("lblFilterValue1").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2Label = '" + _filterPanel.FindControl("lblFilterValue2").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1ValidatorSpan = '" + _filterPanel.FindControl("spanCriteria1Validator").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2ValidatorSpan = '" + _filterPanel.FindControl("spanCriteria2Validator").ClientID + "';\n");
                str.Append("t.FilterModal.IntegerValidator1 = '" + _filterPanel.FindControl("rngIntFilterID1").ClientID + "';\n");
                str.Append("t.FilterModal.IntegerValidator2 = '" + _filterPanel.FindControl("rngIntFilterID2").ClientID + "';\n");            	
                str.Append("} (SEL.Trees.DomIDs.Filters));");
            }
            return str.ToString();
        }
        #endregion Methods
    }


    /// <summary>
    /// A control for creating a jsTree within an aspx page (www.jstree.com)
    ///  - only use if you need full control on rendering the tree, otherwise TreeCombo
    /// </summary>
    public class Tree : WebControl
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public Tree() : base(HtmlTextWriterTag.Div) { }

        #endregion Constructors



        #region Fields

        bool _outputResourceBlock = true;
        bool _showMenu = false;

        string _staticLibraryPath = "/static/";  // poor on the decoupling aspect
        string _themesPath = "../themes/";
        string _webServicePath = "~/PathTo/svcServiceName.asmx";
        string _getInitialTreeNodesWebServiceMethodName = "GetInitialTreeNodes";
        string _getBranchNodesWebServiceMethodName = "GetBranchNodes";

        #endregion Fields



        #region Properties

        /// <summary>
        /// Set to false if you need to prevent the javascript for the jsTree being created directly after the control html
        /// </summary>
        public bool OutputResources
        {
            get { return _outputResourceBlock; }
            set { _outputResourceBlock = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return _showMenu; }
            set { _showMenu = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return _staticLibraryPath; }
            set { _staticLibraryPath = value; }
        }


        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return _themesPath; }
            set { _themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return _webServicePath; }
            set { _webServicePath = ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial collection of jsTreeData
        /// </summary>
        public string WebServiceInitialTreeNodesMethod
        {
            get { return _getInitialTreeNodesWebServiceMethodName; }
            set { _getInitialTreeNodesWebServiceMethodName = value; }
        }
        /// <summary>
        /// The name of the method that will return the branch nodes when one is "opened"
        /// </summary>
        public string WebServiceBranchNodesMethod
        {
            get { return _getBranchNodesWebServiceMethodName; }
            set { _getBranchNodesWebServiceMethodName = value; }
        }

        #endregion Properties



        #region Enums

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (_outputResourceBlock)
            {
                writer.Write(new StringBuilder("<script type=\"\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(GenerateJavaScript()).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>"));
                writer.Write(SharedTreeMethods.GenerateCSS());
            }
        }

        #endregion Method Overrides



        #region Methods

        public string GenerateJavaScript()
        {
            return new StringBuilder(@" 
                $('#").Append(ClientID).Append(@"').jstree({
                    core: { animation: 0 },
                    themes: { theme: 'default' },
                    ui: { select_limit: 1 },
                    crrm: {},
                    dnd: {},
                    json_data: {
                        data: { data: 'loading...' },
                        ajax: {
                            url: '").Append(_webServicePath).Append("/").Append(_getBranchNodesWebServiceMethodName).Append(@"',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            async: true,
                            data: function (n) {
                                var nodeID = n.attr ? n.attr('id') : '';
                                var crumbs = n.attr ? n.attr('crumbs') : '';
                                var fieldID = n.attr ? n.attr('fieldid') : '00000000-0000-0000-0000-000000000000';
                                var metadata = n.metadata ? n.metadata : '';
                                return ").Append("\"{ fieldID: '\" + fieldID + \"', crumbs: '\" + (typeof crumbs === 'undefined' ? '' : crumbs) + \"', nodeID: '\" + nodeID + \"', metadata: '\" + metadata + \"' }\";").Append(@"
                            },
                            success: function (r) {
                                return r.d.data;
                            }
                        }
                    },
                    types: {
                        max_depth: -2,
                        max_children: -2,
                        types: {
                            'default': {
                                valid_children: 'none',
                                start_drag: false,
                                select_node: false,
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            group: {
                                valid_children: ['group', 'nodelink', 'node'],
                                icon: { image: '").Append(StaticLibraryPath).Append(@"icons/16/Plain/folder_blue.png' },
                                start_drag: false,
                                select_node: function (e) {
                                    this.toggle_node(e);
                                    return false;
                                },
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            nodelink: {
                                valid_children: ['group', 'nodelink', 'node'],
                                icon: { image: '").Append(StaticLibraryPath).Append(@"icons/16/Plain/table_view.png' },
                                start_drag: false,
                                select_node: function (e) {
                                    this.toggle_node(e);
                                    return false;
                                },
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            node: {
                                valid_children: 'none',
                                icon: { image: '").Append(StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' },
                                start_drag: function(e)
                                {
                                    $(e).addClass('tree-drag-add').data('outOfTree', false);
                                    $(document).bind('mouseup.tree', function ()
                                    {
                                        $(e).removeClass('tree-drag-add');
                                        var treeObj = $('#").Append(ClientID).Append(@"'),
                                            treeObjAssoc = $('#' + treeObj.attr('associatedcontrolid'));
                                        /*if ($(e).data('outOfTree') === true)
                                        {
                                            treeObj.jstree('delete_node', e);
                                            SEL.Trees.Tree.DisplayEmptyMessage('").Append(ClientID).Append(@"');
                                        }*/
                                        treeObjAssoc.find('.tree-drag-add').removeClass('tree-drag-add');
                                        treeObjAssoc.css('backgroundImage', 'none').find('li').css({ opacity: 1 });
                                        $(document).unbind('mouseup.tree');
                                    });
                                },
                                select_node: true,
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            start_drag: false,
                            select_node: false,
                            move_node: false,
                            delete_node: false,
                            hover_node: false
                        }
                    },
                    plugins: ['themes', 'json_data', 'ui', 'crrm', 'dnd', 'types']
                });

                $('#' + $('#").Append(ClientID).Append(@"').attr('associatedcontrolid')).mouseleave(function ()
                {
                    $('.tree-drag-add').data('outOfTree', false);
                    $('#' + $('#").Append(ClientID).Append(@"').attr('associatedcontrolid'))
                        .css('backgroundImage', 'none')
                        .find('li').css({ opacity: 1 });
                })
                .mouseenter(function ()
                {
                    var dragObj = $('.tree-drag-add');
                    dragObj.data('outOfTree', true);
                    if(dragObj.length > 0)
                    {
                        $('#' + $('#").Append(ClientID).Append(@"').attr('associatedcontrolid'))
                            .css('backgroundImage', 'url(").Append(StaticLibraryPath).Append(@"/images/backgrounds/gradients/availableFieldsDropHover.png)')
                            .find('li').css({ opacity: 0.8 });
                    }
                });

                SEL.Trees.New('").Append(ClientID).Append(@"', '").Append(WebServicePath).Append(@"', '").Append(WebServiceInitialTreeNodesMethod).Append(@"', '").Append(WebServiceBranchNodesMethod).Append(@"', '');
            ").MinifyToString();
        }

        #endregion Methods
    }

    /// <summary>
    /// Used for dropping simple columns that have been selected on to
    ///  - only use if you need full control on rendering the column drop area, otherwise TreeCombo
    /// </summary>
    public class TreeColumnDrop : WebControl, ITreeDrop
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public TreeColumnDrop() : base(HtmlTextWriterTag.Div) { }

        #endregion Constructors



        #region Fields

        bool _outputResourceBlock = true;
        bool _showMenu = false;

        string _staticLibraryPath = "/static/";  // poor on the decoupling aspect
        string _themesPath = "../themes/";
        string _webServicePath = "~/PathTo/svcServiceName.asmx";
        string _getSelectedNodesWebServiceMethodName = "GetSelectedNodes";

        #endregion Fields



        #region Properties

        /// <summary>
        /// Set to false if you need to prevent the javascript for the jsTree being created directly after the control html
        /// </summary>
        public bool OutputResources
        {
            get { return _outputResourceBlock; }
            set { _outputResourceBlock = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return _showMenu; }
            set { _showMenu = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return _staticLibraryPath; }
            set { _staticLibraryPath = value; }
        }


        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return _themesPath; }
            set { _themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return _webServicePath; }
            set { _webServicePath = ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial select nodes for a drop area
        /// </summary>
        public string WebServiceSelectedNodesMethod
        {
            get { return _getSelectedNodesWebServiceMethodName; }
            set { _getSelectedNodesWebServiceMethodName = value; }
        }

        #endregion Properties



        #region Enums

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (_outputResourceBlock)
            {
                writer.Write(new StringBuilder("<script type=\"\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(GenerateJavaScript()).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>"));
                writer.Write(SharedTreeMethods.GenerateCSS());
            }
        }

        #endregion Method Overrides



        #region Methods

        public string GenerateJavaScript()
        {
            return new StringBuilder(@"
                $('#").Append(ClientID).Append(@"').jstree({ 
                    core: { animation: 0 }, 
                    themes: { theme: 'default', dots: false }, 
                    ui: { select_limit: 1 }, 
                    crrm:
                    { 
                        move: { always_copy: 'multitree' }
                    }, 
                    dnd: {},
                    json_data: { data: {} }, 
                    types:
                    { 
                        max_depth: -2, 
                        max_children: -2, 
                        types:
                        {
                            'default':
                            {
                                valid_children: 'none', 
                                start_drag: false, 
                                select_node: false, 
                                move_node: false, 
                                delete_node: false, 
                                hover_node: false
                            },
                            node:
                            {
                                valid_children: 'none', 
                                icon: { image: '").Append(StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' }, 
                                start_drag: function(e)
                                {
                                    $(e).addClass('tree-drag-remove').data('outOfDrop', false);
                                    $(document).bind('mouseup.tree', function ()
                                    {
                                        $(e).removeClass('tree-drag-remove');
                                        var dropObj = $('#").Append(ClientID).Append(@"');
                                        var treeID = dropObj.attr('associatedcontrolid');
                                        if ($(e).data('outOfDrop') === true)
                                        {
                                            dropObj.jstree('delete_node', e);
                                            SEL.Trees.Node.Enable(treeID, e[0].id.replace('copy_', ''));
                                            SEL.Trees.Tree.DisplayEmptyMessage('").Append(ClientID).Append(@"');
                                        }
                                        $('#' + treeID).css('backgroundImage', 'none');
                                        $(document).unbind('mouseup.tree');
                                    });
                                },
                                select_node: true, 
                                move_node: true, 
                                delete_node: true, 
                                hover_node: false 
                            }, 
                            start_drag: false, 
                            select_node: false, 
                            move_node: false, 
                            delete_node: false, 
                            hover_node: false 
                        } 
                    }, 
                    unique: {}, 
                    plugins: ['themes', 'json_data', 'ui', 'dnd', 'crrm', 'types', 'unique'] 
                });
                
                var assocID = $('#").Append(ClientID).Append(@"').attr('associatedcontrolid');

                $('#").Append(ClientID).Append(@"').bind('move_node.jstree', function (e, d)
                {
                    SEL.Trees.Node.Disable(assocID, d.rslt.o.attr('id'));
                    SEL.Trees.Tree.DisplayEmptyMessage('").Append(ClientID).Append(@"');
                });

                $('#' + assocID).mouseleave(function ()
                {
                    $('.tree-drag-remove').data('outOfDrop', false);
                    $('#' + assocID).css('backgroundImage', 'none');
                })
                .mouseenter(function ()
                {
                    var dragObj = $('.tree-drag-remove');
                    dragObj.data('outOfDrop', true);
                    if(dragObj.length > 0) { $('#' + assocID).css('backgroundImage', 'url(").Append(StaticLibraryPath).Append(@"/images/backgrounds/gradients/availableFieldsDropHover.png)'); }
                });

                $('#' + assocID).bind('open_node.jstree', function (e, d)
                {
                    SEL.Trees.Node.Disable(assocID, d.args[0].attr('id'), 'refreshBranch');
                });

                SEL.Trees.New('").Append(ClientID).Append(@"', '").Append(WebServicePath).Append(@"', '', '', '").Append(WebServiceSelectedNodesMethod).Append(@"');
                ").MinifyToString();
        }

        #endregion Methods
    }

    /// <summary>
    /// Used for dropping field/column filters that have been selected on to
    ///  - only use if you need full control on rendering the filter drop area, otherwise TreeCombo
    /// </summary>
    public class TreeFilterDrop : WebControl, ITreeDrop
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public TreeFilterDrop() : base(HtmlTextWriterTag.Div) { }

        #endregion Constructors



        #region Fields

        bool _outputResourceBlock = true;
        bool _showMenu = false;

        string _staticLibraryPath = "/static/";  // poor on the decoupling aspect
        string _themesPath = "../themes/";
        string _webServicePath = "~/PathTo/svcServiceName.asmx";
        string _getSelectedNodesWebServiceMethodName = "GetSelectedNodes";

        #endregion Fields



        #region Properties

        /// <summary>
        /// Set to false if you need to prevent the javascript for the jsTree being created directly after the control html
        /// </summary>
        public bool OutputResources
        {
            get { return _outputResourceBlock; }
            set { _outputResourceBlock = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return _showMenu; }
            set { _showMenu = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return _staticLibraryPath; }
            set { _staticLibraryPath = value; }
        }


        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return _themesPath; }
            set { _themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return _webServicePath; }
            set { _webServicePath = ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial select nodes for a drop area
        /// </summary>
        public string WebServiceSelectedNodesMethod
        {
            get { return _getSelectedNodesWebServiceMethodName; }
            set { _getSelectedNodesWebServiceMethodName = value; }
        }

        #endregion Properties



        #region Enums

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (_outputResourceBlock)
            {
                writer.Write(new StringBuilder("<script type=\"\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(GenerateJavaScript()).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>"));
                writer.Write(SharedTreeMethods.GenerateCSS());
            }
        }

        #endregion Method Overrides



        #region Methods

        public string GenerateJavaScript()
        {
            return new StringBuilder(@"
            $('#").Append(ClientID).Append(@"').jstree({ 
                core: { animation: 0 }, 
                themes: {
                    theme: 'default',
                    dots: false
                }, 
                ui: { select_limit: 1 }, 
                crrm: {
                    move: {
                        always_copy: 'multitree'
                    }
                }, 
                json_data: { data: {} }, 
                types: {
                    max_depth: -2,
                    max_children: -2,
                    types:
                    {
                        'default': {
                            valid_children: 'none',
                            start_drag: false,
                            select_node: false,
                            move_node: false,
                            delete_node: false,
                            hover_node: false
                        },
                        node: {
                            valid_children: 'none',
                            icon: { image: '").Append(StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' },
                            start_drag: function(e) {
                                $(e).addClass('tree-drag-remove').data('outOfDrop', false);
                                $(document).bind('mouseup.tree', function () {
                                    $(e).removeClass('tree-drag-remove');
                                    var dropObj = $('#").Append(ClientID).Append(@"');
                                    if ($(e).data('outOfDrop') === true)
                                    {
                                        dropObj.jstree('delete_node', e);
                                        SEL.Trees.Tree.DisplayEmptyMessage('").Append(ClientID).Append(@"');
                                    }
                                    $('#' + dropObj.attr('associatedcontrolid')).css('backgroundImage', 'none');
                                    $(document).unbind('mouseup.tree');
                                });
                            },
                            select_node: true,
                            move_node: true,
                            delete_node: true,
                            hover_node: false
                        },
                        start_drag: false,
                        select_node: false,
                        move_node: false,
                        delete_node: false,
                        hover_node: false
                    }
                }, 
                plugins: ['themes', 'json_data', 'ui', 'dnd', 'crrm', 'types'] 
            });
            
            var assocID = $('#").Append(ClientID).Append(@"').attr('associatedcontrolid');
            $('#' + assocID).mouseleave(function ()
            {
                $('.tree-drag-remove').data('outOfDrop', false);
                $('#' + assocID).css('backgroundImage', 'none');
            })
            .mouseenter(function ()
            {
                var dragObj = $('.tree-drag-remove');
                dragObj.data('outOfDrop', true);
                if(dragObj.length > 0)
                {
                    $('#' + assocID).css('backgroundImage', 'url(").Append(StaticLibraryPath).Append(@"/images/backgrounds/gradients/availableFieldsDropHover.png)');
                }
            });

            var fm = SEL.Trees.Filters.FilterModal;
            $('#").Append(ClientID).Append(@"').bind('move_node.jstree', function (e, d) {                                
                if (d.rslt.cy)
                {
                    fm.Edit(d.rslt.oc,'").Append(ClientID).Append(@"');
                }

                SEL.Trees.Filters.RefreshFilterSummaryLayout('").Append(ClientID).Append(@"');

                SEL.Trees.Tree.DisplayEmptyMessage('").Append(ClientID).Append(@"');
            });

            $('#").Append(ClientID).Append(@"').bind('create.jstree', function (e, d) {                   
                fm.Edit(d.rslt.obj,'").Append(ClientID).Append(@"');
                SEL.Trees.Filters.RefreshFilterSummaryLayout('").Append(ClientID).Append(@"');
            });

            $('#").Append(ClientID).Append(@"').bind('loaded.jstree', function ()
            {
                $('#").Append(ClientID).Append(@"').jstree('data').data.core.li_height = 30;
            });									

			$('[id$=_tcFilters_pnlFilter]').appendTo('#aspnetForm');
			$('[id$=_tcFilters_modFilter_backgroundElement]').appendTo('#aspnetForm');
			$('#tcPopupListItemContainer').appendTo('#aspnetForm');

			$('[id$=_tcFilters_pnlFilter] .inputs INPUT').unbind('keypress.checkForEnter');
			SEL.Common.BindEnterKeyForSelector('[id$=_tcFilters_pnlFilter] .inputs INPUT', SEL.Trees.Filters.FilterModal.Save);

            SEL.Trees.New('").Append(ClientID).Append(@"', '").Append(WebServicePath).Append(@"', '', '', '").Append(WebServiceSelectedNodesMethod).Append(@"');

            ").MinifyToString();
        }

        #endregion Methods
    }

    /// <summary>
    /// Used for dropping field/column filters that have been selected on to
    ///  - only use if you need full control on rendering the filter drop area, otherwise TreeCombo
    /// </summary>
    public class TreeReportColumnDrop : WebControl, ITreeDrop
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public TreeReportColumnDrop() : base(HtmlTextWriterTag.Div) { }

        #endregion Constructors



        #region Fields

        bool _outputResourceBlock = true;
        bool _showMenu = false;

        string _staticLibraryPath = "/static/";  // poor on the decoupling aspect
        string _themesPath = "../themes/";
        string _webServicePath = "~/PathTo/svcServiceName.asmx";
        string _getSelectedNodesWebServiceMethodName = "GetSelectedNodes";

        #endregion Fields



        #region Properties

        /// <summary>
        /// Set to false if you need to prevent the javascript for the jsTree being created directly after the control html
        /// </summary>
        public bool OutputResources
        {
            get { return _outputResourceBlock; }
            set { _outputResourceBlock = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return _showMenu; }
            set { _showMenu = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return _staticLibraryPath; }
            set { _staticLibraryPath = value; }
        }


        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return _themesPath; }
            set { _themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return _webServicePath; }
            set { _webServicePath = ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial select nodes for a drop area
        /// </summary>
        public string WebServiceSelectedNodesMethod
        {
            get { return _getSelectedNodesWebServiceMethodName; }
            set { _getSelectedNodesWebServiceMethodName = value; }
        }

        #endregion Properties



        #region Enums

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (_outputResourceBlock)
            {
                writer.Write(new StringBuilder("<script type=\"\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(GenerateJavaScript()).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>"));
                writer.Write(SharedTreeMethods.GenerateCSS());
            }
        }

        #endregion Method Overrides



        #region Methods

        public string GenerateJavaScript()
        {
            return new StringBuilder(@"
                $('#").Append(ClientID).Append(@"').jstree({

                    core: { animation: 0 },

                    themes:
                    {
                        theme: 'default',
                        dots: false
                    },

                    ui: { select_limit: 1 },

                    crrm:
                    {
                        move: { always_copy: 'multitree' }
                    },

                    json_data: { data: {} },

                    types:
                    {
                        max_depth: -2,
                        max_children: -2,

                        types:
                        {
                            'default':
                            {
                                valid_children: 'none',
                                start_drag: false,
                                select_node: false,
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },

                            node:
                            {
                                valid_children: 'none',
                                icon: { image: '").Append(StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' },
                                start_drag: true,
                                select_node: true,
                                move_node: true,
                                delete_node: true,
                                hover_node: false
                            },

                            start_drag: false,
                            select_node: false,
                            move_node: false,
                            delete_node: false,
                            hover_node: false
                        }
                    },

                    plugins: ['themes', 'json_data', 'ui', 'dnd', 'crrm', 'types']
                });
            ").MinifyToString();
        }

        #endregion Methods
    }

    /// <summary>
    /// A definition of the properties and methods that a 
    /// tree drop area must implement to be useable with 
    /// a Tree as part of a TreeCombo
    /// </summary>
    interface ITreeDrop
    {
        string ID { get; set; }
        string ClientID { get; }
        Unit Height { get; set; }
        string CssClass { get; set; }
        AttributeCollection Attributes { get; }
        bool ShowButtonMenu { get; set; }
        bool OutputResources { get; set; }
        string WebServicePath { get; set; }
        string WebServiceSelectedNodesMethod { get; set; }
        string ThemesPath { get; set; }
        string StaticLibraryPath { get; set; }

        string GenerateJavaScript();
    }

    /// <summary>
    /// Shared methods
    /// </summary>
    internal class SharedTreeMethods
    {
        public static string GenerateCSS()
        {
            return new StringBuilder(@"
                    <style type=").Append("\"text/css\">").Append(@"
                        /* overwrite default jsTree styles to work with modalpopup */
                        #vakata-dragged { z-index: 10007 !important; }
                        #vakata-dragged.jstree-default ins { background:transparent !important; }
                        #vakata-dragged.jstree-default .jstree-ok { background:url('') !important; }
                        #vakata-dragged.jstree-default .jstree-invalid { background:url('') !important; }
                        #jstree-marker.jstree-default { z-index: 10006 !important; }
                        #jstree-marker-line.jstree-default { z-index: 10005 !important; }

                        li.tree-node-disabled { display: none; }

                        div.treemenu, div.treemenuleft { background-color: #cccccc; border: 1px solid #999999; border-bottom-width: 0; padding: 1px 5px; font-size: 2px; }
                        div.treemenuLeft { text-align: right; }
                        div.treediv { overflow: auto; border: 1px solid #999999; text-align: center; }
                        div.treediv ul { text-align: left; }

                        div.treediv-filters li, div.treediv-filters li a { height: 30px; }
                        div.treediv-filters li a span.nodeText,
                        div.treediv-filters li a span.filterInfo,
                        div.treediv-filters li a span.criteria,
                        div.treediv-filters li a span.editImage
                        {
                            display: inline-block;
                            width: 120px;
                            border: 1px solid transparent;
                            border-right-color: Grey;   
                            border-bottom-color: Grey;
                            border-left-width: 0px;  
                            text-align: center;
                            line-height: 28px;
                            height: 28px;
                            vertical-align: middle;
                            overflow: hidden;
                            font-size: 8pt;  
                        }

                        div.treediv-filters li a span.nodeText { width: 145px; }

                        div.treediv-filters li a span.filterInfo { width: 155px; }

                        div.treediv-filters li a span.criteria { width: 250px; }

                        div.treediv-filters li a span.editImage { width: 25px; }

                        div.treediv-filters li a span.editImage img { margin-top: 6px; }

                        div.treediv-filters ins { display: none !important; }

                        div.treediv-filters li, div.treediv-filters li a
                        {
                            height: 30px;
                            vertical-align: middle;
                            overflow: hidden;
                            padding: 0;
                            margin: 0;
                        }

                        div.treediv-filters.jstree-default .jstree-clicked, div.treediv-filters li a
                        {                            
                            border: 0px solid transparent;
                            padding: 0;
                            margin: 0;
                        }
                        div.treediv-filters li a.jstree-clicked span.nodeText,
                        div.treediv-filters li a.jstree-clicked span.filterInfo,
                        div.treediv-filters li a.jstree-clicked span.criteria,
                        div.treediv-filters li a.jstree-clicked span.editImage
                        {       
                            border-top-color: #000000;
                            border-bottom-color: #000000;
                            font-weight: bold;
                        }

                        .jstree-default a .jstree-icon { background-position:0px 0px; }                      
                    </style>").ToString();
        }

        public static string GenerateJavaScript(string themePath)
        {
            return new StringBuilder(@"$.jstree._themes = '").Append(themePath).Append(@"';").ToString();
        }

        public static void TryAddWebResources(ref ScriptManager parentSM)
        {
            if (parentSM == null)
            {
                // it may make sense to decouple this from the sel.X.js structure and have one seperately for the Helpers
                // if the library grows to the point where we'd want to use it elsewhere
                throw new InvalidOperationException("SpendManagementHelpers.Tree requires a ScriptManager on the page and for sel.main.js to have been registered");

                //string treeJS = Page.ClientScript.GetWebResourceUrl(typeof(Tree), "SpendManagementHelpers.Tree.sel.trees.js");
                //Page.ClientScript.RegisterClientScriptInclude("sel.trees.js", treeJS);
            }
            else
            {
                // when finished, would be good to combine and minify these, unless we can get the ScriptManager set up to do so
                // .Net 3.5/4 may have introduced a <CombinedScripts>
                parentSM.Scripts.Add(new ScriptReference("SpendManagementHelpers.Tree.jquery.jstree.js", "SpendManagementHelpers")); // this is a very slightly modified version of jsTree for the "unique" functionality being done by ID rather than Title
                parentSM.Scripts.Add(new ScriptReference("SpendManagementHelpers.Tree.sel.trees.js", "SpendManagementHelpers"));
            }
        }
    }
}

namespace SpendManagementHelpers.Trees
{
    /// <summary>
    /// jsTree is a jQuery plugin (we are using v1.0 as of 11/2011)
    /// - this class is used to package return data to the tree in a format that is useable with the json_data plugin (once serialised using JavascriptSerialize or via a web method return)
    /// </summary>
    public class jsTreeData
    {
        /// <summary>
        /// Required for serialisation
        /// </summary>
        public jsTreeData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodes">List of Nodes to add to the array</param>
        public jsTreeData(List<jsTreeNode> nodes)
        {
            data = nodes ?? new List<jsTreeNode>();
        }

        /// <summary>
        /// This array is the element that is usually required for the return of json_data
        /// </summary>
        public List<jsTreeNode> data;

        /// <summary>
        /// The class for a node on the tree
        /// </summary>
        public class jsTreeNode
        {
            /// <summary>
            /// Required for serialisation
            /// </summary>
            public jsTreeNode()
            {
            }

            /// <summary>
            /// The title
            /// </summary>
            public string data = "";

            /// <summary>
            /// A set of element attributes: 
            /// - id (a series of Guids from the tree "path" with _ seperator) 
            /// - rel (tree node type) 
            /// - fieldid (cField guid) 
            /// - joinviaid (JoinVia Guid)
            /// </summary>
            public LiAttributes attr = new LiAttributes();

            /// <summary>
            /// Open for a field, closed for a foreignKey or folder
            /// </summary>
            public string state = "";

            /// <summary>
            /// A metadata blob
            /// </summary>
            public Dictionary<string, object> metadata = null;

            /// <summary>
            /// A set of element attributes that get added to the &lt;li>
            /// </summary>
            public class LiAttributes
            {
                /// <summary>
                /// Required for serialisation
                /// </summary>
                public LiAttributes()
                {
                }

                /// <summary>
                /// a series of Guids from the tree "path" with _ seperator
                /// </summary>
                public string id = "";

                /// <summary>
                /// a series of names from the tree "path"
                /// </summary>
                public string crumbs = "";

                /// <summary>
                /// tree node type
                /// </summary>
                public string rel = "";

                /// <summary>
                /// cField guid
                /// </summary>
                public string fieldid = "";

                /// <summary>
                /// JoinVia Guid
                /// </summary>
                public int joinviaid = 0;
            }
        }
    }

    #region unused formats

    ///// <summary>
    ///// jsTree is a jQuery plugin (we are using v1.0 as of 11/2011)
    ///// - this class is used to package return data to the tree in a format that is useable with the json_data plugin (once serialised using JavascriptSerialize or via a web method return)
    ///// </summary>
    //public class jsTreeData
    //{
    //    /// <summary>
    //    /// Required by the serializer
    //    /// </summary>
    //    public jsTreeData() { }

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="nodes">List of Nodes to add to the array</param>
    //    public jsTreeData(List<jsTreeNode> nodes)
    //    {
    //        if (nodes != null)
    //        {
    //            data = nodes.ToArray();
    //        }
    //        else
    //        {
    //            data = new jsTreeNode[0];
    //        }
    //    }

    //    /// <summary>
    //    /// This array is the element that is usually required for the return of json_data
    //    /// </summary>
    //    public jsTreeNode[] data;

    //    /// <summary>
    //    /// The class for a node on the tree
    //    /// </summary>
    //    public class jsTreeNode
    //    {
    //        public jsTreeNode()
    //        {
    //            attr = new LiAttributes();
    //            data = new jsTreeNodeData();
    //        }

    //        public jsTreeNodeData data;
    //        public LiAttributes attr;
    //        public string state;

    //        public class LiAttributes
    //        {
    //            public LiAttributes() { }

    //            public string id = "";
    //            public string rel;
    //            public string fieldid;
    //        }

    //        public class jsTreeNodeData
    //        {
    //            public jsTreeNodeData()
    //            {
    //                attr = new AnchorAttributes();
    //            }

    //            public string title = "";
    //            public AnchorAttributes attr;

    //            public class AnchorAttributes
    //            {
    //                public AnchorAttributes() { }

    //                public string href = "#";
    //            }
    //        }
    //    }
    //}



    ///// <summary>
    ///// jsTree is a jQuery plugin (we are using v1.0 as of 11/2011)
    ///// - this class is used to package return data to the tree in a format that is useable with the json_data plugin (once serialised using JavascriptSerialize or via a web method return)
    ///// </summary>
    //public struct jsTreeData
    //{
    //    /// <summary>
    //    /// This array is the element that is usually required for the return of json_data
    //    /// </summary>
    //    public List<jsTreeNode> data;

    //    /// <summary>
    //    /// The class for a node on the tree
    //    /// </summary>
    //    public struct jsTreeNode
    //    {
    //        public string data; // title
    //        public LiAttributes attr;
    //        public string state;
    //        public List<KeyValuePair<string, object>> metadata;

    //        public struct LiAttributes
    //        {
    //            public string id;
    //            public string rel;
    //            public string fieldid;
    //        }
    //    }
    //}

    #endregion
}

public class cTreeControls
{
    public static HtmlGenericControl CreateGenericControl(string htmlControlType, string id = "", string cssClass="", string innerText = "", string innerHtml="", List<Control> contentControls = null)
    {
        HtmlGenericControl span = new HtmlGenericControl(htmlControlType);

        if (!string.IsNullOrEmpty(id))
        {
            span.ID = id;
        }

        if (!string.IsNullOrEmpty(cssClass))
        {
            span.Attributes.Add("class", cssClass);
        }

        if (!string.IsNullOrEmpty(innerText))
        {
            span.InnerText = innerText;
        }

        if (!string.IsNullOrEmpty(innerHtml))
        {
            span.InnerHtml = innerHtml;
        }

        if (contentControls != null)
        {
            foreach(Control c in contentControls)
            {
                span.Controls.Add(c);
            }
        }
        
        return span;
    }

    public static Image CreateTooltip(string id = "", string tooltipid = "", string altText = "", string cssClass = "")
    {
        Image img = new Image { ImageUrl = "~/shared/images/icons/16/plain/tooltip.png" };

        if (!string.IsNullOrEmpty(id))
        {
            img.ID = id;
        }

        if (!string.IsNullOrEmpty(altText))
        {
            img.AlternateText = altText;
        }

        if (!string.IsNullOrEmpty(cssClass))
        {
            img.CssClass = cssClass;
        }

        if (!string.IsNullOrEmpty(tooltipid))
        {
            img.Attributes.Add("onmouseover", "SEL.Tooltip.Show('" + tooltipid + "', this);");
        }

        return img;
    }
}