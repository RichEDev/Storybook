namespace BusinessLogic.DataConnections
{
    using BusinessLogic.Cache;

    /// <summary>
    /// Defines an object handles <see cref="IDataFactoryCustom{TComplexType,TPrimaryKeyDataType}"/> and the ability to GetByCustom.
    /// </summary>
    /// <typeparam name="TComplexType">The complex type this interface operates on.</typeparam>
    /// <typeparam name="TPrimaryKeyDataType">The primary key data type of the complex type this interface operates on.</typeparam>
    public interface IDataFactoryCustom<TComplexType, in TPrimaryKeyDataType, out TArchivedReturnType> : IDataFactoryArchivable<TComplexType, TPrimaryKeyDataType, TArchivedReturnType> where TComplexType : class
    {
        /// <summary>
        /// Gets an instance of <typeparamref name="TComplexType"/> using a <see cref="GetByCustom"/> instance from cache.
        /// </summary>
        /// <param name="customGet">An instance of <see cref="GetByCustom"/></param>
        /// <returns>An instance of <typeparamref name="TComplexType"/> from cache</returns>
        TComplexType GetByCustom(GetByCustom customGet);
    }
}
