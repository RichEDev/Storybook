namespace Spend_Management.shared.css
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    using BusinessLogic.Modules;

    using SpendManagementLibrary;

    /// <summary>
    /// Used witin a link html element to return CSS for this account.
    /// </summary>
    public partial class styles : System.Web.UI.Page
    {
        /// <summary>
        /// The _cls colours.
        /// </summary>
        private cColours _clsColours;

        /// <summary>
        /// The _current user.
        /// </summary>
        private CurrentUser _currentUser;

        /// <summary>
        /// The _active module.
        /// </summary>
        private Modules _activeModule;

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/css";
            var sb = new StringBuilder();
            string requestedStyle = "default";
            if (Request.QueryString.AllKeys.Contains("style"))
            {
                requestedStyle = string.IsNullOrEmpty(Request.QueryString["style"]) ? "default" : Request.QueryString["style"];
            }

            if (Request.IsAuthenticated) 
            {
                // we're logged in
                this._currentUser = cMisc.GetCurrentUser();
                if (requestedStyle != "logon")
                {
                    this._clsColours = new cColours(this._currentUser.AccountID, this._currentUser.CurrentSubAccountId, this._currentUser.CurrentActiveModule);
                }

                this._activeModule = this._currentUser.CurrentActiveModule;
            }
            else
            {
                this.SetActiveModule();
            }

            var clsInformationMessages = new cInformationMessages();

            if (clsInformationMessages.GetMessages().Count > 0)
            {
                sb.Append("#logonoutershadow { height: 338px; }");
                sb.Append("#logonoutercontainer { height:340px; width:560px; margin:-140px 0px 0px -280px; }");
            }
            else
            {
                sb.Append("#logonoutershadow { height: 224px; }");
                sb.Append("#logonoutercontainer { height: 226px; width: 560px; margin: -83px 0px 0px -280px; }");
            }

            this.litLogonPanelHeights.Text = sb.ToString();

            StringBuilder toolTipColours = new StringBuilder();

            string toolTipBorderColour = string.Empty;
            string toolTipBackgroundColour = string.Empty;

            switch (this._activeModule)
            {
                case Modules.Expenses:
                    toolTipBorderColour = "#013473";
                    toolTipBackgroundColour = "#7794CA";
                    break;
                case Modules.Contracts:
                   toolTipBorderColour = "#00241C";
                   toolTipBackgroundColour = "#00A0AF";
                    break;
                case Modules.SmartDiligence:
                    toolTipBorderColour = "rgb(30,35,38)";
                    toolTipBackgroundColour = "#C7114A";
                    break;
                case Modules.Greenlight:
                    toolTipBorderColour = "#E0E0E0";
                    toolTipBackgroundColour = "#E46C0A";
                    break;
                case Modules.GreenlightWorkforce:
                    toolTipBorderColour = "#E0E0E0";
                    toolTipBackgroundColour = "#5E6E66";
                    break;
                case Modules.CorporateDiligence:
                    toolTipBorderColour = "#43165A";
                    toolTipBackgroundColour = "#43165A";
                    break; 
           }

            if (Request.IsAuthenticated == false) 
            {
                // Only apply default tooltip styling to logon page.          
              toolTipColours.Append(@"
                .tooltipcontainer .tooltipcontent
                {
                    background-color: ").Append(toolTipBackgroundColour).Append(@";
                }
                .tooltipcontainer .tooltippointer
                {
                    border-top: 10px solid ").Append(toolTipBorderColour).Append(@";
                }
                ");
            }
          
            this.litToolTipColours.Text = toolTipColours.ToString();

			var titleBarBorderColour = this.Colours == null ? this.DefaultColours.sectionHeadingUnderlineColour : this.Colours.sectionHeadingUnderlineColour;
			var titleBarBackgroundColour = this.Colours == null ? this.DefaultColours.sectionHeadingUnderlineColour : this.Colours.sectionHeadingUnderlineColour;
			var titleBarTextColour = this.Colours == null ? this.DefaultColours.sectionHeadingUnderlineColour : this.Colours.sectionHeadingUnderlineColour;

            var errorModalColours = new StringBuilder();

            errorModalColours.Append(@"
                .errorModal
                {
                    background-color: #ffffff;
                    border-color: ").Append(titleBarBorderColour).Append(@";
                }
                .errorModalBody
                {
                }
                ");

            this.litErrorModalColours.Text = errorModalColours.ToString();
        }

        /// <summary>
        /// Gets the Colours to use for this account.
        /// </summary>
        public cColours Colours
        {
            get { return this._clsColours; }
        }

        /// <summary>
        /// Gets default colours, to be used when the user is not logged in
        /// </summary>
        public cColours DefaultColours
        {
            get
            {
                this.SetActiveModule();

                return new cColours(this._activeModule);
            }
        }

        /// <summary>
        /// The set active module.
        /// </summary>
        private void SetActiveModule()
        {
            this._activeModule = HostManager.GetModule(this.Request.Url.Host);
        }

        /// <summary>
        /// Gets the base URL for referencing images etc
        /// </summary>
        public string ImageRootReference
        {
            get
            {
                return GlobalVariables.StaticContentLibrary;
            }
        }

        /// <summary>
        /// Gets the base URL for the logon page images
        /// </summary>
        public string LogonPageImagesPath
        {
            get { return GlobalVariables.LogonPageImagesPath; }
        }
    }
}
