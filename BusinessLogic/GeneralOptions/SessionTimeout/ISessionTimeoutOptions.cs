namespace BusinessLogic.GeneralOptions.SessionTimeout
{
    /// <summary>
    /// Defines a <see cref="ISessionTimeoutOptions"/> and it's members
    /// </summary>
    public interface ISessionTimeoutOptions
    {
        /// <summary>
        /// Gets or sets the idle timeout
        /// </summary>
        int IdleTimeout { get; set; }

        /// <summary>
        /// Gets or sets the countdown timer
        /// </summary>
        int CountdownTimer { get; set; }
    }
}
