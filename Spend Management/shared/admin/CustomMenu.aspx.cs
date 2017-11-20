using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using Spend_Management.shared.webServices;

namespace Spend_Management.shared.admin
{
    /// <summary>
    /// Summary description for Custom Menu.
    /// </summary>
    public partial class CustomMenu : System.Web.UI.Page
    {
        /// <summary>
        /// Add Custom Menu Tree Nodes and Build a Tree
        /// </summary>
        /// <param name="customMenuService">An instance of the webservice svcCustomMenu</param>
        /// <returns>The JSON string of the tree data.</returns>
        public string AddMenuNodesInWebTree(SvcCustomMenu customMenuService)
        {
            var lstNodes = customMenuService.GetEasyTreeNodesForCustomMenu();
            return JsonConvert.SerializeObject(lstNodes);
        }

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Master.enablenavigation = false;
                var customurl = GlobalVariables.StaticContentLibrary + "/JsTreeThemes/default/style.min.css";
                Literal cssFile = new Literal() { Text = @"<link href=""" + this.ResolveUrl(customurl) + @""" type=""text/css"" rel=""stylesheet"" />" };
                this.Page.Header.Controls.Add(cssFile);

                CurrentUser user = cMisc.GetCurrentUser();
                if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GreenLightMenu, true))
                {
                    Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                }

                Master.title = string.Format("Custom Menu Structure");
                var menuService = new SvcCustomMenu();
                string customMenuTreeData = this.AddMenuNodesInWebTree(menuService);
                StringBuilder js = new StringBuilder();
                js.Append("$(document).ready(function() { \n");
                js.Append("$('#tabs').tabs({hide: function(event, ui) {$(ui.panel).animate({ opacity: 0.1},10);},show: function(event, ui) {$(ui.panel).animate({ opacity: 1.0},20);}}); \n");
                js.Append("$('#baseTree').tabs(); \n");
                js.Append("(function(r) { r.menuTreeData = " + customMenuTreeData + ";} (SEL.CustomMenuStructure.Elements)); \n");
                js.Append("SEL.CustomMenuStructure.Tree.PageLoad(); \n");
                js.Append("SEL.CustomMenuStructure.Icon.SetupIconSearch(); \n ");
                js.Append("});");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "js", js.ToString(), true);
            }
        }
    }
}