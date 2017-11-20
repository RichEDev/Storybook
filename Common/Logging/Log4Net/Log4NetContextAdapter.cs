namespace Common.Logging.Log4Net
{
    using log4net;

    /// <summary>
    /// Enables additional context to be output within logging messages when using <see cref="Log4NetAdapter{T}"/>.
    /// </summary>
    public class Log4NetContextAdapter : IExtraContext
    {
        /// <summary>
        /// Gets or sets a property
        /// </summary>
        /// <param name="key">The name of the property to modify</param>
        /// <returns>The value of the property with the matching <paramref name="key" /></returns>
        public object this[string key]
        {
            get
            {
                return LogicalThreadContext.Properties[key];
            }

            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                LogicalThreadContext.Properties[key] = value;
            }
        }

        /// <summary>
        /// Clears all context from <see cref="Log4NetContextAdapter"/>.
        /// </summary>
        public void Clear()
        {
            LogicalThreadContext.Properties.Clear();
        }

        /// <summary>
        /// Removes a property from <see cref="Log4NetContextAdapter"/>
        /// </summary>
        /// <param name="key">The key of the property to remove</param>
        public void Remove(string key)
        {
            LogicalThreadContext.Properties.Remove(key);
        }
    }
}
