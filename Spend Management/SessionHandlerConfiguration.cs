namespace Spend_Management
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Defines the session config section within the web.config
    /// </summary>
    public class SessionConfigSection : ConfigurationSection
    {
        /// <summary>
        /// Returns all child instances with in the session config section
        /// </summary>
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public SessionInstanceCollection Instances
        {
            get { return (SessionInstanceCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    /// <summary>
    /// Defines the collection of instances within the session config section
    /// </summary>
    public class SessionInstanceCollection : ConfigurationElementCollection
    {
        /// <summary>When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
        /// <returns>A new <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SessionInstanceElement();
        }

        /// <summary>Gets the element key for a specified configuration element when overridden in a derived class.</summary>
        /// <returns>An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SessionInstanceElement)element).Path;
        }
    }

    /// <summary>
    /// Defines a specific "add" element within the session config section in the web.config
    /// </summary>
    public class SessionInstanceElement : ConfigurationElement
    {
        /// <summary>
        /// The path to apply read-write access to.
        /// </summary>
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }
    }

    /// <summary>
    /// Accesses the Session config section in the web.config
    /// </summary>
    public class SessionConfig
    {
        /// <summary>
        /// Private collection of paths in the Session config section of the web.config
        /// </summary>
        private static readonly List<string> PathsCollection;

        static SessionConfig()
        {
            PathsCollection = new List<string>();

            SessionConfigSection sec = (SessionConfigSection)ConfigurationManager.GetSection("sessionConfig");
            foreach (SessionInstanceElement i in sec.Instances)
            {
                PathsCollection.Add(i.Path.ToLower());
            }
        }

        /// <summary>
        /// Gets a list of paths defined in the web.config as read-write access to session.
        /// </summary>
        public static List<string> Paths => PathsCollection;
    }
}