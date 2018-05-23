using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using SpendManagementLibrary.Interfaces;

using Spend_Management;
using Spend_Management.shared.code.GreenLight;
using Spend_Management.shared.code.EasyTree;
using Spend_Management.shared.code;
using System.Text;
using System.Web;
using System.Globalization;
using System.Linq;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

/// <summary>
/// Menu Page
/// </summary>
public partial class smMenu : System.Web.UI.MasterPage, IMasterPage
{

    private ArrayList arrMenuitems = new ArrayList();

    private string sMenuTitle;
    private int userid = 0;
    private string sTitle;

    private string sHelpUrl;
    private bool bShowDummyMenu = false;
    private string sOnloadfunc;
    private bool bHome;
    private bool bStylesheet = true;

    string page;

	public string menuItemLabelColour;
	public string menuItemLabelHover;
    public bool hasView = false;
    CustomMenuStructure customMenu;

    #region properties
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

    public bool enablenavigation { get; set; }

    public string menutitle
    {
        get { return sMenuTitle; }
        set { sMenuTitle = value; }
    }
    public ArrayList menuitems
    {
        get { return arrMenuitems; }
    }
    public string title
    {
        get { return sTitle; }
        set
        {
            sTitle = value;
            litpagetitle.Text = value;
        }
    }

    public string PageSubTitle { get; set; }

    public int helpid { get; set; }

