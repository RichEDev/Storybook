namespace CacheDataAccess.ProjectCodes
{
    using BusinessLogic.Cache;
    using BusinessLogic.ProjectCodes;

    /// <summary>
    /// Enables the retrieval of a <see cref="IProjectCodeWithUserDefinedFields"/> from cache with a matching description.
    /// </summary>
    public class GetByProjectCodeDescription : GetByCustom
    {
        /// <summary>
        /// Enables the retrieval of a <see cref="IProjectCodeWithUserDefinedFields"/> from cache with a matching description.
        /// </summary>
        /// <param name="description">The description of the <see cref="IProjectCodeWithUserDefinedFields"/> you wish to retrieve.</param>
        public GetByProjectCodeDescription(string description) : base("descriptions", description)
        {
            
        }
    }
}
