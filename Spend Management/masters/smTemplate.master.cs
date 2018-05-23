
namespace Spend_Management
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Interfaces;
    using System.Text;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    public partial class smTemplate : System.Web.UI.MasterPage, IMasterPage
    {
        private string sTitle;
        private int nHelpID;
        private bool bShowDummyMenu = false;
        private string sOnloadfunc;
        private bool bHome;
        private bool bStylesheet = true;
        string page;
        private bool bEnableNavigation = true;
        private bool bShowSubMenus = true;
                
        #region properties
        public string title
        {
            get { return sTitle; }
            set
            {
                sTitle = value;
                Page.Title = value;
                litpagetitle.Text = value;
                litTempPageTitle.Text = value;
            }
        }

        public string PageSubTitle { get; set; }

        public int helpid
        {
            get { return nHelpID; }
            set { nHelpID = value; }
        }

        public bool showdummymenu
        {
            get { return bShowDummyMenu; }
            set { bShowDummyMenu = value; }
        }
        public string onloadfunc
        {
            get { return sOnloadfunc; }
            set { sOnloadfunc = value; }
        }
        public bool home
        {
            get { return bHome; }
            set { bHome = value; }
        }
        public bool stylesheet
        {
            get { return bStylesheet; }
            set { bStylesheet = value; }
        }
        public bool enablenavigation
        {
            get { return bEnableNavigation; }
            set { bEnableNavigation = value; }
        }

        public string menutitle { get; set; }

        /// <summary>
        /// Hides the left div container and all sub contents (page options etc)
        /// </summary>
        public bool ShowSubMenus
        {
            get { return bShowSubMenus; }
            set { bShowSubMenus = value; }
        }

        /// <summary>
        /// Outputs the styles.aspx and layout.css link elements if true
        /// </summary>
        public bool UseDynamicCSS
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        /// <summary>
        /// The _start processing time for the page.
        /// </summary>
        private DateTime _startProcessing;

        /// <summary>
        /// The _end processing time for the page.
        /// </summary>
        private DateTime _endProcessing;

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
            this.UseDynamicCSS = true;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("X-UA-Compatible", "IE=edge");
            LoadBreadcrumbs();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string theme = this.Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory) { UseDynamicCSS = this.UseDynamicCSS };
            clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
            clsMasterPageMethods.RedirectUserBypassingChangePassword();
            clsMasterPageMethods.SetupJQueryReferences(ref jQueryCss, ref scriptman);
            clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
            clsMasterPageMethods.SetupDynamicStyles(ref Head1);

            if (IsPostBack == false)
            {
                if (!currentUser.CheckUserHasAccesstoWebsite())
                {
                    Response.Redirect(ErrorHandlerWeb.LogOut, true);
                }

                var lituser = this.side_bar.FindControl("lituser") as Literal;
                var litlogo = this.header.FindControl("litlogo") as Literal;
                clsMasterPageMethods.SetupCommonMaster(
                    ref lituser,
                    ref litlogo,
                    ref this.favLink,
                    ref this.litstyles,
                    ref this.litemplogon,
                    ref this.windowOnError);
                clsMasterPageMethods.AttachOnLoadAndUnLoads(ref body, onloadfunc);
                this.litstyles.Text = cColours.customiseStyles(false);

                // Check of the breadcrumbs should be disabled
                if (enablenavigation == false)
                {
                    sitemap.Visible = false;
                    litok.Text = clsMasterPageMethods.DisableBreadCrumbsMessage;
                }

                this.Page.ClientScript.RegisterClientScriptInclude("validate", cMisc.Path + "/validate.js");

                ViewState["accountid"] = currentUser.Account.accountid;
                ViewState["employeeid"] = currentUser.Employee.EmployeeID;

                #region Broadcasts
                broadcastLocation location = broadcastLocation.notSet;
                switch (page)
                {
                    case "submitclaim.aspx":
                        location = broadcastLocation.SubmitClaim;
                        break;
                }
                if (this.Page.User.Identity.Name != "")
                {
                    cBroadcastMessages clsmessages = new cBroadcastMessages(currentUser.Account.accountid);
                    DataTable broadcast = clsmessages.getMessagesToDisplay(location, currentUser.Employee);
                    if (broadcast.Rows.Count != 0)
                    {
                        body.Attributes.Add("onload", "displayBroadcastMessage(" + broadcast.Rows[0]["broadcastid"] + ",'" + cMisc.Path + "/broadcastprovider.aspx','" + page + "');");
                    }
                }
                #endregion Broadcasts
            }


            if (bShowSubMenus == false)
            {
                pnlSubMenuHolders.Visible = false;
                litPageOptionsOverrideCss.Text = "<style> #maindiv { margin-left: 25px; } body { background-image: url();} .submenuholder { width: 1px; }</style>";
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(ref mdlMasterPopup));

            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sel.public.api", clsMasterPageMethods.SetupPublicApi(ref this.scriptman));
        }

        /// <summary>
        /// Creating Breadcrumbs.
        /// </summary>
        protected void LoadBreadcrumbs()
        {
            SiteMapNode currentNode = SiteMap.CurrentNode;
            SiteMapNode rootNode = SiteMap.RootNode;
            StringBuilder breadCrumbs = new StringBuilder();
            var list = new List<string>();
            if (currentNode == rootNode)
            {
                breadCrumbs.Append("<li class=\"active\"><a href=\"#\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\"  alt=\"\"/></i> " + currentNode.Title + "</a></li>");
            }
            else if (currentNode != null)
            {
                breadCrumbs.AppendFormat("<li><a href=\"{1}\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\"  alt=\"\"/></i>{0}</a></li>", rootNode.Title, rootNode.Url);
                while (currentNode.Title != rootNode.Title)
                {
                    if (currentNode == null)
                    {
                        break;
                    }
                    else
                    {

                        list.Add("<li><a href=\"" + currentNode.Url + "\"><label class=\"breadcrumb_arrow\">/</label>" + currentNode.Title + "</a></li>");
                        currentNode = currentNode.ParentNode;
                    }
                }
                for (int i = list.Count - 1; i > -1; i--)
                {
                    breadCrumbs.Append(list[i]);
                }
            }
            litBreadcrumb.Text = breadCrumbs.ToString();
        }
    } 
}
