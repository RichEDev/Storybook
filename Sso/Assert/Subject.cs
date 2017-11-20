namespace Sso.Assert
{
    using System.Collections.ObjectModel;

    using Sso.Assert.SubjectConf;

    /// <summary>
    ///     The subject.
    /// </summary>
    public class Subject
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Subject" /> class.
        /// </summary>
        public Subject()
        {
            this.SubjectConfirmations = new Collection<SubjectConfirmation>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name id.
        /// </summary>
        public NameIdentifier NameId { get; set; }

        /// <summary>
        ///     Gets the subject confirmations.
        /// </summary>
        public Collection<SubjectConfirmation> SubjectConfirmations { get; private set; }

        #endregion
    }
}