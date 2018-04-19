namespace BusinessLogic.GeneralOptions.MyDetails
{
    /// <summary>
    /// Defines a <see cref="MyDetailsOptions"/> and it's members
    /// </summary>
    public class MyDetailsOptions : IMyDetailsOptions
    {
        /// <summary>
        /// Gets or sets the edit my details
        /// </summary>
        public bool EditMyDetails { get; set; }

        /// <summary>
        /// Gets or sets the allow employee to notify of change of details
        /// </summary>
        public bool AllowEmployeeToNotifyOfChangeOfDetails { get; set; }
    }
}
