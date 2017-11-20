namespace CacheDataAccess.ProjectCodes
{
    using BusinessLogic.Cache;
    using BusinessLogic.ProjectCodes;

    /// <summary>
    /// Enables the retrieval of a <see cref="IProjectCodeWithUserDefinedFields"/> from cache with a matching name.
    /// </summary>
    public class GetByProjectCodeName : GetByCustom
    {
        /// <summary>
        /// Enables the retrieval of a <see cref="IProjectCodeWithUserDefinedFields"/> from cache with a matching name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IProjectCodeWithUserDefinedFields"/> you wish to retrieve.</param>
        public GetByProjectCodeName(string name) : base("names", name)
        {
            
        }
    }
}
