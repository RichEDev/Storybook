using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Web.SessionState;
using SpendManagementLibrary;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using CacheDataAccess.Caching;
using Common.Logging;
using Configuration.Core;
using RestSharp;
using SpendManagementLibrary.Employees;
using Spend_Management.Bootstrap;
using Spend_Management.shared.code;
using Spend_Management.shared.code.GreenLight;
using Configuration = System.Configuration.Configuration;

namespace Spend_Management
{
    public class cMasterPageMethods
    {
        public CurrentUser CurrentUser { get; set; }

        public bool UseDynamicCSS { get; set; }

        /// <summary>
        /// store hasView flag indicate if menu has a view
        /// </summary>
        private bool hasView = false;

        /// <summary>
        /// This store individual CustomMenu.
        /// </summary>
        private CustomMenuStructure customMenu;

        /// <summary>
        /// Initialises a new instance of the cMasterPageMethods class.
        /// </summary>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="theme">
        /// The theme (stylesheet name) for the page.
        /// </param>
        public cMasterPageMethods(CurrentUser currentUser, string theme)
        {
            this.CurrentUser = currentUser;
            this.Theme = theme;
            this.UseDynamicCSS = true;
        }

        /// <summary>
        /// Analyzes Current user and Session to establish the current module.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        /// <returns>
        /// The current Modules module enum.
        /// </returns>
        public static Modules GetModuleEnum(string host)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            Modules returnModules = user == null ? HostManager.GetModule(host) : user.CurrentActiveModule;

            return returnModules;
        }

        public void SetupCommonMaster(ref Literal userInfo, ref Literal logo, ref HtmlLink favIcon, ref Literal litStyles, ref Literal delegateBar, ref Literal windowOnError)
        {
            Literal favArea = new Literal();
            favArea.Text = "<div></div>";

            this.SetupCommonMaster(ref userInfo, ref logo, ref favIcon, ref litStyles, ref delegateBar, ref windowOnError, string.Empty);
        }

        public void SetupCommonMaster(ref Literal userInfo, ref Literal logo, ref HtmlLink favIcon, ref Literal litStyles, ref Literal delegateBar, ref Literal windowOnError, ref Literal favouriteArea, string appPath)
        {
            // logo
            logo.Text = getLogo();
            // Info bar
            userInfo.Text = GenerateInfoBar();
            // Attach global event handler for site map
            SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(cBreadCrumbs.SetBreadCrumbs);
            // Fav Icon
            favIcon.Href = GetFavIcon();
            // cColour styles
            litStyles.Text = cColours.customiseStyles();

            if (HttpContext.Current.Session["myid"] != null)
            {
                delegateBar.Text = GenerateYouAreLoggedInAsX();
            }

            if (favouriteArea.Text != "<div></div>")
            {
                MenuFavourites menuFavourites = new MenuFavourites(CurrentUser);
                favouriteArea.Text = menuFavourites.SetupFavouriteMenuItems(appPath);
            }

            GenerateWindowOnError(ref windowOnError);
        }

        public void SetupCommonMaster(ref Literal userInfo, ref Literal logo, ref HtmlLink favIcon, ref Literal litStyles, ref Literal delegateBar, ref Literal windowOnError, string appPath = null)
        {

            // logo
            logo.Text = getLogo(true);

            //// User Name
            userInfo.Text = GetEmployeeName();
            //// Attach global event handler for site map
            SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(cBreadCrumbs.SetBreadCrumbs);
            //// Fav Icon
            favIcon.Href = GetFavIcon();
            //// cColour styles
           //// litStyles.Text = cColours.customiseStyles();

            if (HttpContext.Current.Session["myid"] != null)
            {
                delegateBar.Text = GenerateYouAreLoggedInAsX();
            }

            GenerateWindowOnError(ref windowOnError);
        }

        /// <summary>
        ///  Gets Employee Name
        /// </summary>
        /// <returns></returns>
        public string GetEmployeeName()
        {
            StringBuilder sbName = new StringBuilder();
            // Employee Name
            Employee activeEmployee;
            if (this.CurrentUser.Delegate != null)
            {
                activeEmployee = this.CurrentUser.Delegate;
            }
            else
            {
                activeEmployee = this.CurrentUser.Employee;
            }

            if (!string.IsNullOrEmpty(activeEmployee.Title))
            {
                sbName.Append(activeEmployee.Title);
            }
            sbName.Append(" " + activeEmployee.FullName);

            // Active Sub Account
            cAccountSubAccounts subAccounts = new cAccountSubAccounts(this.CurrentUser.Account.accountid);
            if (subAccounts.Count > 1)
            {
                sbName.Append(string.Format("<br /><em>{0}</em>", subAccounts.getSubAccountById(this.CurrentUser.CurrentSubAccountId).Description));
            }

            return sbName.ToString();
        }

