namespace Common.Logging.Log4Net
{
    using System;
    using System.Collections.Generic;

    using log4net.Core;
    using log4net.Filter;

    /// <summary>
    /// Enables supporting AND filters (composite) with log4net.
    /// </summary>
    /// <remarks>
    /// Usage within the log4net config:
    /// <filter type="Common.Logging.Log4Net.AndFilter, Common">
    ///   <filter type = "log4net.Filter.PropertyFilter">
    ///   <key value="url" />
    ///   <stringToMatch value = "/foo/foobar.aspx" />
    ///   </filter >
    ///   <filter type="log4net.Filter.PropertyFilter">
    ///     <key value = "employeeid" />
    ///     <stringToMatch value="543" />
    ///   </filter>
    ///   // Do not log any messages that match the above criteria if this is set to false
    ///   <acceptOnMatch value = "false" />
    /// </filter>
    /// Note that this will override the default whitelisting/blacklisting on filtering.
    /// </remarks>
    public class AndFilter : FilterSkeleton
    {
        /// <summary>
        /// Collection of <see cref="IFilter"/>
        /// </summary>
        private readonly IList<IFilter> _filters = new List<IFilter>();

        /// <summary>
        /// Whether or not to accept on match or not.
        /// </summary>
        private bool _acceptOnMatch;

        /// <summary>
        /// Gets or sets a value indicating whether or not this filter should accept on a match and log the entry or block and deny.
        /// </summary>
        public bool AcceptOnMatch
        {
            get { return this._acceptOnMatch; }
            set { this._acceptOnMatch = value; }
        }

        /// <summary>
        /// Sets an additional <see cref="IFilter"/> to enable AND statement style filtering.
        /// </summary>
        public IFilter Filter
        {
            set { this._filters.Add(value); }
        }

        /// <summary>
        /// Decide if the <see cref="T:log4net.Core.LoggingEvent" /> should be logged through an appender.
        /// </summary>
        /// <param name="loggingEvent">The <see cref="T:log4net.Core.LoggingEvent" /> to decide upon</param>
        /// <returns>The decision of the filter</returns>
        /// <remarks>
        /// <para>
        /// If the decision is <see cref="F:log4net.Filter.FilterDecision.Deny" />, then the event will be
        /// dropped. If the decision is <see cref="F:log4net.Filter.FilterDecision.Neutral" />, then the next
        /// filter, if any, will be invoked. If the decision is <see cref="F:log4net.Filter.FilterDecision.Accept" /> then
        /// the event will be logged without consulting with other filters in
        /// the chain.
        /// </para>
        /// <para>
        /// This method is marked <c>abstract</c> and must be implemented
        /// in a subclass.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="loggingEvent"/> is <see langword="null"/></exception>
        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException(nameof(loggingEvent));
            }

            foreach (IFilter filter in this._filters)
            {
                if (filter.Decide(loggingEvent) != FilterDecision.Accept)
                {
                    // one of the filters criteria has not been met.
                    return FilterDecision.Neutral; 
                }
            }

            // All conditions are true, return Accept or Deny based on this._acceptOnMatch as defined within the configuration.
            return this._acceptOnMatch ? FilterDecision.Accept : FilterDecision.Deny;
        }
    }
}
