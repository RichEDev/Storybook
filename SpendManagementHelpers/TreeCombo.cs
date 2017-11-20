namespace SpendManagementHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using SpendManagementHelpers.TreeControl;
    using SpendManagementHelpers.TreeControl.Drop;

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
        TreeDrop _drop;

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

        private bool _renderCustomModal;

        private string _customModalID;

        private CheckBox _filterRuntime;

        private bool _renderReportOptions;

        public bool RenderReportOptions
        {
            get
            {
                return this._renderReportOptions;
            }

            set
            {
                this._renderReportOptions = value;
            }
        }

        #endregion Fields



        #region Properties

        /// <summary>
        /// The client id of the contained tree control
        /// </summary>
        public string TreeClientID
        {
            get { return this._tree.ClientID; }
        }

        /// <summary>
        /// The client id of the contained tree drop control
        /// </summary>
        public string TreeDropClientID
        {
            get { return this._drop.ClientID; }
        }

        /// <summary>
        /// Determines which controls to include in this composite
        /// </summary>
        public ControlsToRender ComboType
        {
            get { return this._layout; }
            set { this._layout = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return this._showMenu; }
            set { this._showMenu = value; }
        }

        /// <summary>
        /// Allow the drop area to contain duplicate nodes from the same tree node
        /// </summary>
        public bool AllowDuplicatesInDrop
        {
            get { return this._allowDuplicatesInDrop; }
            set { this._allowDuplicatesInDrop = value; }
        }

        /// <summary>
        /// The width of the left panel, defaults to 50%, can be a pixel width
        /// </summary>
        public Unit LeftPanelWidth
        {
            get { return this._leftWidth; }
            set { this._leftWidth = value; }
        }

        /// <summary>
        /// The title to display on the left section
        /// </summary>
        public string LeftTitle
        {
            get { return this._leftTitle; }
            set { this._leftTitle = value; }
        }

        /// <summary>
        /// The title to display on the right section
        /// </summary>
        public string RightTitle
        {
            get { return this._rightTitle; }
            set { this._rightTitle = value; }
        }

        /// <summary>
        /// The title css class
        /// </summary>
        public string TitleCssClass
        {
            get { return this._titleCssClass; }
            set { this._titleCssClass = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return this._staticLibraryPath; }
            set { this._staticLibraryPath = value; }
        }

        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return this._themesPath; }
            set { this._themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return this._webServicePath; }
            set { this._webServicePath = this.ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial collection of jsTreeData
        /// </summary>
        public string WebServiceInitialTreeNodesMethod
        {
            get { return this._getInitialTreeNodesWebServiceMethodName; }
            set { this._getInitialTreeNodesWebServiceMethodName = value; }
        }
        /// <summary>
        /// The name of the method that will return the branch nodes when one is "opened"
        /// </summary>
        public string WebServiceBranchNodesMethod
        {
            get { return this._getBranchNodesWebServiceMethodName; }
            set { this._getBranchNodesWebServiceMethodName = value; }
        }
        /// <summary>
        /// The name of the method that will return the initial select nodes for a drop area
        /// </summary>
        public string WebServiceSelectedNodesMethod
        {
            get { return this._getSelectedNodesWebServiceMethodName; }
            set { this._getSelectedNodesWebServiceMethodName = value; }
        }

        /// <summary>
        /// Set the validation group for the
        /// </summary>
        public string FilterValidationGroup
        {
            get { return this._filterValidationGroup; }
            set { this._filterValidationGroup = value; }
        }

        /// <summary>
        /// Set this to FALSE to stop a filter modal being rendered with this control. TRUE by default.
        /// </summary>
        public bool RenderFilterModal
        {
            get { return this._renderFilterModal; }
            set { this._renderFilterModal = value; }
        }

        public bool RenderCustomModal
        {
            get
            {
                return this._renderCustomModal;
            }

            set
            {
                this._renderCustomModal = value;
            }
        }

        public string CustomModalID
        {
            get
            {
                return this._customModalID;
            }
            set
            {
                this._customModalID = value;
            }
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
            TreeAndLookUpDisplayFields = 3,

            /// <summary>
            /// The tree with a custom modal on drop (with edit) defined by CustomModalID
            /// </summary>
            TreeAndCustomModal = 4
        }

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(this.Page);
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

            this._leftPanelWidth = new Unit(522, UnitType.Pixel);
            if (this._leftWidth.Type == UnitType.Pixel && this._leftWidth.IsEmpty == false && this._leftWidth.Value < ((int)totalPanelWidth.Value - 10))
            {
                this._leftPanelWidth = this._leftWidth;
            }

            this._rightPanelWidth = new Unit((totalPanelWidth.Value - 10 - this._leftPanelWidth.Value), UnitType.Pixel);
            #endregion calculate widths

            #region calculate heights
            Unit totalHeight = (base.Height.Type == UnitType.Pixel && base.Height.IsEmpty == false) ? base.Height : Unit.Pixel(300);
            Unit panelHeight = (this._showMenu == true) ? Unit.Pixel((int)totalHeight.Value - 30) : totalHeight;
            #endregion calculate heights

            #region section spans
            HtmlGenericControl leftSection = new HtmlGenericControl("span");
            leftSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "inline-block");
            leftSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.VerticalAlign, "top");
            leftSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, this._leftPanelWidth.ToString());
            leftSection.Attributes.CssStyle.Add("-moz-inline-block", "");

            HtmlGenericControl rightSection = new HtmlGenericControl("span");
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "inline-block");
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.VerticalAlign, "top");
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, this._rightPanelWidth.ToString());
            rightSection.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginLeft, Unit.Pixel(10).ToString());
            rightSection.Attributes.CssStyle.Add("-moz-inline-block", "");
            #endregion section spans

            #region panel titles
            Panel leftTitle = new Panel();
            Panel rightTitle = new Panel();

            Literal leftTitleText = new Literal();
            Literal rightTitleText = new Literal();

            leftTitleText.Text = this._leftTitle;
            rightTitleText.Text = this._rightTitle;

            leftTitle.Controls.Add(leftTitleText);
            rightTitle.Controls.Add(rightTitleText);

            if (this._titleCssClass != string.Empty)
            {
                leftTitle.CssClass = this._titleCssClass;
                rightTitle.CssClass = this._titleCssClass;
            }

            leftSection.Controls.Add(leftTitle);
            rightSection.Controls.Add(rightTitle);
            #endregion panel titles

            #region Trees

            #region tree control

            this._tree = new Tree
                        {
                            ID = "tcTree",
                            Height = panelHeight,
                            ShowButtonMenu = this._showMenu,
                            CssClass = "treediv",
                            OutputResources = false,
                            ThemesPath = this._themesPath,
                            StaticLibraryPath = this._staticLibraryPath,
                            WebServicePath = this._webServicePath,
                            WebServiceInitialTreeNodesMethod = this._getInitialTreeNodesWebServiceMethodName,
                            WebServiceBranchNodesMethod = this._getBranchNodesWebServiceMethodName
                        };
            
            #endregion tree control

            #region drop control

            switch (this._layout)
            {
                case ControlsToRender.TreeAndColumns:
                    this._drop = new TreeColumnDrop();
                    this._drop.Attributes.Add("nodenoun", "columns");
                    break;
                case ControlsToRender.TreeAndFilters:
                    this._drop = new TreeFilterDrop();
                    this._drop.Attributes.Add("nodenoun", "filters");
                    break;
                case ControlsToRender.TreeAndReportColumns:
                    this._drop = new TreeReportColumnDrop();
                    this._drop.Attributes.Add("nodenoun", "columns");
                    break;
                case ControlsToRender.TreeAndLookUpDisplayFields:
                    this._drop = new TreeColumnDrop();
                    this._drop.Attributes.Add("nodenoun", "display fields");
                    break;
                case ControlsToRender.TreeAndCustomModal:
                    this._drop = new TreeCustomDrop();
                    this._drop.Attributes.Add("nodenoun", "custom");
                    break;
            }
            this._drop.ID = "tcDrop";
            this._drop.Height = panelHeight;
            this._drop.ShowButtonMenu = this._showMenu;
            this._drop.CssClass = "treediv";
            switch (this._layout)
            {
                case ControlsToRender.TreeAndFilters:
                    this._drop.CssClass = this._drop.CssClass + " treediv-filters";
                    break;
                case ControlsToRender.TreeAndCustomModal:
                    this._drop.CssClass = this._drop.CssClass + " treediv-custom";
                    break;
            }

            this._drop.OutputResources = false;
            this._drop.ThemesPath = this._themesPath;
            this._drop.StaticLibraryPath = this._staticLibraryPath;
            this._drop.WebServicePath = this._webServicePath;
            this._drop.WebServiceSelectedNodesMethod = this._getSelectedNodesWebServiceMethodName;

            #endregion drop control

            #endregion Trees
            
            #region button menus

            if (this._showMenu)
            {
                Panel leftMenu = new Panel();
                Panel rightMenu = new Panel();

                leftMenu.CssClass = "treemenuleft";
                rightMenu.CssClass = "treemenu";

                leftMenu.Style.Add(HtmlTextWriterStyle.Position, "relative");
                rightMenu.Style.Add(HtmlTextWriterStyle.Position, "relative");

                this._moveNodeToDropIcon.ImageUrl = this._staticLibraryPath + "icons/16/plain/navigate_right.png";
                this._removeAllNodesIcon.ImageUrl = this._staticLibraryPath + "icons/16/plain/navigate_left2.png";
                this._removeNodeIcon.ImageUrl = this._staticLibraryPath + "icons/16/plain/navigate_left.png";
                this._moveNodeUpIcon.ImageUrl = this._staticLibraryPath + "icons/16/plain/navigate_open.png";
                this._moveNodeDownIcon.ImageUrl = this._staticLibraryPath + "icons/16/plain/navigate_close.png";

                this._moveNodeToDropIcon.CssClass = "btn";
                this._removeAllNodesIcon.CssClass = "btn";
                this._removeNodeIcon.CssClass = "btn";
                this._moveNodeUpIcon.CssClass = "btn";
                this._moveNodeDownIcon.CssClass = "btn";

                this._moveNodeToDropIcon.AlternateText = "Move selection to " + this._rightTitle;
                this._removeAllNodesIcon.AlternateText = "Remove all";
                this._removeNodeIcon.AlternateText = "Remove selection";
                this._moveNodeUpIcon.AlternateText = "Move selection up";
                this._moveNodeDownIcon.AlternateText = "Move selection down";

                //imgMoveNodeToDrop.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                this._moveNodeUpIcon.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                this._moveNodeDownIcon.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                //imgMoveNodeToDrop.Style.Add("right", "5px");
                this._moveNodeUpIcon.Style.Add("right", "23px");
                this._moveNodeDownIcon.Style.Add("right", "5px");

                leftMenu.Controls.Add(this._moveNodeToDropIcon);
                rightMenu.Controls.Add(this._removeAllNodesIcon);
                rightMenu.Controls.Add(this._removeNodeIcon);
                rightMenu.Controls.Add(this._moveNodeUpIcon);
                rightMenu.Controls.Add(this._moveNodeDownIcon);

                leftSection.Controls.Add(leftMenu);
                rightSection.Controls.Add(rightMenu);
            }

            #endregion button menus

            #region add the trees

            leftSection.Controls.Add(this._tree);
            rightSection.Controls.Add(this._drop);
            //switch (this._layout)
            //{
            //    case ControlsToRender.TreeAndColumns:
            //        rightSection.Controls.Add(this._drop);
            //        break;
            //    case ControlsToRender.TreeAndFilters:
            //        rightSection.Controls.Add(this._drop);
            //        break;
            //    case ControlsToRender.TreeAndReportColumns:
            //        rightSection.Controls.Add(this._drop);
            //        break;
            //    case ControlsToRender.TreeAndLookUpDisplayFields:
            //        rightSection.Controls.Add(this._drop);
            //        break;
            //    case ControlsToRender.TreeAndCustomModal:
            //        rightSection.Controls.Add(this._drop);
            //        break;
            //}

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

            if (this._layout == ControlsToRender.TreeAndFilters && this._renderFilterModal)
            {
                TreeControls.CreateCriteriaModalPopup(this.Controls, StaticLibraryPath, ref _filterPanel, ref _filterRuntime, _validatorCssClass, _tooltipCssClass, _iconCssClass, _dataFieldCssClass, _fieldContainerCssClass, _mandatoryCssClass, _filterValidationGroup, _titleCssClass, this._renderReportOptions);
                this._filterPanelRendered = true;
            }

            #endregion Filter Modal Panel

            base.CreateChildControls();
        }

        

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (this._showMenu)
            {
                this._moveNodeToDropIcon.Attributes.Add("onclick", new StringBuilder().Append("SEL.Trees.Node.Move.Between('").Append(this._tree.ClientID).Append("', '")
                                                                                 .Append(this._drop.ClientID).Append("', ")
                                                                                 .Append(this._allowDuplicatesInDrop.ToString(CultureInfo.InvariantCulture).ToLower()).Append(");")
                                                                                 .ToString());
                this._removeAllNodesIcon.Attributes.Add("onclick", "SEL.Trees.Node.Remove.All('" + this._drop.ClientID + "');");
                this._removeNodeIcon.Attributes.Add("onclick", "SEL.Trees.Node.Remove.Selected('" + this._drop.ClientID + "');");
                this._moveNodeUpIcon.Attributes.Add("onclick", "SEL.Trees.Node.Move.Up('" + this._drop.ClientID + "');");
                this._moveNodeDownIcon.Attributes.Add("onclick", "SEL.Trees.Node.Move.Down('" + this._drop.ClientID + "');");
            }
            
            this._tree.Attributes.Add("associatedcontrolid", this._drop.ClientID);
            this._drop.Attributes.Add("associatedcontrolid", this._tree.ClientID);

            base.RenderControl(writer);

            writer.Write(new StringBuilder("<script language=\"javascript\" type=\"text/javascript\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(TreeControls.GeneratePanelDomIDsForFilterModal(_filterPanelRendered, _renderReportOptions, _filterPanel, _tree, _drop, _filterRuntime)).AppendLine(this._tree.GenerateJavaScript()).AppendLine(this._drop.GenerateJavaScript()).AppendLine(SharedTreeMethods.GenerateJavaScript(this._themesPath)).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>").ToString());

            writer.Write(SharedTreeMethods.GenerateCSS());

            writer.Write(new StringBuilder("<!--[if lt IE 7]><style type=\"text/css\">#").Append(this._tree.ClientID).Append(" { width: ").Append(this._leftPanelWidth.Value - 2).Append("px; } #").Append(this._drop.ClientID).Append(" { width: ").Append(this._rightPanelWidth.Value - 2).Append("px; }</style><![endif]-->"));
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

        
        #endregion Methods
    }
}