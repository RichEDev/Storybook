namespace BusinessLogic.DataConnections
{
    /// <summary>
    /// An interface for a customer data connection
    /// </summary>
    public interface ICustomerDataConnection<T> : IDataConnection<T> where T : class
    {
    }
}