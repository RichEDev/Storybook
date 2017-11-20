namespace BusinessLogic.Tables
{
    using System;

    using BusinessLogic.Tables.Type;

    using Common.Logging;

    /// <summary>
    /// Extends the <see cref="RepositoryBase{T,TK}"/> with a <see cref="ITable"/> indexer and <c>GetParentTable(Guid id)</c>.
    /// </summary>
    public abstract class TableRepository : RepositoryBase<ITable, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRepository"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        protected TableRepository(ILog logger) : base(logger)
        {
        }

        /// <summary>
        /// Gets an instance of <see cref="ITable"/> with a matching name from memory if possible.
        /// </summary>
        /// <param name="name">The name of the <see cref="ITable"/> you want to retrieve</param>
        /// <returns>The required <see cref="ITable"/> or null if it cannot be found</returns>
        public abstract ITable this[string name] { get; }

        /// <summary>
        /// Gets an instance of <see cref="ITable"/> which is the parent of the <see cref="Guid"/> given.
        /// </summary>
        /// <param name="id">The id of the parent <see cref="ITable"/></param>
        /// <returns>The <see cref="ITable"/> with a matching id.</returns>
        public abstract ITable GetParentTable(Guid id);
    }
}
