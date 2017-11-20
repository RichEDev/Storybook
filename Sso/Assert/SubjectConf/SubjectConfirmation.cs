namespace Sso.Assert.SubjectConf
{
    using System;

    using Sso.Assert;

    /// <summary>
    ///     The subject confirmation.
    /// </summary>
    public class SubjectConfirmation
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectConfirmation"/> class.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public SubjectConfirmation(Uri method, SubjectConfirmationData data)
        {
            this.Method = method;
            this.SubjectConfirmationData = data;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the method.
        /// </summary>
        public Uri Method { get; set; }

        /// <summary>
        ///     Gets or sets the name identifier.
        /// </summary>
        public NameIdentifier NameIdentifier { get; set; }

        /// <summary>
        ///     Gets or sets the subject confirmation data.
        /// </summary>
        public SubjectConfirmationData SubjectConfirmationData { get; set; }

        #endregion
    }
}