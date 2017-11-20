namespace BusinessLogic.CustomFields
{
    using System;
    using Common.Logging;

    using DataConnections;
    using Fields.Type.Base;

    /// <summary>
    /// Repository for storing and accessing instances of <see cref="IField"/>.
    /// </summary>
    public abstract class CustomFieldRepository : RepositoryBase<IField, Guid>, IGetBy<IField, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldRepository"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        protected CustomFieldRepository(ILog logger) : base(logger)
        {
        }

        /// <summary>
        /// Gets the <see cref="IField">IField</see> that matches the supplied field name
        /// </summary>
        /// <param name="name">the field name</param>
        /// <returns>the <see cref="IField">IField</see></returns>
        public abstract IField this[string name] { get; }
    }
}
