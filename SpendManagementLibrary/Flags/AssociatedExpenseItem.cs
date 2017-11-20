namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An expense item (subcat) that belongs to the expense item collection on a flag.
    /// </summary>
    [Serializable]
    [DataContract]
    public class AssociatedExpenseItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AssociatedExpenseItem"/> class.
        /// </summary>
        /// <param name="subcatid">
        /// The id of the subcat.
        /// </param>
        /// <param name="name">
        /// The name of the subcat.
        /// </param>
        public AssociatedExpenseItem(int subcatid, string name)
        {
            this.SubcatID = subcatid;
            this.Name = name;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the subcat id.
        /// </summary>
        [DataMember]
        public int SubcatID { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the subcat.
        /// </summary>
        [DataMember]
        public string Name { get; protected set; }
        #endregion
    }
}
