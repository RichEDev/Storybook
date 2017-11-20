namespace Spend_Management.shared
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Xml;

    using SpendManagementLibrary;

    /// <summary>
    /// Shows indicators for the product integrity
    /// </summary>
    public partial class ProductIntegrity : Page
    {
        /// <summary>
        /// The background-color for good.
        /// </summary>
        private const string Good = "#BDFEA7";

        /// <summary>
        /// The background-color for Bad
        /// </summary>
        private const string Bad = "#FF3C41";

        /// <summary>
        /// The background-color for Warning
        /// </summary>
        private const string Warning = "#FF9900";

        /// <summary>
        /// The text colour for Good
        /// </summary>
        private const string GoodFontColor = "#000000";

        /// <summary>
        /// The text colour for Bad
        /// </summary>
        private const string BadFontColor = "#FFFFFF";

        /// <summary>
        /// The text colour for Warning
        /// </summary>
        private const string WarningFontColor = "#FFFFFF";

        protected void Page_Load(object sender, EventArgs e)
        {
            XmlDocument webConfig = new XmlDocument();
            webConfig.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config"));
            XmlNode assemblyBlock = webConfig.SelectSingleNode("configuration/system.web/compilation/assemblies");

            SortedList<string, string> requiredAssemblies = null;

            if (assemblyBlock != null)
            {
                requiredAssemblies = new SortedList<string, string>();

                foreach (XmlNode assemblyNode in assemblyBlock.ChildNodes)
                {
                    if (assemblyNode.Name == "add" && assemblyNode.Attributes != null && assemblyNode.Attributes["assembly"] != null)
                    {
                        string[] assemblyParts = assemblyNode.Attributes["assembly"].Value.Split(new[] { ',' });
                        string name = assemblyParts[0];
                        string fullInformation = assemblyNode.Attributes["assembly"].Value;
                        requiredAssemblies.Add(name, fullInformation);
                    }
                }
            }

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.Employee.AdminOverride == false)
            {
                Response.Redirect(ErrorHandlerWeb.InsufficientAccess);
            }

            string expectedVersion = string.Empty;

            if (string.IsNullOrWhiteSpace(Request.QueryString["version"]) == false)
            {
                expectedVersion = Request.QueryString["version"];
            }

            // Show the address of the current server beign used to present this page.
            this.serverInformation.Text = Request.ServerVariables["LOCAL_ADDR"] + "(" + Environment.MachineName + ")";

            StringBuilder sb = new StringBuilder();
            
            // Show all the accounts on this web server and the state of the SqlDependency object on them.
            foreach (cAccount account in cAccounts.CachedAccounts.Values)
            {
                if (account.archived == false)
                {
                    sb.AppendLine("<tr><td" + this.GetStyle(account.SqlDependencyStarted ? Good : Bad, true, account.SqlDependencyStarted ? GoodFontColor : BadFontColor) + ">" + account.companyid + "</td></tr>");
                }
            }

            this.sqlDependencies.Text = "<fieldset><legend><h3>SqlDependencies</h3></legend>";
            this.sqlDependencies.Text += "<p>A list of all instances of cAccount loaded on this server and their status with SqlDependency.Start.</p>";
            this.sqlDependencies.Text += "<div style=\"background-color: " + Good + "; color: " + GoodFontColor + "; line-height: 25px; width: 600px; padding: 4px;\">Indicates SqlDependency.Start has been called successfully</div>";
            this.sqlDependencies.Text += "<div style=\"background-color: " + Bad + "; " + BadFontColor + "; line-height: 25px; width: 600px; padding: 4px;\">Indicates SqlDependency.Start has not been called or failed to run successfully</div>";
            this.sqlDependencies.Text += "<table cellpadding=\"5\" cellspacing=\"4\" width=\"600px\"><tr><th>Account (companyid)</th></tr>\n" + sb + "\n</table></fieldset>";
            sb = new StringBuilder();

            // Show all of the required assemblies referenced in the web.config and compare to what is actually present in the bin folder.
            if (requiredAssemblies != null)
            {
                SortedList<string, string> presentAssemblies = SpendManagementLibrary.ProductIntegrity.GetAssemblyVersions();

                // Show our own assemblies at the top of the list
                sb.AppendLine(this.CheckOurAssembly("expenses", expectedVersion, ref presentAssemblies));
                sb.AppendLine(this.CheckOurAssembly("framework", expectedVersion, ref presentAssemblies));
                sb.AppendLine(this.CheckOurAssembly("Spend Management", expectedVersion, ref presentAssemblies));
                sb.AppendLine(this.CheckOurAssembly("SpendManagementHelpers", expectedVersion, ref presentAssemblies));
                sb.AppendLine(this.CheckOurAssembly("SpendManagementLibrary", expectedVersion, ref presentAssemblies));

                // Loop through all the assemblies referenced in the web.config
                foreach (KeyValuePair<string, string> keyValuePair in requiredAssemblies)
                {
                    string background;

                    if (presentAssemblies.ContainsKey(keyValuePair.Key) && presentAssemblies[keyValuePair.Key].ToLower() == keyValuePair.Value.ToLower())
                    {
                        // Expected reference is located in the bin folder
                        background = this.GetStyle(Good, false, GoodFontColor);
                        presentAssemblies.Remove(keyValuePair.Key);
                    }
                    else if (presentAssemblies.ContainsKey(keyValuePair.Key) && presentAssemblies[keyValuePair.Key] != keyValuePair.Value)
                    {
                        // Expected reference is the incorrect version in the bin folder or incorrectly referenced.
                        background = this.GetStyle(Bad, false, BadFontColor);
                        presentAssemblies.Remove(keyValuePair.Key);
                    }
                    else
                    {
                        // Expected reference is not located in the bin folder
                        background = this.GetStyle(Bad, false, BadFontColor);
                    }

                    sb.AppendLine("<tr><td" + background + ">" + keyValuePair.Value + "</td></tr>");
                }

                // loop through all remaining assemblies in the bin folder and mark them as unknown/warning since they are not in the web.config
                foreach (string assembly in presentAssemblies.Values)
                {
                    sb.AppendLine("<tr><td" + this.GetStyle(Warning, false, WarningFontColor) + ">" + assembly + "</td></tr>");
                }

                this.assembliesSummary.Text = "<fieldset><legend><h3>Assemblies</h3></legend>";
                this.assembliesSummary.Text += "<p>Indicates the status of references for the website.</p>";
                this.assembliesSummary.Text += "<div style=\"background-color: " + Good + "; color: " + GoodFontColor + "; line-height: 25px; width: 600px; padding: 4px;\">Indicates the reference in the web.config is present.</div>";
                this.assembliesSummary.Text += "<div style=\"background-color: " + Bad + "; color: " + BadFontColor + "; line-height: 25px; width: 600px; padding: 4px;\">Indicates the reference in the web.config is not present.</div>";
                this.assembliesSummary.Text += "<div style=\"background-color: " + Warning + "; color: " + WarningFontColor + "; line-height: 25px; width: 600px; padding: 4px;\">Indicates the reference is not present in the web.config.</div>";
                this.assembliesSummary.Text += "<table cellpadding=\"5\" cellspacing=\"4\"><tr><th>Assembly</th></th>\n" + sb + "</table></fieldset>";
            }
        }

        private string CheckOurAssembly(string name, string expectedVersion, ref SortedList<string, string> presentAssemblies)
        {
            string line = string.Empty;

            if (presentAssemblies.ContainsKey(name))
            {
                expectedVersion = string.Format("{0}, Version={1}, Culture=neutral, PublicKeyToken=null", name, expectedVersion);
                line = "<tr><td" + this.GetStyle(expectedVersion == presentAssemblies[name] ? Good : Bad, false, expectedVersion == presentAssemblies[name] ? GoodFontColor : BadFontColor) + ">" + presentAssemblies[name] + "</td></tr>";
                presentAssemblies.Remove(name);
            }

            return line;
        }

        private string GetStyle(string type, bool bold, string fontColor)
        {
            string style = " style=\"";

            style += "background-color: " + type + ";";

            if (string.IsNullOrWhiteSpace(fontColor) == false)
            {
                style += "color: " + fontColor + ";";
            }

            if (bold)
            {
                style += "font-weight: bold;";
            }

            style += "\"";

            return style;
        }
    }
}