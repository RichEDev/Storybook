namespace Common.Logging.Converters
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Ensures the <code>Trace.CorrelationManager.ActivityId</code> is set so we can output as part of correlated logging.
    /// </summary>
    public class TraceActivityId
    {
        /// <summary>
        /// Sets the <see cref="Trace.CorrelationManager"/>.ActivityId and returns its current value as a <see cref="string"/>
        /// </summary>
        /// <returns>The current ActivityId</returns>
        public override string ToString()
        {
            if (Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            }

            return Trace.CorrelationManager.ActivityId.ToString();
        }
    }
}