namespace Sso.Shared
{
    /// <summary>
    /// The id.
    /// </summary>
    public class Id
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Id(string id)
        {
            this.ID = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public string ID { get; set; }

        #endregion
    }
}