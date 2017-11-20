using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using SpendManagementLibrary;

namespace Spend_Management
{
    public class cMasterMenu
    {
        /* extracted from smMenu.master */
        private List<cMenuItem> lstMenuitems = new List<cMenuItem>();
        private string sMenuTitle;

        /// <summary>
        /// For smMenu.master
        /// </summary>
        /// <param name="menuTitle"></param>
        public cMasterMenu(string menuTitle)
        {
            sMenuTitle = menuTitle;
        }

        /// <summary>
        /// Constructor for pages with title set elsewhere, eg. smMenuForm.master
        /// </summary>
        public cMasterMenu() { }

        /// <summary>
        /// Add a menu option with gif logo
        /// </summary>
        /// <param name="logo">Just the name portion of the logo without any extension</param>
        /// <param name="size">Usually 48</param>
        /// <param name="label">Title for the menu option</param>
        /// <param name="description">Description for the menu option</param>
        /// <param name="url">Url to navigate to</param>
        public void AddMenuItem(string logo, int size, string label, string description, string url)
        {
            cMenuItem item = new cMenuItem(logo, size, label, description, url);
            lstMenuitems.Add(item);
        }

        /// <summary>
        /// Add a menu option with a non-gif logo image extension
        /// </summary>
        /// <param name="logo"></param>
        /// <param name="size"></param>
        /// <param name="label"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <param name="logoExt"></param>
        public void AddMenuItem(string logo, int size, string label, string description, string url, string logoExt)
        {
            cMenuItem item = new cMenuItem(logo, size, label, description, url, logoExt);
            lstMenuitems.Add(item);
        }

        /// <summary>
        /// Add a menu option to open in a target window
        /// </summary>
        /// <param name="logo"></param>
        /// <param name="size"></param>
        /// <param name="label"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="newWindow"></param>
        public void AddMenuItem(string logo, int size, string label, string description, string url, string target, bool newWindow)
        {
            cMenuItem item = new cMenuItem(logo, size, label, description, url, target, newWindow);
            lstMenuitems.Add(item);
        }

        /// <summary>
        /// Add a menu option to open in a target window with a non-gif logo extension
        /// </summary>
        /// <param name="logo"></param>
        /// <param name="size"></param>
        /// <param name="label"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="newWindow"></param>
        public void AddMenuItem(string logo, int size, string label, string description, string url, string target, bool newWindow, string logoExt)
        {
            cMenuItem item = new cMenuItem(logo, size, label, description, url, target, newWindow, logoExt);
            lstMenuitems.Add(item);
        }

        /// <summary>
        /// Build a master menu with div positioning
        /// </summary>
        /// <returns>smMenuForm Menu in div layout</returns>
        public string CreateMenuHTML(int? columns)
        {
            StringBuilder output = new StringBuilder();

            output.Append("<div class=\"mastermenu\">");
            foreach(cMenuItem item in lstMenuitems)
            {
                output.Append(CreateMenuItemHTML(item, columns));
            }
            output.Append("</div>");
            return output.ToString();
        }
        /// <summary>
        /// Create the individual menu item's HTML
        /// </summary>
        /// <param name="menuItem">the cMenuItem to process</param>
        /// <returns>Master Menu Item HTML</returns>
        private string CreateMenuItemHTML(cMenuItem menuItem, int? columns)
        {
            StringBuilder html = new StringBuilder();
            string logoPlainPath = cMisc.Path + "/shared/images/icons/" + menuItem.logosize + "/plain/" + menuItem.logo + "." + menuItem.LogoExt;
            string logoShadowPath = cMisc.Path + "/shared/images/icons/" + menuItem.logosize + "/shadow/" + menuItem.logo + "." + menuItem.LogoExt;
            string width = (columns.HasValue) ? " style=\"width: " + (100 / columns).ToString() + "%;\"" : string.Empty;

            html.Append("<span class=\"mastermenuitemspan\" onclick=\"");
            if (menuItem.url.Length >= 10 && menuItem.url.Substring(0, 10).ToLower() == "showpolicy")
            {
                html.Append("MasterMenuItemShowPolicy('./policy.aspx');");
            }
            else if (menuItem.target != null)
            {
                html.Append("window.open('" + menuItem.resolvedUrl + "');");
            }
            else
            {
                html.Append("document.location='" + menuItem.resolvedUrl + "';");
            }
            html.Append("\" onmouseover=\"MasterMenuItemOver(this, '" + logoShadowPath + "');\" onmouseout=\"MasterMenuItemOut(this, '" + logoPlainPath + "');\"" + width + ">");
            html.Append("<div class=\"mastermenuitem\">");
            html.Append("<img class=\"mastermenuitemlogo\" style=\"height: " + menuItem.logosize + "px; width: " + menuItem.logosize + "px;\" src=\"" + logoPlainPath + "\">");
            html.Append("<span class=\"mastermenuitemlabel\">" + menuItem.label + "</span>");
            html.Append("<hr class=\"mastermenuitemline\">");
            html.Append("<span class=\"mastermenuitemdescription\">" + menuItem.description + "</span>");
            html.Append("</div>");
            html.Append("</span>");

            return html.ToString();
        }


