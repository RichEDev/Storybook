namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// Defines an object handles <see cref="IGetBy{T,TK}"/>, <see cref="IAdd{T}"/>, <see cref="IDelete{T}"/> and <see cref="IGetAll{T}"/>
    /// </summary>
    /// <typeparam name="TComplexType">The complex type this interface operates on.</typeparam>
    /// <typeparam name="TPrimaryKeyDataType">The primary key data type of the complex type this interface operates on.</typeparam>
    public interface IDataFactory<TComplexType, in TPrimaryKeyDataType> : IGetBy<TComplexType, TPrimaryKeyDataType>, IAdd<TComplexType>, IDelete<TPrimaryKeyDataType>, IGetAll<TComplexType>
        where TComplexType : class
    {
    }
}
