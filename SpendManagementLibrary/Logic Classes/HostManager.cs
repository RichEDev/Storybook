namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using SpendManagementLibrary.Definitions;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Hostnames logic class
    /// </summary>
    public static class HostManager
    {
        /// <summary>
        /// Initialises static members of the <see cref="HostManager"/> class.
        /// </summary>
        static HostManager()
        {
            GlobalHostInformation = new ConcurrentDictionary<int, Host>();
        }

        /// <summary>
        /// Gets or sets the host information to be used instead of cache throughout the application.
        /// </summary>
        public static ConcurrentDictionary<int, Host> GlobalHostInformation { get; set; }

        /// <summary>
        /// Clears the existing list of hostnames and repopulates it from the database.
        /// </summary>
        public static void ResetHostInformation()
        {
            GlobalHostInformation = new ConcurrentDictionary<int, Host>();
            SetHostInformation();
        }

        /// <summary>
        /// Sets the host information into a global static list called GlobalHostInformation.
        /// </summary>
        public static void SetHostInformation()
        {
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                const string CachingSQL = "SELECT h.hostnameID, h.hostname, h.moduleID, t.Theme FROM dbo.hostnames h inner join dbo.moduleBase m ON h.moduleId = m.ModuleId inner join dbo.Theme t on t.ThemeId = m.ThemeId";

                using (IDataReader reader = connection.GetReader(CachingSQL))
                {
                    var ordinalHostnameId = reader.GetOrdinal("hostnameID");
                    var ordinalHostname = reader.GetOrdinal("hostname");
                    var ordinalModuleId = reader.GetOrdinal("moduleID");
                    var ordinalTheme = reader.GetOrdinal("Theme");

                    while (reader.Read())
                    {
                        var hostNameId = reader.GetInt32(ordinalHostnameId);
                        var hostName = reader.GetString(ordinalHostname);
                        var moduleId = reader.GetInt32(ordinalModuleId);
                        var theme = reader.GetString(ordinalTheme);

                        GlobalHostInformation.GetOrAdd(hostNameId, new Host(hostNameId, hostName, moduleId, theme));
                    }

                    connection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Get a Hostname object by its description
        /// </summary>
        /// <param name="hostnameDescription">The string representation of the hostname</param>
        /// <returns>The desired Hostname object</returns>
        public static Host GetHost(string hostnameDescription)
        {
            Host host = GlobalHostInformation.Values.FirstOrDefault(x => x.HostnameDescription == hostnameDescription)
                        ?? GlobalHostInformation.Values.Where(x => hostnameDescription.Contains(RemoveHostnameCompanyIdPlaceHolder(x.HostnameDescription))).OrderByDescending(x => x.HostnameDescription).FirstOrDefault();

            return host;
        }

        /// <summary>
        /// Get a Hostname object by module
        /// </summary>
        /// <param name="accountHostIds">
        /// The account Host Ids.
        /// </param>
        /// <param name="module">
        /// The module enumerator.
        /// </param>
        /// <returns>
        /// The desired Hostname object
        /// </returns>
        public static Host GetHost(List<int> accountHostIds, Modules module)
        {
            return (from hostId in accountHostIds select GetHost(hostId) into host where host != null let moduleId = Convert.ToInt32(module) where host.ModuleId == moduleId select host).FirstOrDefault();
        }

        /// <summary>
        /// Gets host description information via a host id.
        /// </summary>
        /// <param name="hostId">
        /// The host id.
        /// </param>
        /// <returns>
        /// The host information class.
        /// </returns>
        public static Host GetHost(int hostId)
        {
            return GlobalHostInformation.ContainsKey(hostId) ? GlobalHostInformation[hostId] : default(Host);
        }

        /// <summary>
        /// Returns a list of hostnames for the given host id.
        /// </summary>
        /// <param name="currentModule">
        /// The current module.
        /// </param>
        /// <returns>
        /// A list of hostnames for the given module
        /// </returns>
        public static List<Host> GetHosts(Modules currentModule)
        {
            int moduleId = Convert.ToInt32(currentModule);

            return GlobalHostInformation.Values.Where(hostname => hostname.ModuleId == moduleId).ToList();
        }

        /// <summary>
        /// Gets the host name descript for an account. It will substitute the {companyId} place holder for the company id in the process.
        /// </summary>
        /// <param name="accountHostIds">
        /// The account host ids.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <param name="companyId">
        /// The company id (name).
        /// </param>
        /// <returns>
        /// The host name description.
        /// </returns>
        public static string GetHostName(List<int> accountHostIds, Modules module, string companyId)
        {
            string hostName = string.Empty;
            Host host = GetHost(accountHostIds, module);

            if (host != null)
            {
                hostName = SubstituteHostnameCompanyIdPlaceHolder(host.HostnameDescription, companyId);
            }

            return hostName;
        }

        /// <summary>
        /// Returns the module for the supplied host name.
        /// </summary>
        /// <param name="hostName">The host name.</param>
        /// <returns>The Modules enumeration for the specified host name</returns>
        public static Modules GetModule(string hostName)
        {
            var moduleId = Modules.SpendManagement;

            try
            {
                if (!string.IsNullOrEmpty(hostName))
                {
                    Host currentHostName = GetHost(hostName);

                    if (Enum.IsDefined(typeof(Modules), currentHostName.ModuleId))
                    {
                        moduleId = (Modules)currentHostName.ModuleId;
                    }
                    else
                    {
                        throw new InvalidCastException("Not a valid module id");
                    }
                }
            }
            catch (Exception)
            {
                moduleId = Modules.SpendManagement;
            }

            return moduleId;
        }

        /// <summary>
        /// Returns the hostname description with the {company} tag replaced with the company id name.
        /// </summary>
        /// <param name="hostName">The host name description with possible placeholder tag.</param>
        /// <param name="companyid">The company id string.</param>
        /// <returns>The legitimate host name.</returns>
        public static string SubstituteHostnameCompanyIdPlaceHolder(string hostName, string companyid)
        {
            return hostName.Replace("{companyID}", companyid).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Removes the {companyID}. placeholder from the supplied host name.
        /// </summary>
        /// <param name="hostName">
        /// The host name.
        /// </param>
        /// <returns>
        /// The hostname minus possible placeholder.
        /// </returns>
        public static string RemoveHostnameCompanyIdPlaceHolder(string hostName)
        {
            return hostName.Replace("{companyID}.", string.Empty).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Gets the style sheet theme for the given module.
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <returns>The style sheet theme name.</returns>
        public static string GetTheme(Modules moduleId)
        {
            int module = Convert.ToInt32(moduleId);
            var info = GlobalHostInformation.Values.FirstOrDefault(x => x.ModuleId == module);
            return info == null ? "ExpensesThemeNew" : info.Theme;
        }

        /// <summary>
        /// Returns the theme for the specified hostname.
        /// </summary>
        /// <param name="hostName">The host name.</param>
        /// <param name="defaultTheme">The default theme to use (expenses/framework) if no results found.</param>
        /// <returns>The associated theme for the given hostname.</returns>
        public static string GetTheme(string hostName, string defaultTheme = "ExpensesThemeNew")
        {
            var themeObject = GlobalHostInformation.FirstOrDefault(x => RemoveHostnameCompanyIdPlaceHolder(x.Value.HostnameDescription).Contains(hostName)).Value;
            return themeObject == null ? defaultTheme : themeObject.Theme;
        }

        /// <summary>
        /// Gets the main address.
        /// </summary>
        /// <param name="absoluteUri">
        /// The absolute uri.
        /// </param>
        /// <param name="absolutePath">
        /// The absolute path.
        /// </param>
        /// <param name="queryString">
        /// The query String.
        /// </param>
        /// <returns>
        /// The formatted current address.
        /// </returns>
        public static string GetFormattedAddress(string absoluteUri, string absolutePath, string queryString)
        {
            string address = absoluteUri.Replace(absolutePath, string.Empty).Replace("https://", string.Empty).Replace("http://", string.Empty);
            if (queryString.Length > 0)
            {
                address = address.Replace(queryString, string.Empty);
            }

            return address;
        }

        /// <summary>
        /// Validates the supplied host address against the list of Host Ids supplied.
        /// </summary>
        /// <param name="accountHostIds">
        /// The account Host Ids.
        /// </param>
        /// <param name="currentAddress">
        /// The current address.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// Returns true if any of the host objects have an address that matches .
        /// </returns>
        public static bool ValidateHostAgainstAccountHostIds(List<int> accountHostIds, string currentAddress, string companyId)
        {
            foreach (int hostId in accountHostIds)
            {
                Host host = GetHost(hostId);
                if (host == null)
                {
                    continue;
                }

                string hostBase = SubstituteHostnameCompanyIdPlaceHolder(host.HostnameDescription, companyId);

                if (hostBase == currentAddress)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The find host name from module.
        /// </summary>
        /// <param name="accountHostIds">
        /// The account Host Ids.
        /// </param>
        /// <param name="currentModule">
        /// The current module.
        /// </param>
        /// <returns>
        /// The first matching host name id that matches the module for the account's list of host id's.
        /// </returns>
        public static int GetMatchingHostnameIdFromModuleForAccountIds(List<int> accountHostIds, Modules currentModule)
        {
            if (accountHostIds.Count == 1)
            {
                return accountHostIds[0];
            }

            int moduleId = Convert.ToInt32(currentModule);

            var hosts = accountHostIds.Select(GetHost).Where(host => host != null).ToArray();
            var hostOfCurrentModule = hosts.FirstOrDefault(host => host.ModuleId == moduleId);
            return hostOfCurrentModule != null ? hostOfCurrentModule.HostnameId : 0;
        }
    }
}