        /// <summary>
        /// Build the normal master page menu for menu.master or smMenu.master
        /// </summary>
        /// <returns>Page title div followed by menu in tabular layout</returns>
        public string CreateMenu()
        {
            cMenuItem menuitem;

            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<div class=menutitle>");
            output.Append(sMenuTitle);
            output.Append("</div>");

            int i = 0;
            int height;
            decimal numitems;
            bool even = false;

            numitems = decimal.Parse(lstMenuitems.Count.ToString());
            if (numitems == 0)
            {
                return "";
            }
            menuitem = (cMenuItem)lstMenuitems[0];
            height = (menuitem.logosize + 20) * (int)Math.Floor((numitems / (decimal)2.0));

            output.Append("<div class=menu style=\"height: " + height + "px;\">");
            for (i = 0; i < lstMenuitems.Count; i++)
            {
                if ((double)((double)i / 2) == (double)Math.Floor((double)i / 2.0))
                {
                    even = true;
                }
                else
                {
                    even = false;
                }
                menuitem = (cMenuItem)lstMenuitems[i];

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
                            output.Append("window.open('" + menuitem.resolvedUrl + "');");
                        }
                        else
                        {
                            output.Append("document.location='" + menuitem.resolvedUrl + "';");
                        }
                    }
                }
                else
                {
                    if (menuitem.target != null)
                    {
                        output.Append("window.open('" + menuitem.resolvedUrl + "');");
                    }
                    else
                    {
                        output.Append("document.location='" + menuitem.resolvedUrl + "';");
                    }
                }
                output.Append("\" border=0 width=100% onmouseover=\"menuItemOver(" + i + ");changeIcon('" + menuitem.logo + "','" + cMisc.Path + "/shared/images/icons/" + menuitem.logosize + "/shadow/" + menuitem.logo + "." + menuitem.LogoExt + "');\" onmouseout=\"menuItemOut(" + i + ");changeIcon('" + menuitem.logo + "','" + cMisc.Path + "/shared/images/icons/" + menuitem.logosize + "/plain/" + menuitem.logo + "." + menuitem.LogoExt + "');\">");
                output.Append("<tr>");
                output.Append("<td style=\"vertical-align:top;width:58px;text-align:left;\">");

                output.Append("<img id=\"icon" + menuitem.logo + "\" class=\"menuitemlogo\" src=\"" + cMisc.Path + "/shared/images/icons/" + menuitem.logosize + "/plain/" + menuitem.logo + "." + menuitem.LogoExt + "\" border=0>");
                output.Append("</td>");

                output.Append("<td>");

                output.Append("<span id=\"menuitemlabel" + i + "\" class=\"menuitemtitle\">" + menuitem.label + "</span>");
                output.Append("<hr id=\"menuitemline" + i + "\" style=\"color: #ccc; line-height: 3px; border-width: 1px; height: 1px;font-size:9px;\">");
                output.Append("<span class=menuitemdescription>");
                output.Append(menuitem.description);
                output.Append("</span>");
                output.Append("</td>");
                output.Append("</tr>");
                output.Append("</table>");
                output.Append("</td>");
                if (even == false || i == (lstMenuitems.Count - 1))
                {
                    if ((lstMenuitems.Count - 1) == i && even == true)
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
}
