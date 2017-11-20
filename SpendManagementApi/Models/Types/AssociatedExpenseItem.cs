namespace SpendManagementApi.Models.Types
{
    using System;

    /// <summary>
    /// An expense item (subcat) that belongs to the expense item collection on a flag.
    /// </summary>

    public class AssociatedExpenseItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AssociatedExpenseItem"/> class.
        /// </summary>
        /// <param name="subcatId">
        /// The id of the subcat.
        /// </param>
        /// <param name="name">
        /// The name of the subcat.
        /// </param>
        public AssociatedExpenseItem(int subcatId, string name)
        {
            this.SubcatId = subcatId;
            this.Name = name;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the subcat id.
        /// </summary>
        public int SubcatId { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the subcat.
        /// </summary>
        public string Name { get; protected set; }
        #endregion
    }
}