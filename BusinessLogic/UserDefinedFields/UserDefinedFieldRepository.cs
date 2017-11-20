namespace BusinessLogic.UserDefinedFields
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Tables.Type;

    using Common.Logging;

    public abstract class UserDefinedFieldRepository: RepositoryBase<IField, Guid>, IGetBy<IField, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDefinedFieldRepository"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        protected UserDefinedFieldRepository(ILog logger) : base(logger)
        {
        }

        /// <summary>
        /// Get the <see cref="IField"/> that has a name matching the given string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IField this[string name] { get; }

        /// <summary>
        /// Get a <see cref="List{IField}"/> for the given <seealso cref="ITable"/>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract List<IField> this[ITable table] { get; }
    }
}
