namespace BusinessLogic.Employees
{
    using BusinessLogic.Interfaces;

    /// <summary>
    /// An interface for common fields of an Employee
    /// </summary>
    public interface IEmployee : IIdentifier<int>
    {
        /// <summary>
        /// Gets or sets the sub account id of this <see cref="IEmployee"/>
        /// </summary>
        int SubAccountId { get; set; }
    }
}
