namespace BusinessLogic.GeneralOptions.Cache
{
    /// <summary>
    /// Defines a <see cref="ICacheOptions"/> and it's members
    /// </summary>
    public interface ICacheOptions
    {
        /// <summary>
        /// Gets or sets the cache timeout.
        /// </summary>
        int CacheTimeout { get; set; }

        /// <summary>
        /// Gets or sets the cache period for a short period.
        /// </summary>
        int CachePeriodShort { get; set; }

        /// <summary>
        /// Gets or sets the cache period for a normal period.
        /// </summary>
        int CachePeriodNormal { get; set; }

        /// <summary>
        /// Gets or sets the cache period for a long period.
        /// </summary>
        int CachePeriodLong { get; set; }
    }
}