    public string LitMenu
    {
        set { litmenu.Text = value; litmenu.Visible = true; }
    }
    public string helpurl
    {
        get { return sHelpUrl; }
        set { sHelpUrl = value; }
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

    /// <summary>
    /// Outputs the styles.aspx and layout.css link elements if true
    /// </summary>
    public bool UseDynamicCSS
    {
        get;
        set;
    }

    /// <summary>
    /// Sets the lit title.
    /// </summary>
    public string LitTitle
    {
        set
        {
            this.litBreadCrumbs.Text = value;
            this.litBreadCrumbs.Visible = true;
        }
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
        // Force IE out of compat mode
        Response.AddHeader("X-UA-Compatible", "IE=edge");
        var currentUser = cMisc.GetCurrentUser();
        string theme = this.Page.StyleSheetTheme;
        var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory) { UseDynamicCSS = this.UseDynamicCSS };
        clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
        clsMasterPageMethods.RedirectUserBypassingChangePassword();
	    clsMasterPageMethods.SetupJQueryReferences(ref this.jQueryCss, ref this.scriptman);
        clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
        clsMasterPageMethods.SetupDynamicStyles(ref this.htmlhead);

        var colours = new cColours(currentUser.Account.accountid, currentUser.CurrentSubAccountId, currentUser.CurrentActiveModule);
		this.menuItemLabelColour = colours.defaultMenuOptionStandardTxtColour;
		this.menuItemLabelHover = colours.defaultMenuOptionHoverTxtColour;

        if (IsPostBack == false)
        {
            string appPath = ResolveUrl("menu.aspx").Replace("/masters/menu.aspx", string.Empty);
            if (!currentUser.CheckUserHasAccesstoWebsite())
            {
                Response.Redirect(ErrorHandlerWeb.LogOut, true);
            }

            if (!currentUser.Employee.AdminOverride)
			{
				////this.favouritesArea.Text = "<div></div>";
			}

            var lituser = this.side_bar.FindControl("lituser") as Literal;
            var litlogo = this.header.FindControl("litlogo") as Literal;
            clsMasterPageMethods.SetupCommonMaster(
                ref lituser,
                ref litlogo,
                ref this.favLink,
                ref this.litstyles,
                ref this.litemplogon,
                ref this.windowOnError,
                appPath);
            clsMasterPageMethods.AttachOnLoadAndUnLoads(ref body, onloadfunc);

            this.litstyles.Text = cColours.customiseStyles(false);

            ViewState["accountid"] = currentUser.Account.accountid;
            ViewState["employeeid"] = currentUser.Employee.EmployeeID;

			litstyles.Text = cColours.customiseStyles() + "<style type=\"text/css\">body{background-image:none;}</style>";

            #region broadcasts
            broadcastLocation location = broadcastLocation.notSet;
            switch (page)
            {
                case "home.aspx":
                    location = broadcastLocation.HomePage;
                    break;
            }

            cBroadcastMessages clsmessages = new cBroadcastMessages(currentUser.Account.accountid);
            DataTable broadcast = clsmessages.getMessagesToDisplay(location, currentUser.Employee);
            cEmployees clsEmployees = new cEmployees(currentUser.Account.accountid);
            DataTable notes = clsEmployees.getNotes(currentUser.Employee.EmployeeID);
            string onload = "";
            if (broadcast.Rows.Count != 0)
            {
                onload = "displayBroadcastMessage(" + broadcast.Rows[0]["broadcastid"] + ",'" + cMisc.Path + "/broadcastprovider.aspx','" + page + "');";

            }
            if (notes.Rows.Count != 0 && page == "home.aspx")
            {
                onload += "displayNotes(" + notes.Rows[0]["noteid"] + ");";
            }

            this.onloadfunc = onload;
            #endregion broadcasts

            if (decimal.Parse(menuitems.Count.ToString()) > 0)
            {
                
                litmenu.Text = CreateMenu();
            }
        }

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
        if (currentUser.Employee != null && currentUser.Employee.AdminOverride)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "menusortable", "SEL.Menu.SetupFavouritesArea();", true);
        }	
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="logo"></param>
    /// <param name="size"></param>
    /// <param name="label"></param>
    /// <param name="description"></param>
    /// <param name="url"></param>
    public void addMenuItem(string logo, int size, string label, string description, string url)
    {
        cMenuItem item = new cMenuItem(logo, size, label, description, url);
        arrMenuitems.Add(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logo"></param>
    /// <param name="size"></param>
    /// <param name="label"></param>
    /// <param name="description"></param>
    /// <param name="url"></param>
    /// <param name="target"></param>
    /// <param name="newWindow"></param>
    public void addMenuItem(string logo, int size, string label, string description, string url, string target, bool newWindow)
    {
        cMenuItem item = new cMenuItem(logo, size, label, description, url, target, true);
        arrMenuitems.Add(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logo"></param>
    /// <param name="size"></param>
    /// <param name="label"></param>
    /// <param name="description"></param>
    /// <param name="url"></param>
    /// <param name="logoExt"></param>
    public void addMenuItem(string logo, int size, string label, string description, string url, string logoExt)
    {
        cMenuItem item = new cMenuItem(logo, size, label, description, url, logoExt);
        arrMenuitems.Add(item);
    }

	/// <summary>
	/// Add a custom menu item
	/// </summary>
	/// <param name="logo">Menu logo</param>
	/// <param name="size">Menu logo size</param>
	/// <param name="label">Menu label</param>
	/// <param name="description">Menu description</param>
	/// <param name="url">Menu url</param>
	/// <param name="isCustom">True if the menu is custom</param>
	public void AddMenuItem(string logo, int size, string label, string description, string url, bool isCustom)
	{
		var item = new cMenuItem(logo, size, label, description, url, isCustom);
		this.arrMenuitems.Add(item);
	}

    /// <summary>
    /// This method adds in the custom entity views for the given menu id and user.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="menuId">
    /// The menu id.
    /// </param>
    public void AddCustomEntityViewMenuIcons(ICurrentUser user, int menuId)
    {
        var entities = new cCustomEntities(user);
        List<cCustomEntityView> menuItems = entities.getViewsByMenuId(menuId);
        var disabledModuleMenuViews = new DisabledModuleMenuViews(user.AccountID, (int)user.CurrentActiveModule);
        string viewName;

        foreach (cCustomEntityView view in menuItems)
        {
            if (disabledModuleMenuViews.IsViewDisabled(menuId, view.viewid))
            {
                continue;
            }

            var entity = entities.getEntityById(view.entityid);
            if (
                !user.CheckAccessRole(
                    AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false))
            {
                continue;
            }

            var entityUrl = string.Format(
                "../shared/viewentities.aspx?entityid={0}&viewid={1}", entity.entityid, view.viewid);

            viewName = view.viewname;
            if (view.showRecordCount)
            { 
            var baseTable = new cTables(user.AccountID).GetTableByID(entity.table.TableID);
            string recordCount = new cGridNew(user.AccountID, user.EmployeeID, baseTable).getViewRecordCount(user,view).ToString();
            viewName = string.Format(view.viewname + " ({0})", recordCount);
            }
            this.AddMenuItem(view.MenuIcon, 48, viewName, view.MenuDescription, entityUrl, true);
        }
        AddCustomMenuIcons(menuId);
    }

    /// <summary>
    /// This method add custom menu to the menu master page.
    /// </summary>
    /// <param name = "user" ></ param >
    /// < param name= "menuId" ></ param >

    public void AddCustomMenuIcons(int menuId)
    {
        var currentUser = cMisc.GetCurrentUser();
        customMenu = new CustomMenuStructure(currentUser.AccountID);
        List<CustomMenuStructureItem> menuItems = customMenu.GetCustomMenusByParentId(menuId);
        List<string> SystemMenus = new List<string> { "Home", "Administrative Settings", "Base Information", "Tailoring", "Base Information", "Tailoring", "Policy Information", "User Management", "System Options", "My Details" };
        foreach (string menu in SystemMenus)
        {
            if (menuItems.Any(x => x.CustomMenuName.ToLower() == menu.ToLower()))
            {
                menuItems.Remove(menuItems.Where(x => x.CustomMenuName == menu).FirstOrDefault());
            }
        }
        string theme = this.Page.StyleSheetTheme;
        cMasterPageMethods clsMasterPageMethods;
        foreach (CustomMenuStructureItem menuItem in menuItems)
        {
            clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory);
            if (clsMasterPageMethods.CheckForAnyViewInMenuTree(currentUser, menuItem.CustomMenuId))
            {
                var entityUrl = string.Format(
                   "/shared/ViewCustomMenu.aspx?menuid={0}", menuItem.CustomMenuId);
                this.AddMenuItem(string.IsNullOrEmpty(menuItem.CustomMenuIcon)? "window_dialog.png" : menuItem.CustomMenuIcon, 48, menuItem.CustomMenuName, menuItem.CustomMenuDescription, entityUrl, true);
            }
        }
    }

    private string CreateMenu()
    {
        if (SiteMap.CurrentNode.Title != "CustomMenuIcons")
        {
            SiteMapNode currentNode = SiteMap.CurrentNode;
            SiteMapNode rootNode = SiteMap.RootNode;
            StringBuilder breadCrumbs = new StringBuilder();
            breadCrumbs.Append("<ol class=\"breadcrumb\">");
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

            breadCrumbs.Append("</ol>");
            this.litBreadCrumbs.Text = breadCrumbs.ToString();
        }

        var output = new StringBuilder();
        output.Append("<div class=\"main-content-area\" style=\"background:#eeeeee !important;\"><div>");
        int j;
        decimal numitems = decimal.Parse(this.menuitems.Count.ToString(CultureInfo.InvariantCulture));
        if (numitems == 0)
        {
            return string.Empty;
        }

        var menuitem = (cMenuItem)this.menuitems[0];
        StringBuilder dynamicUrl = new StringBuilder();
        var iconPath = GlobalVariables.StaticContentLibrary;


        for (j = 0; j < this.menuitems.Count; j++)
        {
            menuitem = (cMenuItem)this.menuitems[j];
            bool even = ((double)j / 2) == Math.Floor(j / 2.0);

            if (menuitem.url != null)
            {
                if (menuitem.url.Length >= 10)
                {
                    if (menuitem.url.Substring(0, 10).ToLower() == "showpolicy")
                    {
                        dynamicUrl.Append("showpolicy('./policy.aspx');");
                    }
                    else
                    {
                        if (menuitem.target != null)
                        {
                            dynamicUrl.Append("window.open('" + this.ResolveUrl(menuitem.url) + "');");
                        }
                        else
                        {
                            dynamicUrl.Append("document.location='" + this.ResolveUrl(menuitem.url) + "';");
                        }
                    }
                }
                else
                {
                    if (menuitem.target != null)
                    {
                        dynamicUrl.Append("window.open('" + this.ResolveUrl(menuitem.url) + "');");
                    }
                    else
                    {
                        dynamicUrl.Append("document.location='" + this.ResolveUrl(menuitem.url) + "';");
                    }
                }
            }

            output.Append(" <div class=\"col-md-6\"><a style=\"cursor:pointer\" onclick=\"" + dynamicUrl + "\">");
            dynamicUrl.Clear();
            var iconName = menuitem.IsCustom ? menuitem.logo : menuitem.logo + "." + menuitem.LogoExt;

            output.Append("<div class=\"well\"><div class=\"media contact-info wow fadeInDown\" data-wow-duration=\"1000ms\" data-wow-delay=\"600ms\" style=\"margin-left: -6px;\">");
            output.Append("<div class=\"pull-left\" style=\"margin-top: 5px;\"><img src=\"" + iconPath + "/icons/48/new/" + iconName + "\" alt=\"\" /></div>");
            output.Append("<div class=\"media-body\"><h2 id=\"menuitemlabel" + j.ToString(CultureInfo.InvariantCulture) + "\">" + menuitem.label + "</h2>");
            output.Append("<p>" + menuitem.description + "</p>");
            output.Append("</div></div></div></a></div>");

            if (!even)
            {
                output.Append("<div class=\"clearfix\"></div>");
            }

        }

        output.Append("</div>");
        return output.ToString();
    }

   
    private string createMenuOld()
    {
        cMenuItem menuitem;


        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("<div class=menutitle>");
        output.Append(menutitle);
        output.Append("</div>");



        int i = 0;

        int height;
        decimal numitems;
        bool even = false;


        numitems = decimal.Parse(menuitems.Count.ToString());
        if (numitems == 0)
        {
            return "";
        }
        menuitem = (cMenuItem)menuitems[0];
        height = (menuitem.logosize + 20) * (int)Math.Floor((numitems / (decimal)2.0));

        output.Append("<div class=menu style=\"height: " + height + "px;\">");
        for (i = 0; i < menuitems.Count; i++)
        {
            if ((double)((double)i / 2) == (double)Math.Floor((double)i / 2.0))
            {
                even = true;
            }
            else
            {
                even = false;
            }
            menuitem = (cMenuItem)menuitems[i];

            if (even == true)
            {

                output.Append("<table width=\"95%\" align=center style=\"border:1px solid #fff; cursor:pointer;cursor:hand;margin-left:auto; margin-right:auto; margin-bottom: 20px;\">");
                output.Append("<tr>");
            }
            output.Append("<td width=45% valign=\"top\">");
            output.Append("<table id=\"menuitem" + i + "\" onclick=\"");
            if (menuitem.url.Length >= 10)
            {
                if (menuitem.url.Substring(0, 10).ToLower() == "showpolicy")
                {
                    output.Append("showpolicy('./policy.aspx');");
                }
                else
                {
                    if (menuitem.target != null)
                    {
                        output.Append("window.open('" + ResolveUrl(menuitem.url) + "');");
                    }
                    else
                    {
                        output.Append("document.location='" + ResolveUrl(menuitem.url) + "';");
                    }
                }
            }
            else
            {
                if (menuitem.target != null)
                {
                    output.Append("window.open('" + ResolveUrl(menuitem.url) + "');");
                }
                else
                {
                    output.Append("document.location='" + ResolveUrl(menuitem.url) + "';");
                }
            }

			var iconPath = GlobalVariables.StaticContentLibrary;
			var iconName = menuitem.IsCustom ? menuitem.logo : menuitem.logo + "." + menuitem.LogoExt;

			output.Append(
				"\" border=\"0\" width=\"100%\" onmouseover=\"menuItemOver(" + i + ");changeIcon('" + i.ToString() + "','" + iconPath + "/icons/" + menuitem.logosize + "/shadow/" + iconName
				+ "');\" onmouseout=\"menuItemOut(" + i + ");changeIcon('" + i.ToString() + "','" + iconPath + "/icons/" + menuitem.logosize + "/plain/" + iconName + "');\">");

            output.Append("<tr>");
            output.Append("<td style=\"vertical-align:top;width:58px;text-align:left;\">");

			output.Append("<img id=\"icon" + i.ToString() + "\" class=\"menuitemlogo\" src=\"" + iconPath + "/icons/" + menuitem.logosize + "/plain/" + iconName + "\" border=0>");
            output.Append("</td>");

            output.Append("<td>");

            output.Append("<span id=\"menuitemlabel" + i + "\" class=\"menuitemtitle\">" + menuitem.label + "</span>");
            output.Append("<hr id=\"menuitemline" + i + "\" class=\"mastermenuitemline\" style=\"color: " + menuItemLabelColour + "; background-color: " + menuItemLabelColour + ";\">");
            output.Append("<span class=menuitemdescription>");
            output.Append(menuitem.description);
            output.Append("</span>");
            output.Append("</td>");
            output.Append("</tr>");
            output.Append("</table>");
            output.Append("</td>");
            if (even == false || i == (arrMenuitems.Count - 1))
            {
                if ((arrMenuitems.Count - 1) == i && even == true)
                {
                    output.Append("<td style=\"width: 5%;\">&nbsp;</td>");
                    output.Append("<td width=45%>&nbsp;</td>");
                }
                output.Append("</tr>");
                output.Append("</table>");


            }
            else
            {
                output.Append("<td style=\"width: 5%;\">&nbsp;</td>");

            }


        }

        output.Append("</div>");
        return output.ToString();
    }
}
