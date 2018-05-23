namespace Spend_Management
{
    using System;
    using System.Linq;
    using System.Text;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;

    using Spend_Management.shared;

    using Syncfusion.Web.UI.HTML;

    /// <summary>
    /// The anonymous user.
    /// </summary>
    public partial class AnonymousUser : System.Web.UI.MasterPage
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title 
        {
            get { return this.litPageTitle.Text; }
            set { this.litPageTitle.Text = value; }
        }

        /// <summary>
        /// The _start processing time for the page.
        /// </summary>
        private DateTime _startProcessing;

        /// <summary>
        /// The _end processing time for the page.
        /// </summary>
        private DateTime _endProcessing;
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        /// <summary>
        /// The page pre innit event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Init(object sender, EventArgs e)
        {
            this._startProcessing = DateTime.Now;
        }

        /// <summary>
        /// The page_ pre render event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            this._endProcessing = DateTime.Now;
            this.PageStatistics.Text = string.Format("<!--***Page Processing Time***: Start time: {0}, End time: {1}, {2} Seconds in total -->", this._startProcessing.TimeOfDay, this._endProcessing.TimeOfDay, this._endProcessing.Subtract(this._startProcessing).TotalSeconds);
        }

        /// <summary>
        /// The set page title.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void SetPageTitle(string title)
        {
            this.litPageTitle.Text = title;
        }

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
            // Force IE out of compat mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");
            string brandName = "Expenses";
            Modules activeModule = HostManager.GetModule(this.Request.Url.Host);

            var module = this.ProductModuleFactory[activeModule];

            if (module != null)
            {
                brandName = module.BrandName;
            }

            Page.Title = string.Format("{0} logon", brandName);

            this.SetStyle();

            switch (activeModule)
            {
                case Modules.Contracts:
                case Modules.SmartDiligence:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    this.selfregbutton.Visible = false;
                    break;
            }

            var clsInfoMessages = new cInformationMessages();
            System.Collections.Generic.List<cInformationMessage> listInfoMessages = clsInfoMessages.GetMessages();

            if (listInfoMessages.Count > 0)
            {
                this.pnlInformation.Visible = true;
                var infoMessages = new StringBuilder();
                for (int i = 0; i < listInfoMessages.Count; i++)
                {
                    cInformationMessage tmpInfoMsg = listInfoMessages.ElementAt(i);
                    infoMessages.Append("<div class=\"informationmessage\"><div class=\"informationmessageheader\">" + tmpInfoMsg.Title + "</div>" + tmpInfoMsg.Message + "</div>");
                }

                this.litInformationText.Text = infoMessages.ToString();
            }
        }

        /// <summary>
        /// The set style.
        /// </summary>
        private void SetStyle()
        {
            Modules activeModule = HostManager.GetModule(this.Request.Url.Host);
            var moduleColour = new cColours(activeModule);

            var sb = new StringBuilder();
            sb.Append("<style type=\"text/css\">\n");

            sb.Append("#sitecontainer { \n");
            sb.Append("background-color: " + moduleColour.defaultFieldTxt + "; \n");
            sb.Append("margin-top: -12px; \n");
            sb.Append("} \n");

            sb.Append("#companylogodiv { \n");
            sb.Append("margin-bottom: 1px; \n");
            sb.Append("}\n");


            sb.Append("#pageheaderdiv { \n");
            sb.Append("background-color: " + moduleColour.defaultHeaderBGColour + "; \n");
            sb.Append("border-top: 1px solid " + moduleColour.defaultHeaderBGColour + "; \n");
            sb.Append("border-left: 1px solid " + moduleColour.defaultHeaderBGColour + "; \n");
            sb.Append("color: " + moduleColour.defaultHeaderBGColour + "; \n");


            sb.Append("}\n");

            sb.Append("#pagecontainerdiv { \n");
            sb.Append("background-color: " + moduleColour.defaultFieldTxt + "; \n");
            sb.Append("}\n");

            sb.Append("#pagesubpaneldiv { \n");
            sb.Append("background-color: " + moduleColour.defaultHeaderBreadcrumbTxtColour + "; \n");
            sb.Append("border-top: 1px solid #ccc ; \n");
            sb.Append("border-right: 1px solid #ccc; \n");
            sb.Append("border-bottom: 1px solid #ccc; \n");
            sb.Append("}\n");

            sb.Append(".pagesubpanelheader { \n");
            sb.Append("color: #FFFFFF; \n");
            sb.Append("background-color: " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");

            sb.Append("text-align: center; \n");
            sb.Append("}\n");

            sb.Append(".greygradient { \n");
            sb.Append("background-color: #FFFFFF; \n");
            sb.Append("color: " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("border-top: 1px solid " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("border-bottom: 1px solid " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("background-image: url(../shared/images/grey_gradient.png); \n");
            sb.Append("}\n");

            sb.Append("#information { \n");
            sb.Append("background-color: #FFFFFF; \n");
            sb.Append("}\n");

            sb.Append("#logonform { \n");
            sb.Append("background-color: " + moduleColour.defaultFieldTxt + "; \n");
            sb.Append("background-image: url(../shared/images/grey_gradient.png); \n");
            sb.Append("}\n");

            sb.Append("#loginfooter { \n");
            sb.Append("color: " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("}\n");

            sb.Append("#titlegradient { \n");
            if (activeModule == Modules.Expenses)
            {
                sb.Append("background-image: url(../shared/images/login_header_bg.png); border-bottom: #000000 1px solid; \n");
            }

            sb.Append("background-color: " + moduleColour.defaultHeaderBGColour + "; \n");
            if (activeModule == Modules.SmartDiligence)
            {
                sb.Append("background-color: #c7c6c6; \n");
            }

            sb.Append("}\n");

            sb.Append(".reqfield { \n");
            sb.Append("color: " + moduleColour.defaultFieldTxt + "; \n");
            sb.Append("}\n");

            sb.Append(".errortext { \n");
            sb.Append("color: " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("}\n");

            sb.Append("a { color: " + moduleColour.defaultFieldTxt + "; }\n");

            sb.Append(".stooltipcontent { \n");
            sb.Append("background-color: " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("border: solid 1px " + moduleColour.defaultSectionHeadingUnderlineColour + "; \n");
            sb.Append("color: " + moduleColour.defaultFieldTxt + "; \n");
            sb.Append("}\n");

            sb.Append("</style>\n");

            this.litStyles.Text = sb.ToString();
        }
    }
}
