namespace Spend_Management
{
    using System;
    using System.Collections;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Combined Menu and Form page
    /// </summary>
    public partial class smMenuForm : System.Web.UI.MasterPage, IMasterPage
    {
        private ArrayList arrMenuitems = new ArrayList();
        private string sMenuTitle;

        private int userid = 0;
        private string sTitle;
        private string sSubPageTitle;
        private int nHelpID;
        private bool bShowDummyMenu = false;
        private string sOnloadfunc;
        private bool bHome;
        private bool bStylesheet = true;
        string page;
        private bool bEnableNavigation = true;

        #region properties

        /// <summary>
        /// Master Page title, when setting also sets the Page.Title
        /// </summary>
        public string title
        {
            get { return sTitle; }
            set
            {
                sTitle = value;
                Page.Title = sTitle;
                litpagetitle.Text = value;
            }
        }

        /// <summary>
        /// Master Page subtitle
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
        /// Help ID associated with the page
        /// </summary>
        public int helpid
        {
            get { return nHelpID; }
            set { nHelpID = value; }
        }

        /// <summary>
        /// Show the menu ---
        /// </summary>
        public bool showdummymenu
        {
            get { return bShowDummyMenu; }
            set { bShowDummyMenu = value; }
        }

        /// <summary>
        /// JavaScript function to place in the body onload attribute
        /// </summary>
        public string onloadfunc
        {
            get { return sOnloadfunc; }
            set { sOnloadfunc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
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

        public string menutitle
        {
            get { return sMenuTitle; }
            set { sMenuTitle = value; }
        }
        public ArrayList menuitems
        {
            get { return arrMenuitems; }
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
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string theme = this.Page.StyleSheetTheme;
            var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme) { UseDynamicCSS = this.UseDynamicCSS };
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

                clsMasterPageMethods.SetupCommonMaster(ref lituser, ref litlogo, ref favLink, ref litstyles, ref litemplogon, ref windowOnError);
                clsMasterPageMethods.AttachOnLoadAndUnLoads(ref body, this.onloadfunc);

                // Check of the breadcrumbs should be disabled
                if (enablenavigation == false)
                {
                    sitemap.Visible = false;
                    litok.Text = clsMasterPageMethods.DisableBreadCrumbsMessage;
                }

                this.Page.ClientScript.RegisterClientScriptInclude("validate", cMisc.Path + "/validate.js");

                ViewState["accountid"] = currentUser.Account.accountid;
                ViewState["employeeid"] = currentUser.Employee.EmployeeID;
  
                litstyles.Text = cColours.customiseStyles() + "<style type=\"text/css\">body{background-image:none;}</style>";

            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(ref mdlMasterPopup));
        }
    }
}
