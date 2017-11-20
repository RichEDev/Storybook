namespace expenses
{
    using System;

    using SpendManagementLibrary.Interfaces;

    using Spend_Management;
    using SpendManagementLibrary.Enumerators;
    using System.Web.UI.WebControls;
    using System.Web;
    using System.Collections.Generic;
    using System.Text;

    public partial class expform : System.Web.UI.MasterPage, IMasterPage
    {
        private string _title;

        private int nHelpID;
        private bool bShowDummyMenu = false;
        private string sOnloadfunc;
        private bool bHome;
        private bool bStylesheet = true;
        string page;
        private bool bEnableNavigation = true;
        #region properties

        public string title
        {
            get { return _title; }
            set
            {
                _title = value;
                _title = _title.Replace(" ", "&nbsp;");
                litpagetitle.Text = _title;
                litTempPageTitle.Text = _title;
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
        /// Outputs the styles.aspx and layout.css link elements if true
        /// </summary>
        public bool UseDynamicCSS
        {
            get;
            set;
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
           // Force IE out of compat mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");

            if (enablenavigation==true)
            { 
            LoadBreadcrumbs();
            }
            this.UseDynamicCSS = true;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string theme = this.Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme);
            clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
            clsMasterPageMethods.RedirectUserBypassingChangePassword();
            clsMasterPageMethods.UseDynamicCSS = UseDynamicCSS;
            clsMasterPageMethods.SetupJQueryReferences(ref jQueryCss, ref scriptman);
            clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
            clsMasterPageMethods.SetupDynamicStyles(ref Head1);

            cMasterPageMethods.AddBroadcastMessagePlugin(ref Head1, ref scriptman);

            if (IsPostBack == false)
            {
                this.page = Request.Url.Segments[Request.Url.Segments.GetLength(0) - 1];
                if (!currentUser.CheckUserHasAccesstoWebsite())
                {
                    Response.Redirect(ErrorHandlerWeb.LogOut, true);
                }

                if (this.Page.User.Identity.Name != string.Empty)
                {
                    // broadcast messages
                    string broadcastFunc = cMasterPageMethods.GetOnloadScriptForBroadcastMessages(this.page);
                    this.onloadfunc = this.onloadfunc == string.Empty
                                     ? broadcastFunc
                                     : string.Format("{0};{1}", this.onloadfunc, broadcastFunc);
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

                Page.ClientScript.RegisterClientScriptInclude("validate", cMisc.Path + "/validate.js");
                
                ViewState["accountid"] = currentUser.Account.accountid;
                ViewState["employeeid"] = currentUser.Employee.EmployeeID;
               
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
