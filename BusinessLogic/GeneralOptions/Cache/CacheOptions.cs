namespace BusinessLogic.GeneralOptions.Cache
{
    /// <summary>
    /// The cache options.
    /// </summary>
    public class CacheOptions : ICacheOptions
    {
        /// <summary>
        /// Gets or sets the cache timeout.
        /// </summary>
        public int CacheTimeout { get; set; }

        /// <summary>
        /// Gets or sets the cache period for a short period.
        /// </summary>
        public int CachePeriodShort { get; set; }

        /// <summary>
        /// Gets or sets the cache period for a normal period.
        /// </summary>
        public int CachePeriodNormal { get; set; }

        /// <summary>
        /// Gets or sets the cache period for a long period.
        /// </summary>
        public int CachePeriodLong { get; set; }
    }
}
