namespace BusinessLogic.Fields
{
    using System;

    using BusinessLogic.Fields.Type.Base;

    using Common.Logging;

    /// <summary>
    /// An implementation of <see cref="FieldRepository"/> to get and store <see cref="IField"/>
    /// </summary>
    public abstract class FieldRepository : RepositoryBase<IField, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldRepository"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        protected FieldRepository(ILog logger) : base(logger)
        {
        }
    }
}
