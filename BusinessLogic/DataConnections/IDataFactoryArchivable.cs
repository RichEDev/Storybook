namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// Defines an object handles <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> and the ability to Archive.
    /// </summary>
    /// <typeparam name="TComplexType">The complex type this interface operates on.</typeparam>
    /// <typeparam name="TPrimaryKeyDataType">The primary key data type of the complex type this interface operates on.</typeparam>
    public interface IDataFactoryArchivable<TComplexType, in TPrimaryKeyDataType, out TArchivedReturnType> : IDataFactory<TComplexType, TPrimaryKeyDataType> where TComplexType : class
    {
        /// <summary>
        /// Archives a <typeparamref name="TComplexType"/> with the matching <typeparamref name="TPrimaryKeyDataType"/> <paramref name="id"/>.-
        /// </summary>
        /// <param name="id">The id to delete.</param>
        /// <returns>A <see cref="TArchivedReturnType"/> indicating the archive status after execution.</returns>
        TArchivedReturnType Archive(TPrimaryKeyDataType id);
    }
}
