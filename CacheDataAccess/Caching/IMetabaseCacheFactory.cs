namespace CacheDataAccess.Caching
{
    using BusinessLogic.Interfaces;

    public interface IMetabaseCacheFactory<T, TK> : ICacheFactory<T, TK>
        where T : class, IIdentifier<TK>
    {
    }
}
