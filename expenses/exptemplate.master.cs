namespace expenses
{
    using System;
    using System.Data;

    using SpendManagementLibrary.Interfaces;

    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using System.Web;
    using System.Text;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    public partial class exptemplate : System.Web.UI.MasterPage, IMasterPage
    {
        private string sTitle;

        private int nHelpID;
        private bool bShowDummyMenu = false;
        private string sOnloadfunc;
        private string sOnUnloadfunc;
        private bool bHome;
        private bool bStylesheet = true;
        string page;
        private bool bEnableNavigation = true;
        
        #region properties
        public string title
        {
            get { return sTitle; }
            set
            {
                sTitle = value;
                sTitle = sTitle.Replace(" ", "&nbsp;");
                litpagetitle.Text = sTitle;
                litTempPageTitle.Text = sTitle;
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
        public string onunloadfunc
        {
            get { return sOnUnloadfunc; }
            set { sOnUnloadfunc = value; }
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

        public bool isDelegate
        {
            get
            {
                if (Session["myid"] != null)
                {
                    if ((int)Session["delegatetype"] == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            // Force IE out of compat mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");
            LoadBreadcrumbs();
            this.UseDynamicCSS = true;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string theme = this.Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory) { UseDynamicCSS = this.UseDynamicCSS };
            clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
            clsMasterPageMethods.RedirectUserBypassingChangePassword();
            clsMasterPageMethods.SetupJQueryReferences(ref jQueryCss, ref scriptman);
            clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
            clsMasterPageMethods.SetupDynamicStyles(ref Head1);
            cMasterPageMethods.AddJQueryColorboxPlugin(ref Head1, ref scriptman);

            if (IsPostBack == false)
            {
                this.page = Request.Url.Segments[Request.Url.Segments.GetLength(0) - 1];
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
                clsMasterPageMethods.AttachOnLoadAndUnLoads(ref body, onloadfunc, onunloadfunc);
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
                #endregion
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(ref mdlMasterPopup));
        }

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
                breadCrumbs.Append("<li><a href=\"" + rootNode.Url + "\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\"  alt=\"\"/></i> " + rootNode.Title + "</a></li>");
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