        private HtmlLink LayoutCSS()
        {
            HtmlLink layout = new HtmlLink { ID = "layoutcss", Href = cMisc.Path + "/shared/css/layout.css" };
            layout.Attributes.Add("rel", "stylesheet");
            layout.Attributes.Add("type", "text/css");
            layout.Attributes.Add("media", "screen");

            return layout;
        }

        private HtmlLink ColoursCSS()
        {
            HtmlLink colours = new HtmlLink { ID = "colourscss", Href = cMisc.Path + "/shared/css/styles.aspx" };
            colours.Attributes.Add("rel", "stylesheet");
            colours.Attributes.Add("type", "text/css");
            colours.Attributes.Add("media", "screen");

            return colours;
        }

        private string SetjQueryCss()
        {
            return GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui-1.9.2.custom.css";
        }

        /// <summary>
        ///  Generates the user info bar
        /// </summary>
        /// <returns></returns>
        public string GenerateInfoBar()
        {
            List<string> lstInfo = new List<string>();
            StringBuilder sbInfoBar = new StringBuilder();


            // Employee Title and Name
            Employee activeEmployee;
            if (this.CurrentUser.Delegate != null)
            {
                activeEmployee = this.CurrentUser.Delegate;
            }
            else
            {
                activeEmployee = this.CurrentUser.Employee;
            }

            lstInfo.Add((string.IsNullOrEmpty(activeEmployee.Title) == false ? activeEmployee.Title + " " : "") + activeEmployee.FullName);

            // Employee position/jobtitle
            if (string.IsNullOrEmpty(activeEmployee.Position) == false)
            {
                lstInfo.Add(activeEmployee.Position);
            }

            // Active Sub Account
            cAccountSubAccounts clsSubAccs = new cAccountSubAccounts(this.CurrentUser.Account.accountid);
            if (clsSubAccs.Count > 1)
            {
                cAccountSubAccount subacc = clsSubAccs.getSubAccountById(this.CurrentUser.CurrentSubAccountId);
                lstInfo.Add("Active Sub Account: " + subacc.Description);
            }

            // Add the current date
            lstInfo.Add(DateTime.Today.ToLongDateString());

            foreach (string str in lstInfo)
            {
                sbInfoBar.Append(str + " | ");
            }

            // Remove the final 3 characters then return the string
            return sbInfoBar.ToString().Remove(sbInfoBar.Length - 3);
        }

        /// <summary>
        ///  Check if the company logo exists with a specific extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private bool CheckIfLogoExists(string extension)
        {
            HttpServerUtility svr = HttpContext.Current.Server;
            bool hasMatch = false;
            if (File.Exists(svr.MapPath(cMisc.Path + "/logos/" + this.CurrentUser.Account.accountid + "." + extension)) == true)
            {
                hasMatch = true;
            }
            return hasMatch;
        }

        /// <summary>
        ///  Returns the company logo string or a default one if no company logo is found
        /// </summary>
        /// <returns></returns>
        public string getLogo(bool check = false)
        {
            StringBuilder output = new StringBuilder();

            if (this.CurrentUser.CurrentActiveModule != Modules.SmartDiligence)
            {
                string ext = string.Empty;

                if (this.CheckIfLogoExists("jpg") == true)
                {
                    ext = "jpg";
                }
                else if (this.CheckIfLogoExists("gif") == true)
                {
                    ext = "gif";
                }
                else if (this.CheckIfLogoExists("png") == true)
                {
                    ext = "png";
                }

                string imageUrl = "<img id=\"clientlogoimg\" alt=\"\" src=\"" + cMisc.Path + "/logos/" + this.CurrentUser.Account.accountid + "." + ext + "\"/>";

                if (string.IsNullOrEmpty(ext) == true)
                {
                    if (this.CurrentUser.CurrentActiveModule == Modules.contracts)
                    {
                         output.Append("<img id=\"clientlogoimg\" alt=\"\" src=\"" + GlobalVariables.StaticContentLibrary +
                                     "/images/expense/selenity.png\"/>");
                    }
                    else
                    {
                        if (check == true)
                        {
                            output.Append("<img id=\"clientlogoimg\" alt=\"\" src=\"" + GlobalVariables.StaticContentLibrary +
                                     "/images/expense/selenity.png\"/>");
                        }
                        else
                        {
                            output.Append("<style type=\"text/css\">\n");
                            output.Append("#logodiv\n");
                            output.Append("{\n");
                            output.Append("background: transparent url(" + cMisc.Path +
                                          "/images/jigsaw1.gif) no-repeat top right; height: 61px;\n");
                            output.Append("}\n");
                            output.Append("</style>");
                        }
                    }
                }
                else
                {
                    output.Append(imageUrl);
                }
            }

            return output.ToString();
        }

