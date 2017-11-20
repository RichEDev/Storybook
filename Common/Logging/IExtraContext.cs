namespace Common.Logging
{
    /// <summary>
    /// Enables additional context to be output within logging messages.
    /// </summary>
    public interface IExtraContext
    {
        /// <summary>
        /// Gets or sets a property
        /// </summary>
        /// <param name="key">The name of the property to modify</param>
        /// <returns>The value of the property with the matching <paramref name="key" /></returns>
        object this[string key] { get; set; }

        /// <summary>
        /// Clears all context from <see cref="IExtraContext"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes a property from <see cref="IExtraContext"/>
        /// </summary>
        /// <param name="key">The key of the property to remove</param>
        void Remove(string key);
    }
}
