namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// Defines an object that is used as more readable front for a data access object.   
    /// </summary>
    /// <typeparam name="TDal">The DAL Type</typeparam>
    /// <typeparam name="TApi">The API Type</typeparam>
    public interface IApiFrontForDbObject<TDal, out TApi> 
        where TDal : class 
        where TApi : class
    {

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        TApi From(TDal dbType, IActionContext actionContext);
        
        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        TDal To(IActionContext actionContext);
    }


    /// <summary>
    /// Defines a type that can be archived.
    /// </summary>
    public interface IArchivable
    {
        /// <summary>
        /// Whether this item is archived
        /// </summary>
        bool Archived { get; set; }
    }
}