        /// <summary>
        ///  Generate the fav icon
        /// </summary>
        /// <returns></returns>
        public string GetFavIcon()
        {
            return cMasterPageMethods.GetFavIcon(this.CurrentUser.CurrentActiveModule);

        }

        public static string GetFavIcon(Modules module)
        {
            switch (module)
            {
                case Modules.contracts:
                case Modules.expenses:
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.PurchaseOrders:
                    return VirtualPath + "/favicon.ico";
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    return VirtualPath + "/shared/images/icons/greenlight.ico";
                case Modules.CorporateDiligence:
                    return VirtualPath + "/shared/images/icons/corpd.ico";
                default:
                    return "/favicon.ico";
            }
        }


        /// <summary>
        /// Generates the you are logged in as X (as a delegate message)
        /// </summary>
        /// <returns></returns>
        public string GenerateYouAreLoggedInAsX()
        {
            cEmployees clsemployees = new cEmployees(this.CurrentUser.Account.accountid);
            //Employee empaccount = clsemployees.GetEmployeeById(this.CurrentUser.Employee.employeeid);
            StringBuilder output = new StringBuilder();
            output.Append("<div class=\"emplogonholder\">\n<div class=\"paneltitle\">You are currently logged in as " + this.CurrentUser.Employee.Title + " " + this.CurrentUser.Employee.Forename + " " + this.CurrentUser.Employee.Surname + "</div>\n<a href=\"" + VirtualPath + "/shared/process.aspx?process=5\"  class=\"loggoffdelegate\">Logoff employee account</a>\n</div>");

            return output.ToString();
        }

        public void AttachOnLoadAndUnLoads(ref HtmlGenericControl body, string onLoad, string unLoad = "")
        {
            if (onLoad != "")
            {
                body.Attributes.Add("onload", onLoad);
            }

            if (unLoad != "")
            {
                body.Attributes.Add("onunload", unLoad);
            }
        }

        /// <summary>
        /// The message used to disable the breadcrumbs
        /// </summary>
        public string DisableBreadCrumbsMessage
        {
            //get { return "Please click save at the bottom of the page to save your changes, otherwise, click cancel"; }
            get { return "<ol class=\"breadcrumb\"><li> Before you can continue, please confirm the action required at the bottom of your screen.</li></ol>"; }
        }

        /// <summary>
        /// Returns the virtual path
        /// </summary>
        private static string VirtualPath
        {
            get
            {
                if (System.Web.HttpRuntime.AppDomainAppVirtualPath == "/")
                {
                    return "";
                }
                else
                {
                    return System.Web.HttpRuntime.AppDomainAppVirtualPath;
                }
            }
        }

