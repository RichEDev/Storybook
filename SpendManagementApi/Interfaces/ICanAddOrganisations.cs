namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// An interface representing if a user can add a new organisation from Add/Edit Expense Item
    /// </summary>
    public interface ICanAddOrganisations
    {
        /// <summary>
        /// Whether the user can add a new from Add/Edit Expense Item
        /// </summary>
        bool CanAddNewOrganisation { get; set; }
    }
}