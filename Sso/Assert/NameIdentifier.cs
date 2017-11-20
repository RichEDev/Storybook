namespace Sso.Assert
{
    /// <summary>
    ///     The name identifier.
    /// </summary>
    public class NameIdentifier
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NameIdentifier"/> class.
        /// </summary>
        /// <param name="value">
        /// The identifier.
        /// </param>
        public NameIdentifier(string value)
        {
            this.Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}