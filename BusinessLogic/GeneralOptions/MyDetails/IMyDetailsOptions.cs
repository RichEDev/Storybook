namespace BusinessLogic.GeneralOptions.MyDetails
{
    /// <summary>
    /// Defines a <see cref="IMyDetailsOptions"/> and it's members
    /// </summary>
    public interface IMyDetailsOptions
    {
        /// <summary>
        /// Gets or sets the edit my details
        /// </summary>
        bool EditMyDetails { get; set; }

        /// <summary>
        /// Gets or sets the allow employee to notify of change of details
        /// </summary>
        bool AllowEmployeeToNotifyOfChangeOfDetails { get; set; }
    }
}
