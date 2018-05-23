
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    using System;

    using SpendManagementLibrary.Interfaces;
    using System.Web;
    using System.Text;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    /// <summary>
    /// smForm Master
    /// </summary>
    public partial class AuthenticatedUser : System.Web.UI.MasterPage, IMasterPage
    {
        private string sTitle;
        private string sSuperTitle;
        private bool bDisableBreadcrumbs = false;
        private bool bHideSideMenus = false;
        private string sOnloadFunction;
        private string sPage;
        private int nHelpID;

        #region properties
        /// <summary>
        /// The main (larger) page title
        /// </summary>
        public string MainTitle
        {
            get { return sTitle; }
            set
            {
                sTitle = value;
                litSubPageTitle.Text = value;
            }
        }

        /// <summary>
        /// Appears above the main page title in smaller font
        /// </summary>
        public string SuperScriptTitle
        {
            get { return sSuperTitle; }
            set
            {
                sSuperTitle = value;
                litpagetitle.Text = value;
            }
        }

        /// <summary>
        /// Madcap help id for the related page
        /// </summary>
        public int HelpID
        {
            get { return nHelpID; }
            set { nHelpID = value; }
        }

        /// <summary>
        /// Sets a &lt;body onload=&quot;string&quot;&gt; javascript string to use
        /// </summary>
        public string OnLoadFunction
        {
            get { return sOnloadFunction; }
            set { sOnloadFunction = value; }
        }

        /// <summary>
        /// When false, disables the sitemap breadcrumbs and shows standard "click save/cancel" message
        /// </summary>
        public bool DisableBreadcrumbs
        {
            get { return bDisableBreadcrumbs; }
            set { bDisableBreadcrumbs = value; }
        }

        /// <summary>
        /// When false, hides the grey side menu and adds some css styles
        /// </summary>
        public bool HideSideMenus
        {
            get { return bHideSideMenus; }
            set { bHideSideMenus = value; }
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
        /// Page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Force IE out of compat mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");
            LoadBreadcrumbs();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string theme = this.Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory) { UseDynamicCSS = true };
            clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
            clsMasterPageMethods.RedirectUserBypassingChangePassword();
            clsMasterPageMethods.SetupJQueryReferences(ref jQueryCss, ref scriptman);
            clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
            clsMasterPageMethods.SetupDynamicStyles(ref Head1);

            if (IsPostBack == false)
            {
                this.Page.Title = sTitle;
                if (!currentUser.CheckUserHasAccesstoWebsite())
                {
                    Response.Redirect(ErrorHandlerWeb.LogOut, true);
                }

                clsMasterPageMethods.AttachOnLoadAndUnLoads(ref body, sOnloadFunction);
                var lituser = this.side_bar.FindControl("lituser") as Literal;
                var litlogo = this.header.FindControl("litlogo") as Literal;
                clsMasterPageMethods.SetupCommonMaster(
                    ref lituser,
                    ref litlogo,
                    ref this.favLink,
                    ref this.litstyles,
                    ref this.litemplogon,
                    ref this.windowOnError);

                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                this.Page.ClientScript.RegisterClientScriptInclude("validate", cMisc.Path + "/validate.js");

                if (bDisableBreadcrumbs == true)
                {
                    sitemap.Visible = false;
                    litok.Text = clsMasterPageMethods.DisableBreadCrumbsMessage;
                }

            }

            if (bHideSideMenus == true)
            {
                pnlSubMenuHolders.Visible = false;
                litPageOptionsOverrideCss.Text = "<style type=\"text/css\">#maindiv { margin-left: 25px; margin-right: 10px; } body { background-image: url(); }  .submenuholder { width: 1px; }</style>";
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(ref mdlMasterPopup));
        }

        public string title { get; set; }

        public string PageSubTitle { get; set; }

        public int helpid { get; set; }

        public bool showdummymenu { get; set; }

        public string onloadfunc { get; set; }

        public bool home { get; set; }

        public bool stylesheet { get; set; }

        public bool enablenavigation { get; set; }

        public string menutitle { get; set; }

        public bool UseDynamicCSS { get; set; }

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
                breadCrumbs.Append("<li class=\"active\"><a href=\"#\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\" alt=\"\"/></i> " + currentNode.Title + "</a></li>");
            }
            else if (currentNode != null)
            {
                breadCrumbs.Append("<li><a href=\"" + rootNode.Url + "\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\" alt=\"\"/></i> " + rootNode.Title + "</a></li>");
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
