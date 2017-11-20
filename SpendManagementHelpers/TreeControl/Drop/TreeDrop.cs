namespace SpendManagementHelpers.TreeControl.Drop
{
    using System;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Abstract class for dropping simple columns that have been selected on to
    ///  - only use if you need full control on rendering the column drop area, otherwise TreeCombo
    /// </summary>
    public abstract class TreeDrop : WebControl
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        protected TreeDrop(HtmlTextWriterTag tag) : base(tag) { }

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
            get { return this._outputResourceBlock; }
            set { this._outputResourceBlock = value; }
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
        /// The name of the method that will return the initial select nodes for a drop area
        /// </summary>
        public string WebServiceSelectedNodesMethod
        {
            get { return this._getSelectedNodesWebServiceMethodName; }
            set { this._getSelectedNodesWebServiceMethodName = value; }
        }

        #endregion Properties

        #region Enums

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(this.Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (this._outputResourceBlock)
            {
                writer.Write(new StringBuilder("<script type=\"\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(this.GenerateJavaScript()).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>"));
                writer.Write(SharedTreeMethods.GenerateCSS());
            }
        }

        #endregion Method Overrides


        #region Methods

        public abstract string GenerateJavaScript();

        #endregion

    }
}
