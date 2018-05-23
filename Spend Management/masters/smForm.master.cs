namespace Spend_Management
{
    using System;

    using AjaxControlToolkit;
    using Spend_Management.shared.code.EasyTree;
    using SpendManagementLibrary.Interfaces;
    using System.Web.UI.WebControls;
    using System.Web;
    using System.Text;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    /// <summary>
    /// smForm Master
    /// </summary>
    public partial class smForm : System.Web.UI.MasterPage, IMasterPage
    {
        //private int userid = 0;
        private string sTitle;
        private string sSubPageTitle;
        private int nHelpID;
        private bool bShowDummyMenu = false;
        private string sOnloadfunc;
        private bool bHome;
        private bool bStylesheet = true;
        string page;
        private bool bEnableNavigation = true;
        private bool bShowSubMenus = true;
        #region properties

        /// <summary>
        /// The small title indicating the product area, sets the main browser title also
        /// </summary>
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
        /// <summary>
        /// Larger title for the specific title for the page
        /// </summary>
        public string PageSubTitle
        {
            get { return sSubPageTitle; }
            set 
            { 
                sSubPageTitle = value;
                litSubPageTitle.Text = value;
                
            }
        }

        /// <summary>
        /// Set EntityBreadCrumb text
        /// </summary>
        public string EntityBreadCrumbText
        {
            get { return litEntityBreadCrumb.Text; }
            set { litEntityBreadCrumb.Text = value; litok.Text = value; }
        }

        /// <summary>
        /// Madcap help id for the related page
        /// </summary>
        public int helpid
        {
            get { return nHelpID; }
            set { nHelpID = value; }
        }

        public bool showdummymenu { get; set; }

        /// <summary>
        /// Sets a &lt;body onload=&quot;string&quot;&gt; javascript string to use
        /// </summary>
        public string onloadfunc
        {
            get { return sOnloadfunc; }
            set { sOnloadfunc = value; }
        }

        public bool home { get; set; }

        public bool stylesheet { get; set; }

        /// <summary>
        /// When false, disables the sitemap breadcrumbs and shows standard "click save/cancel" message
        /// </summary>
        public bool enablenavigation
        {
            get { return bEnableNavigation; }
            set { bEnableNavigation = value; }
        }

        /// <summary>
        /// Setting this to true will cause the page's navigation (side bar and header) from being displayed
        /// </summary>
        public bool DisableNavigation { get; set; }

        public string menutitle { get; set; }

        /// <summary>
        /// When false, hides the grey side menu and adds some css styles
        /// </summary>
        public bool ShowSubMenus
        {
            get { return bShowSubMenus; }
            set { bShowSubMenus = value; }
        }
        public ModalPopupExtender MasterPopup
        {
            get { return mdlMasterPopup; }
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

        /// <summary>
        /// Page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentPage = Page.Request.Url.Segments[Page.Request.Url.Segments.Length - 1].ToLower();

            if (currentPage.Contains("createreport.aspx") || currentPage.Contains("reportviewer.aspx")) // always force IE into compat mode for reports
            {
                Response.AddHeader("X-UA-Compatible", "IE=EmulateIE7");
            }
            else if (currentPage.Contains("aeentity.aspx") && BrowserIsIe8()) // force aeentiy in compat mode for IE8 only
            {
                Response.AddHeader("X-UA-Compatible", "IE=EmulateIE7");
            }
            else // otherwise force everything else to non-compat
            {
                Response.AddHeader("X-UA-Compatible", "IE=edge");
            }

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (enablenavigation == true)
            {
                LoadBreadcrumbs(currentUser);
            }
            this.UseDynamicCSS = true;
            string theme = this.Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory) { UseDynamicCSS = this.UseDynamicCSS };
            clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
            clsMasterPageMethods.RedirectUserBypassingChangePassword();
            clsMasterPageMethods.SetupJQueryReferences(ref jQueryCss, ref scriptman);
            clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
            cMasterPageMethods.AddBroadcastMessagePlugin(ref Head1, ref scriptman);      
            cMasterPageMethods.AddJQueryColorboxPlugin(ref Head1, ref scriptman);
            clsMasterPageMethods.SetupDynamicStyles(ref this.Head1);
            
            if (!this.IsPostBack)
            {
                this.page = this.Request.Url.Segments[this.Request.Url.Segments.GetLength(0) - 1];
                if (!currentUser.CheckUserHasAccesstoWebsite())
                {
                    Response.Redirect(ErrorHandlerWeb.LogOut, true);
                }
                this.onloadfunc = cMasterPageMethods.GetOnloadScriptForBroadcastMessages(this.page);
                if (!string.IsNullOrEmpty(this.onloadfunc))
                {
                    cMasterPageMethods.AddBroadcastMessagePlugin(ref this.Head1, ref this.scriptman);
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
                clsMasterPageMethods.AttachOnLoadAndUnLoads(ref body, this.onloadfunc);
                this.litstyles.Text = cColours.customiseStyles(false);

                //// Check the breadcrumbs should be disabled or not
                if (enablenavigation == false)
                {
                    sitemap.Visible = false;
                    if (!String.IsNullOrEmpty(litEntityBreadCrumb.Text))
                        litok.Text = litEntityBreadCrumb.Text;
                    else
                        litok.Text = clsMasterPageMethods.DisableBreadCrumbsMessage;
                }

                if (this.DisableNavigation)
                {
                    clsMasterPageMethods.HideNavigation(ref body);
                }

                this.Page.ClientScript.RegisterClientScriptInclude("validate", cMisc.Path + "/validate.js");

                ViewState["accountid"] = currentUser.Account.accountid;
                ViewState["employeeid"] = currentUser.Employee.EmployeeID;

            }

            if (bShowSubMenus == false)
            {
                pnlSubMenuHolders.Visible = false;
                litPageOptionsOverrideCss.Text = "<style type=\"text/css\"> #maindiv { margin-left: 25px; margin-right: 10px; } body { background-image: url();}  .submenuholder { width: 1px; }</style>";
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(ref mdlMasterPopup));
        }

        private bool BrowserIsIe8()
        {
            if ((this.Request.Browser.Browser == "IE" || this.Request.Browser.Browser == "InternetExplorer") && (this.Request.Browser.MajorVersion == 8)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create the Bread Crumbs for the page on navigation
        /// </summary>
        /// <param name="currentUser">Current user who logged into the system</param>
        protected void LoadBreadcrumbs(CurrentUser currentUser)
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
                //This page comes under the custom menu. 
                if (currentNode.Title == "DVLA Check Consent")
                {
                    var breadcrumbLength = 0;
                    var isTruncated = false;
                    var menuId =
                        new shared.code.CustomMenuStructure(currentUser.AccountID).GetCustomMenuIdByName(
                            "My Duty of Care Documents",
                            true);
                    var customMenuNode = new CustomMenuNodes();
                    var customMenuItem =
                        new shared.code.CustomMenuStructure(currentUser.AccountID).GetCustomMenuById(menuId);
                    breadCrumbs.Append("<li><a title='Home page' href='");
                    breadCrumbs.Append(
                        "/Home.aspx'> <i><img src='/static/images/expense/menu-icons/bradcrums-dashboard-icon.png'></i>Home</a></li> ");
                    breadCrumbs.Append(
                        customMenuNode.GenerateCustomMenuBreadcrumb(
                            customMenuItem,
                            ref breadcrumbLength,
                            ref isTruncated));
                    breadCrumbs.AppendFormat(
                        "<li><label class='breadcrumb_arrow'>/</label><a class='breadcrumbtitle' href='{0}'>{1}</a></li>",
                        string.Format("/shared/ViewCustomMenu.aspx?menuid={0}", menuId),
                        customMenuItem.CustomMenuName);
                    breadCrumbs.Append("<li><label class=\"breadcrumb_arrow\">/</label>" + currentNode.Title + "</li>");
                }
                else
                {
                    breadCrumbs.AppendFormat(
                        "<li><a href=\"{1}\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\" alt=\"\"/></i> {0}</a></li>",
                        rootNode.Title,
                        rootNode.Url);
                    while (currentNode.Title != rootNode.Title)
                    {
                        if (currentNode == null)
                        {
                            break;
                        }
                        else
                        {

                            list.Add(
                                "<li><a href=\"" + currentNode.Url + "\"><label class=\"breadcrumb_arrow\">/</label>"
                                + currentNode.Title + "</a></li>");
                            currentNode = currentNode.ParentNode;
                        }
                    }
                    for (int i = list.Count - 1; i > -1; i--)
                    {
                        breadCrumbs.Append(list[i]);
                    }
                }
            }
            litBreadcrumb.Text = breadCrumbs.ToString();
        }
        
    }
}
