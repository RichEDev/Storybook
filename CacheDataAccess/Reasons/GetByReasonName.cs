namespace CacheDataAccess.Reasons
{
    using BusinessLogic.Cache;
    using BusinessLogic.Reasons;

    /// <summary>
    /// Enables the retrieval of a <see cref="IReason"/> from cache with a matching name.
    /// </summary>
    public class GetByReasonName : GetByCustom
    {
        /// <summary>
        /// Enables the retrieval of a <see cref="IReason"/> from cache with a matching name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IReason"/> you wish to retrieve.</param>
        public GetByReasonName(string name) : base("names", name)
        {

        }
    }
}