        /// <summary>
        /// Sets JavaScript vars, accountid, employeeid, appPath and theme
        /// </summary>
        public string SetMasterPageJavaScriptVars
        {
            get
            {
                cModules clsModules = new cModules();
                cModule module = clsModules.GetModuleByID((int)CurrentUser.CurrentActiveModule);

                StringBuilder currentUserInfo = new StringBuilder("\n\n");
                currentUserInfo.AppendLine("var CurrentUserInfo = {");
                currentUserInfo.AppendLine("'AccountID':" + CurrentUser.AccountID + ",");
                currentUserInfo.AppendLine("'SubAccountID':" + CurrentUser.CurrentSubAccountId + ",");
                currentUserInfo.AppendLine("'EmployeeID':" + CurrentUser.EmployeeID + ",");
                currentUserInfo.AppendLine(string.Format("'AdminOverride':{0},", CurrentUser.Employee.AdminOverride.ToString().ToLower()));
                currentUserInfo.AppendLine("'IsDelegate':" + CurrentUser.isDelegate.ToString().ToLower() + ",");
                currentUserInfo.AppendLine("'DelegateEmployeeID':" + (CurrentUser.isDelegate ? CurrentUser.Delegate.EmployeeID : 0) + ",");
                currentUserInfo.AppendLine("'Module': {'ID':" + (int)CurrentUser.CurrentActiveModule + ", 'Name':'" + CurrentUser.CurrentActiveModule.ToString() + "'},");
                var reqProperties = new cAccountSubAccounts(CurrentUser.AccountID).getFirstSubAccount().SubAccountProperties;
                currentUserInfo.AppendLine("'AllowEmpToSpecifyCarDOCOnAdd':" + reqProperties.AllowEmpToSpecifyCarDOCOnAdd.ToString().ToLower() + ",");
                currentUserInfo.AppendLine("'Vehicle': {'BlockTax':'" + reqProperties.BlockTaxExpiry.ToString().ToLower() + "', 'BlockMOT':'" + reqProperties.BlockMOTExpiry.ToString().ToLower() + "','BlockInsurance':'" + reqProperties.BlockInsuranceExpiry.ToString().ToLower() + "', 'BlockBreakDownCover':'" + reqProperties.BlockBreakdownCoverExpiry.ToString().ToLower() + "'}");

                currentUserInfo.AppendLine("};");

                return "<script language=\"javascript\" type=\"text/javascript\" >\nvar accountid = " + CurrentUser.Account.accountid + ";var employeeid = " + CurrentUser.Employee.EmployeeID + ";\nvar appPath = '" + VirtualPath + "';\nvar StaticLibPath = '" + GlobalVariables.StaticContentLibrary + "';\nvar theme='" + this.Theme + "';\nvar module='" + this.CurrentUser.CurrentActiveModule + "';\nvar moduleNameHTML='" + module.BrandNameHTML + "';\nvar moduleNamePlain='" + module.BrandNamePlainText + "';" + currentUserInfo + "\n</script>";
            }
        }

        public string SetupGlobalMasterPopup(ref ModalPopupExtender mdlMasterPopup)
        {
            StringBuilder MasterPopupInfo = new StringBuilder();

            MasterPopupInfo.AppendLine("\n");
            MasterPopupInfo.AppendLine("MasterPopupInfo = function () {");
            MasterPopupInfo.AppendLine("SEL.MasterPopup.ModalDOMID = '" + mdlMasterPopup.ClientID + "'");
            MasterPopupInfo.AppendLine("};");

            MasterPopupInfo.AppendLine("MasterPopupInfo.registerClass(\"MasterPopupInfo\", Sys.Component);");
            MasterPopupInfo.AppendLine("Sys.Application.add_init(");
            MasterPopupInfo.AppendLine("\tfunction(){");
            MasterPopupInfo.AppendLine("\t\t$create(MasterPopupInfo, {\"id\":\"MasterPopupInfoVariables\"}, null, null, null)");
            MasterPopupInfo.AppendLine("\t}");
            MasterPopupInfo.AppendLine(");");

            return "<script language=\"javascript\" type=\"text/javascript\">\n" + MasterPopupInfo.ToString() + "</script>";
        }

        public void GenerateWindowOnError(ref Literal windowOnError)
        {
            StringBuilder windowOnErrorScript = new StringBuilder();
            windowOnErrorScript.AppendLine("var onErrorMethod = function(message, url, linenumber)");
            windowOnErrorScript.AppendLine("{");
            //windowOnErrorScript.AppendLine("alert(message + '\\n' + url + '\\n' + linenumber);");
            //windowOnErrorScript.AppendLine("SEL.Errors.ReportClientError(message, url, linenumber); ");
            windowOnErrorScript.AppendLine("}");
            windowOnErrorScript.AppendLine("window.onerror = onErrorMethod;");

            //windowOnErrorScript.AppendLine("onErrorAttach = function() {");
            //windowOnErrorScript.AppendLine("alert('adding onerror event')");
            //windowOnErrorScript.AppendLine("window.onerror = SEL.Errors.ReportClientError;");
            //windowOnErrorScript.AppendLine("}");

            //windowOnErrorScript.Append("onErrorAttach.registerClass(\"onErrorAttach\", Sys.Component);\n");

            //windowOnErrorScript.Append("Sys.Application.add_init(\n");
            //windowOnErrorScript.Append("\tfunction(){\n");
            //windowOnErrorScript.Append("\t\t$create(onErrorAttach, {\"id\":\"WindowOnError\"}, null, null, null)\n");
            //windowOnErrorScript.Append("\t}\n");
            //windowOnErrorScript.Append(");\n");

            windowOnError.Text = string.Format("<script language=\"javascript\" type=\"text/javascript\">{0}</script>", windowOnErrorScript);
        }

