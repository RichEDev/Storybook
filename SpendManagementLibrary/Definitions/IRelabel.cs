namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Logic_Classes.Fields;

    public interface IRelabel
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the relabel item to use (if any) used in <see cref="IRelabler{T}"/>
        /// </summary>
        string RelabelParam { get; set; }
    }
}