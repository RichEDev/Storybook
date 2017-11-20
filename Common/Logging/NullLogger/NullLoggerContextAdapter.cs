namespace Common.Logging.NullLogger
{
    using System.Collections.Generic;

    /// <summary>
    /// Enables additional context to be output within logging messages when using <see cref="NullLoggerContextAdapter"/>.
    /// </summary>
    public class NullLoggerContextAdapter : IExtraContext
    {
        /// <summary>
        /// Private collection of properties.
        /// </summary>
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a property
        /// </summary>
        /// <param name="key">The name of the property to modify</param>
        /// <returns>The value of the property with the matching <paramref name="key" /></returns>
        public object this[string key]
        {
            get
            {
                return this._properties[key];
            }

            set
            {
                this._properties.Remove(key);
                this._properties.Add(key, value);
            }
        }

        /// <summary>
        /// Clears all context from <see cref="IExtraContext"/>.
        /// </summary>
        public void Clear()
        {
            this._properties.Clear();
        }

        /// <summary>
        /// Removes a property from <see cref="IExtraContext"/>
        /// </summary>
        /// <param name="key">The key of the property to remove</param>
        public void Remove(string key)
        {
            this._properties.Remove(key);
        }
    }
}
