namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;

    using CacheDataAccess.Caching;

    using SQLDataAccess.ProjectCodes;

    /// <summary>
    /// The "Project Code" Mocked objects.
    /// </summary>
    public interface IProjectCodes
    {
        /// <summary>
        /// Gets or sets the Mock'd <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        SqlProjectCodesWithUserDefinedValuesFactory SqlProjectCodesWithUserDefinedValuesFactory { get; set;}

        /// <summary>
        /// Gets or sets an instance of <see cref="AccountCacheFactory{T,TK}"/>
        /// </summary>
        AccountCacheFactory<IProjectCode, int> CacheFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="SqlProjectCodesFactory"/>
        /// </summary>
        SqlProjectCodesFactory SqlProjectCodesFactory { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="ICache{T,TK}"/>.
        /// </summary>
        ICache<IProjectCode, int> Cache { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IAccountCacheKey{T}"/>.
        /// </summary>
        IAccountCacheKey<int> CacheKey { get; set; }

        /// <summary>
        /// Gets or sets the an instance of <see cref="RepositoryBase"/>
        /// </summary>
        RepositoryBase<IProjectCode, int> RepositoryBase { get; set; }
    }
}