namespace SpendManagementApi.Interfaces
{
    using Spend_Management;

    /// <summary>
    /// The BasicRepository interface.
    /// </summary>
    public interface IBasicRepository
    {
        /// <summary>
        /// Gets the current user.
        /// </summary>
        ICurrentUser User { get; }

        /// <summary>
        /// Gets the action context.
        /// </summary>
        IActionContext ActionContext { get; }
    }
}