        public void SetupJQueryReferences(ref HtmlLink jQueryCss, ref ToolkitScriptManager toolkitScrMgr)
        {
            // set jQuery css path to staticContent location
            jQueryCss.Href = this.SetjQueryCss();
            toolkitScrMgr.Scripts.Insert(0, new ScriptReference("jQuery", string.Empty));
        }

        /// <summary>
        /// Configures the current master page to be able to make calls to the public api with a valid JWT token.
        /// </summary>
        /// <param name="toolkitScrMgr">The script manager to add the scripts to.</param>
        /// <returns>A block of JavaScript to include on the page.</returns>
        public string SetupPublicApi(ref ToolkitScriptManager toolkitScrMgr)
        {
            toolkitScrMgr.Scripts.Insert(toolkitScrMgr.Scripts.Count, new ScriptReference("/shared/javaScript/sel.public.api.js"));
            
            string token = "";
            
            var client = new RestClient(ConfigurationManager.AppSettings["PublicApiUrl"]);
            RestRequest request = new RestRequest("/Token", Method.POST) { RequestFormat = DataFormat.Json};
            var reqProperties = new cAccountSubAccounts(CurrentUser.AccountID).getFirstSubAccount().SubAccountProperties;
            var idleTimeout = reqProperties.IdleTimeout == 0 ? 5 : reqProperties.IdleTimeout * 60;
            string json = $"\"accountId\":\"{this.CurrentUser.AccountID}\", \"employeeId\":\"{this.CurrentUser.EmployeeID}\", \"TimeoutMinutes\":\"{idleTimeout}\"";

            if (this.CurrentUser.isDelegate)
            {
                json += $", \"delegateId\":\"{this.CurrentUser.Delegate.EmployeeID}\"";
            }

            request.AddParameter("application/json; charset=utf-8", "{"+ json + "}", ParameterType.RequestBody);
            
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                token = response.Content.Replace("\"", string.Empty);
            }
            
