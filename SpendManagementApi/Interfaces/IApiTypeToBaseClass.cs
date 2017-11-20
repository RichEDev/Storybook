namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// The ApiTypeToBaseClass interface.
    /// </summary>
    /// <typeparam name="TDal">
    /// </typeparam>
    /// <typeparam name="TApi">
    /// </typeparam>
    public interface IApiTypeToBaseClass<TDal, out TApi>
        where TDal : class
        where TApi : class
    {
        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        TDal ToBaseClass(IActionContext actionContext);
    }
}
