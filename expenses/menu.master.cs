#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


using SpendManagementLibrary;

using Spend_Management;
using Spend_Management.shared.code.GreenLight;
using System.Web;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

using AjaxControlToolkit;

using RestSharp;

using Spend_Management.shared.code.EasyTree;
using Spend_Management.shared.code;

using DataFormat = Syncfusion.Pdf.Parsing.DataFormat;

#endregion

/// <summary>
/// The menu.
/// </summary>
public partial class menu : MasterPage
{
    #region Fields
    /// <summary>
    /// The arr menuitems.
    /// </summary>
    private readonly ArrayList arrMenuItems = new ArrayList();

    /// <summary>
    /// The page.
    /// </summary>
    private string page;

    /// <summary>
    /// The s title.
    /// </summary>
    private string _title;
     
    /// <summary>
    /// This store individual CustomMenu.
    /// </summary>
    private CustomMenuStructure customMenu;

    #endregion

    #region Public Properties

    /// <summary>
    /// Sets the lit menu.
    /// </summary>
    public string LitMenu
    {
        set
        {
            this.litmenu.Text = value;
            this.litmenu.Visible = true;
        }
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

    /// <summary>
    /// Gets or sets a value indicating whether to use dynamic css. Outputs the styles.aspx and layout.css link elements if true.
    /// </summary>
    public bool UseDynamicCss { get; set; }

    /// <summary>
    /// Gets or sets the helpurl.
    /// </summary>
    public string Helpurl { get; set; }

    /// <summary>
    /// Gets the menuitems.
    /// </summary>
    public ArrayList MenuItems
    {
        get
        {
            return this.arrMenuItems;
        }
    }

    /// <summary>
    /// Gets or sets the menutitle.
    /// </summary>
    public string MenuTitle { get; set; }

    /// <summary>
    /// Gets or sets the onloadfunc.
    /// </summary>
    public string OnloadFunc { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title
    {
        get
        {
            return this._title;
        }

        set
        {
            this._title = value;
            this.litpagetitle.Text = value;
        }
    }

    /// <summary>
    /// This store if the menu has a View 
    /// </summary>
    private bool hasView = false;


    #endregion

    /// <summary>
    /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

    #region Public Methods and Operators

    /// <summary>
    /// The add menu item.
    /// </summary>
    /// <param name="logo">
    /// The logo.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <param name="label">
    /// The label.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="url">
    /// The url.
    /// </param>
    public void AddMenuItem(string logo, int size, string label, string description, string url)
    {
        var item = new cMenuItem(logo, size, label, description, url, "png");
        this.arrMenuItems.Add(item);
    }

    /// <summary>
    /// The add menu item.
    /// </summary>
    /// <param name="logo">
    /// The logo.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <param name="label">
    /// The label.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    /// <param name="newWindow">
    /// The new window.
    /// </param>
    public void AddMenuItem(string logo, int size, string label, string description, string url, string target, bool newWindow)
    {
        var item = new cMenuItem(logo, size, label, description, url, target, true);
        this.arrMenuItems.Add(item);
    }

    /// <summary>
    /// The add menu item.
    /// </summary>
    /// <param name="logo">
    /// The logo.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <param name="label">
    /// The label.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <param name="logoExt">
    /// The logo ext.
    /// </param>
    public void AddMenuItem(string logo, int size, string label, string description, string url, string logoExt)
    {
        var item = new cMenuItem(logo, size, label, description, url, logoExt);
        this.arrMenuItems.Add(item);
    }


	/// <summary>
	/// The add menu item.
	/// </summary>
	/// <param name="logo">
	/// The logo.
	/// </param>
	/// <param name="size">
	/// The size.
	/// </param>
	/// <param name="label">
	/// The label.
	/// </param>
	/// <param name="description">
	/// The description.
	/// </param>
	/// <param name="url">
	/// The url.
	/// </param>
	/// <param name="logoExt">
	/// The logo ext.
	/// </param>
	/// <param name="target">
	/// The target.
	/// </param>
	/// <param name="newWindow">
	/// The new window.
	/// </param>
	public void AddMenuItem(string logo, int size, string label, string description, string url, string logoExt, string target, bool newWindow)
	{
		cMenuItem item = new cMenuItem(logo, size, label, description, url, target, newWindow, logoExt);
		this.arrMenuItems.Add(item);
	}
	/// <summary>
	/// The add menu item.
	/// </summary>
	/// <param name="logo">
	/// The logo.
	/// </param>
	/// <param name="size">
	/// The size.
	/// </param>
	/// <param name="label">
	/// The label.
	/// </param>
	/// <param name="description">
	/// The description.
	/// </param>
	/// <param name="url">
	/// The url.
	/// </param>
	/// <param name="isCustom">
	/// If the item is custom
	/// </param>
	public void AddMenuItem(string logo, int size, string label, string description, string url, bool isCustom)
	{
		cMenuItem item = new cMenuItem(logo, size, label, description, url, isCustom);
		this.arrMenuItems.Add(item);
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
                "shared/viewentities.aspx?entityid={0}&viewid={1}", entity.entityid, view.viewid);

            viewName = view.viewname;
            if (view.showRecordCount)
            {
                var baseTable = new cTables(user.AccountID).GetTableByID(entity.table.TableID);
                string recordCount = new cGridNew(user.AccountID, user.EmployeeID, baseTable).getViewRecordCount(user, view).ToString();
                viewName = string.Format(view.viewname + " ({0})", recordCount);
            }
            this.AddMenuItem(view.MenuIcon, 48, viewName, view.MenuDescription, entityUrl, true);

        }
       
        this.AddCustomMenuIcons( menuId);
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
                this.AddMenuItem(string.IsNullOrEmpty(menuItem.CustomMenuIcon) ? "window_dialog.png": menuItem.CustomMenuIcon, 48, menuItem.CustomMenuName, menuItem.CustomMenuDescription, entityUrl, true);
            }
        }
    }

    #endregion

    #region Methods

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
        var currentUser = cMisc.GetCurrentUser();
        string theme = this.Page.StyleSheetTheme;
        this.UseDynamicCss = true;
        var clsMasterPageMethods = new cMasterPageMethods(currentUser, theme, this.ProductModuleFactory) { UseDynamicCSS = this.UseDynamicCss };
        clsMasterPageMethods.PreventBrowserFromCachingTheResponse();
        clsMasterPageMethods.RedirectUserBypassingChangePassword();
        clsMasterPageMethods.SetupJQueryReferences(ref this.jQueryCss, ref this.scriptman);
        clsMasterPageMethods.SetupSessionTimeoutReferences(ref scriptman, this.Page);
        clsMasterPageMethods.SetupDynamicStyles(ref this.htmlhead);

        cMasterPageMethods.AddBroadcastMessagePlugin(ref this.htmlhead, ref this.scriptman);

        if (!this.IsPostBack)
        {
            this.page = this.Request.Url.Segments.LastOrDefault();
            if (!currentUser.CheckUserHasAccesstoWebsite())
            {
                Response.Redirect(ErrorHandlerWeb.LogOut, true);
            }

            this.ViewState["accountid"] = currentUser.Account.accountid;
            this.ViewState["employeeid"] = currentUser.Employee.EmployeeID;

            // broadcast messages
            this.OnloadFunc = cMasterPageMethods.GetOnloadScriptForBroadcastMessages(this.page);

            string appPath = this.ResolveUrl("home.aspx").Replace("/home.aspx", string.Empty);

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
            clsMasterPageMethods.AttachOnLoadAndUnLoads(ref this.body, this.OnloadFunc);

            this.litstyles.Text = cColours.customiseStyles(false);

            if (decimal.Parse(this.MenuItems.Count.ToString(CultureInfo.InvariantCulture)) > 0)
            {
                this.litmenu.Text = this.CreateMenu();
            }
        }

        this.Page.ClientScript.RegisterClientScriptBlock(
            this.GetType(), "variables", clsMasterPageMethods.SetMasterPageJavaScriptVars);
        this.Page.ClientScript.RegisterStartupScript(
            this.GetType(), "modalvariables", clsMasterPageMethods.SetupGlobalMasterPopup(ref this.mdlMasterPopup));
        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sel.public.api", clsMasterPageMethods.SetupPublicApi(ref this.scriptman));
    }

    /// <summary>
    /// The create menu.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
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
                breadCrumbs.Append("<li class=\"active\"><a href=\"#\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\"  alt=\"\"/></i> " + currentNode.Title + "</a></li>");
            }
            else
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

                        list.Add("<li><a href=\"" + currentNode.Url + "\">  <label class=\"breadcrumb_arrow\">/</label>" + currentNode.Title + "</a></li>");
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
        output.Append("<div class=\"main-content-area\" style=\"min-height: 601px !important; background:#eeeeee !important;\"><div>");
        int j;
        decimal numitems = decimal.Parse(this.MenuItems.Count.ToString(CultureInfo.InvariantCulture));
        if (numitems == 0)
        {
            return string.Empty;
        }

        var menuitem = (cMenuItem)this.MenuItems[0];
        StringBuilder dynamicUrl = new StringBuilder();
        var iconPath = GlobalVariables.StaticContentLibrary;


        for (j = 0; j < this.MenuItems.Count; j++)
            {
            menuitem = (cMenuItem)this.MenuItems[j];
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
    
    #endregion
}