            string url = ConfigurationManager.AppSettings["PublicApiUrl"];
            return "<script type='text/javascript' language='javascript'>var publicApiUrl = '" + url + "'; var publicApiToken = '" + token + "';</script>";
        }

        /// <summary>
        /// Sets up the javascript required to allow the popup window before the session times out (to inform user)
        /// </summary>
        /// <param name="toolkitScriptManager"></param>
        /// <param name="page">The page control</param>
        public void SetupSessionTimeoutReferences(ref ToolkitScriptManager toolkitScriptManager, Control page)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var accountSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties subAccountProperties = accountSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties.Clone();           
            toolkitScriptManager.Scripts.Insert(2, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.idletimer.js"));
            toolkitScriptManager.Scripts.Insert(3, new ScriptReference("idletimeout", string.Empty));

            var sb = new StringBuilder();
            sb.AppendLine("jQuery(document).ready(function () ");
            sb.AppendLine("{");
            sb.AppendLine("jQuery('<div/>', {");
            sb.AppendLine("id: 'sessionTimeoutWarning',");
            sb.AppendLine("style: 'display: none'");
            sb.AppendLine("}).appendTo(jQuery('body'));");
            sb.AppendLine("jQuery.idleTimeout('#sessionTimeoutWarning', {");
            var idleTimeout = subAccountProperties.IdleTimeout == 0 ? 5 : subAccountProperties.IdleTimeout * 60;
            sb.AppendLine("idleAfter: " + idleTimeout + ",");
            sb.AppendLine("warningLength: " + subAccountProperties.CountdownTimer + ",");
            sb.AppendLine("pollingInterval: 900,");
            sb.AppendLine("keepAliveURL: window.appPath + '/shared/webServices/SvcSession.asmx/',");
            sb.AppendLine("killSessionURL: window.appPath + '/shared/webServices/SvcSession.asmx/',");
            sb.AppendLine("serverResponseEquals: 'OK',");
            sb.AppendLine("moduleName: moduleNameHTML");
            sb.AppendLine("});");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(page, this.GetType(), "sessionManagerVariables", sb.ToString(), true);
            toolkitScriptManager.Scripts.Insert(4, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.pulse.min.js"));


        }

        public void SetupSessionTimeoutReferencesNew(ref ToolkitScriptManager toolkitScriptManager, Control page)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var accountSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties subAccountProperties = accountSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties.Clone();
            toolkitScriptManager.Scripts.Insert(0, new ScriptReference("jQuery", string.Empty));
            toolkitScriptManager.Scripts.Insert(2, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.idletimer.js"));
            toolkitScriptManager.Scripts.Insert(3, new ScriptReference("idletimeout", string.Empty));

            var sb = new StringBuilder();
            sb.AppendLine("jQuery(document).ready(function () ");
            sb.AppendLine("{");
            sb.AppendLine("jQuery('<div/>', {");
            sb.AppendLine("id: 'sessionTimeoutWarning',");
            sb.AppendLine("style: 'display: none'");
            sb.AppendLine("}).appendTo(jQuery('body'));");
            sb.AppendLine("jQuery.idleTimeout('#sessionTimeoutWarning', {");
            var idleTimeout = subAccountProperties.IdleTimeout == 0 ? 5 : subAccountProperties.IdleTimeout * 60;
            sb.AppendLine("idleAfter: " + idleTimeout + ",");
            sb.AppendLine("warningLength: " + subAccountProperties.CountdownTimer + ",");
            sb.AppendLine("pollingInterval: 900,");
            sb.AppendLine("keepAliveURL: window.appPath + '/shared/webServices/SvcSession.asmx/',");
            sb.AppendLine("killSessionURL: window.appPath + '/shared/webServices/SvcSession.asmx/',");
            sb.AppendLine("serverResponseEquals: 'OK',");
            sb.AppendLine("moduleName: moduleNameHTML");
            sb.AppendLine("});");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(page, this.GetType(), "sessionManagerVariables", sb.ToString(), true);
            toolkitScriptManager.Scripts.Insert(4, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.pulse.min.js"));


        }

        public void SetupDynamicStyles(ref HtmlHead headElement)
        {
            if (UseDynamicCSS)
            {
                headElement.Controls.Add(this.LayoutCSS());
                headElement.Controls.Add(this.ColoursCSS());
            }
        }

        /// <summary>
        /// The get onload script for broadcast messages.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string GetOnloadScriptForBroadcastMessages(string page)
        {
            broadcastLocation location;
            switch (page)
            {
                case "home.aspx":
                    location = broadcastLocation.HomePage;
                    break;
                case "submitclaim.aspx":
                    location = broadcastLocation.SubmitClaim;
                    break;
                default:
                    return string.Empty;
            }

            string includeNotes = location == broadcastLocation.HomePage ? "true" : "false";
            return string.Format("SEL.Common.GetBroadcastMessages({0}, {1}, '{2}');", 2000, includeNotes, page.ToLower());
        }

        /// <summary>
        /// The add broadcasr message plugin.
        /// </summary>
        /// <param name="headElement">
        /// The head element.
        /// </param>
        /// <param name="toolkitScrMgr">
        /// The toolkit scr mgr.
        /// </param>
        public static void AddBroadcastMessagePlugin(ref HtmlHead headElement, ref ToolkitScriptManager toolkitScrMgr)
        {
            if (headElement.Controls.Contains(headElement.FindControl("broadcastcss")) == false)
            {
                var layout = new HtmlLink
                {
                    ID = "broadcastcss",
                    Href = GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.pnotify.default.css"
                };
                layout.Attributes.Add("rel", "stylesheet");
                layout.Attributes.Add("type", "text/css");
                layout.Attributes.Add("media", "screen");
                headElement.Controls.Add(layout);
            }
            toolkitScrMgr.Scripts.Insert(1, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.pnotify.min.js"));
        }


        /// <summary>
        /// Adds the necessary scripts requried for the jQuery Colorbox addon
        /// http://www.jacklmoore.com/colorbox
        /// </summary>
        /// <param name="headElement">
        /// The head element.
        /// </param>
        /// <param name="toolkitScriptManager">
        /// The toolkit script manager.
        /// </param>
        public static void AddJQueryColorboxPlugin(ref HtmlHead headElement, ref ToolkitScriptManager toolkitScriptManager)
        {
            var layout = new HtmlLink
            {
                ID = "receiptscss",
                Href = GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.colorbox.css"
            };
            layout.Attributes.Add("rel", "stylesheet");
            layout.Attributes.Add("type", "text/css");
            layout.Attributes.Add("media", "screen");
            headElement.Controls.Add(layout);

            toolkitScriptManager.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.colorbox.min.js"));
        }


        public string Theme { get; set; }

        /// <summary>
        /// This method checks if the user needs to be redirected to the change password or odometer readings page.
        /// </summary>
        public void RedirectUserBypassingChangePassword()
        {
            string absoluteUri;

            RedirectToChangePasswordPageOnChangePasswordBypass(out absoluteUri);

            if (this.CurrentUser.CurrentActiveModule == Modules.expenses)
            {
                RedirectToOdometerReadingsOnChangePasswordBypass(absoluteUri);
            }
        }

        /// <summary>
        /// Redirects the user to the change password page if the session value is set.
        /// </summary>
        /// <param name="absoluteUri">The uri string gets set during the process of this method.</param>
        private static void RedirectToChangePasswordPageOnChangePasswordBypass(out string absoluteUri)
        {
            absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();

            if (!absoluteUri.Contains(".aspx") || absoluteUri.Contains("/styles.aspx") || absoluteUri.Contains("/process.aspx"))
            {
                return;
            }

            if (absoluteUri.Contains("/changepassword.aspx")
                || HttpContext.Current.Session["ChangePasswordUserId"] == null)
            {
                return;
            }

            var url = string.Format(
                "~/shared/changepassword.aspx?returnto=3&employeeid={0}",
                HttpContext.Current.Session["ChangePasswordUserId"]);
                HttpContext.Current.Response.Redirect(url, true);
        }

        /// <summary>
        /// If we would like to redirect to the odometer readings page instead of the change password page, we use this sub method
        /// </summary>
        /// <param name="absoluteUri">the uri</param>
        private static void RedirectToOdometerReadingsOnChangePasswordBypass(string absoluteUri)
        {
            if (!absoluteUri.Contains("/odometerreading.aspx") && !absoluteUri.Contains("/logon.aspx")
                && HttpContext.Current.Session["OdometerReadingsOnLogon"] != null)
            {
                HttpContext.Current.Response.Redirect("~/odometerreading.aspx?odotype=1", true);
            }
        }

        /// <summary>
        /// Adds a CSS class to a HTML body element to hide the navigation (sidebar) menu
        /// </summary>
        /// <param name="body">The page's <see cref="HtmlGenericControl">body element</see></param>
        public void HideNavigation(ref HtmlGenericControl body)
        {
            body.Attributes["class"] += " hide-nav";
        }
        
        /// <summary>
        /// This method checks if the menu has a View under menu tree
        /// </summary>
        /// <param name="user"></param>
        /// <param name="menuId"></param>
        /// <returns>
        /// if Menu has View
        /// </returns>
        public bool CheckForAnyViewInMenuTree(CurrentUser user, int menuId)
        {
            if (!hasView)
            {
                var customMenu = new CustomMenuStructure(user.AccountID);
                List<CustomMenuStructureItem> menuItems = customMenu.GetCustomMenusByParentId(menuId);
                menuItems = menuItems.Where(x => x.SystemMenu == false).ToList();
                var entities = new cCustomEntities(user);
                List<cCustomEntityView> menuViews = entities.getViewsByMenuId(menuId);
                var disabledModuleMenuViews = new DisabledModuleMenuViews(user.AccountID, (int)user.CurrentActiveModule);
                foreach (cCustomEntityView view in menuViews)
                {

                    if (!disabledModuleMenuViews.IsViewDisabled(menuId, view.viewid) && user.CheckAccessRole(
                    AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false))
                    {
                        return hasView = true;
                    }
                }

                if (menuItems.ToList().Count <= 0)
                {
                    return hasView = false;
                }
                foreach (CustomMenuStructureItem menuItem in menuItems)
                {
                    this.CheckForAnyViewInMenuTree(user, menuItem.CustomMenuId);
                }
            }
            return hasView;
        }

        /// <summary>
        /// The Prevent browser from caching the response so that user cannot navigate back to a page after they have been logged out
        /// </summary>
        public void PreventBrowserFromCachingTheResponse()
        {
            HttpContext.Current.Response.Headers.Remove("Cache-Control");
            HttpContext.Current.Response.Headers.Remove("Pragma");
            HttpContext.Current.Response.Headers.Remove("Expires");
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
        }
    }
}
