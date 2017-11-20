namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// The BaseClassToAPIType interface.
    /// </summary>
    /// <typeparam name="TDal">
    /// </typeparam>
    /// <typeparam name="TApi">
    /// </typeparam>
    public interface IBaseClassToAPIType<TDal, out TApi>
        where TDal : class
        where TApi : class
    {
        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="dbType">
        /// The db Type.
        /// </param>
        /// <param name="actionContext">
        /// The actionContext which contains DAL classes.
        /// </param>
        /// <returns>
        /// A API Type
        /// </returns>
        TApi ToApiType(TDal dbType, IActionContext actionContext);
    }
}
