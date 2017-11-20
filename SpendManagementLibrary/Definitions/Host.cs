namespace SpendManagementLibrary.Definitions
{
    using System;

    /// <summary>
    /// Class to represent the hostnames table in the MetaBase
    /// </summary>
    [Serializable]
    public class Host
    {
        /// <summary>
        /// Initialises a new instance of the Hostname class.
        /// </summary>
        /// <param name="hostNameId">
        /// The hostName id.
        /// </param>
        /// <param name="hostName">
        /// The host name (taken from Request.Url.Host).
        /// </param>
        /// <param name="moduleId">
        /// The associated module id.
        /// </param>
        /// <param name="theme">
        /// The theme for the module.
        /// </param>
        public Host(int hostNameId, string hostName, int moduleId, string theme)
        {
            this.HostnameId = hostNameId;
            this.HostnameDescription = hostName;
            this.ModuleId = moduleId;
            this.Theme = theme;
        }

        /// <summary>
        /// Gets the hostname id.
        /// </summary>
        public int HostnameId { get; private set; }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        public string HostnameDescription { get; private set; }

        /// <summary>
        /// Gets the module id.
        /// </summary>
        public int ModuleId { get; private set; }

        /// <summary>
        /// Gets the Theme.
        /// </summary>
        public string Theme { get; private set; }
    }
}