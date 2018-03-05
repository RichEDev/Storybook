using BusinessLogic.DataConnections;
using BusinessLogic.Interfaces;

namespace CacheDataAccess.Caching
{
    public interface ICacheFactory<T, TK> : IGetBy<T, TK>, ISave<T>
        where T : class, IIdentifier<TK>
    {
    }
}