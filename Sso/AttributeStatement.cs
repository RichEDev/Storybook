namespace Sso
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Sso.AttribStatement;

    /// <summary>
    ///     The attribute statement.
    /// </summary>
    public class AttributeStatement
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributeStatement" /> class.
        /// </summary>
        public AttributeStatement()
        {
            this.Attributes = new List<SsoAttribute>();
        }
        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the attributes.
        /// </summary>
        public List<SsoAttribute> Attributes { get; private set; }

        #endregion
    }
}