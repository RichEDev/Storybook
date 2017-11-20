namespace SpendManagementLibrary.Report
{
    using System;

    /// <summary>
    /// Defines the name and ID of a group in the Tree.
    /// </summary>
    public class TreeGroup
    {
        /// <summary>
        /// The ID of the Tree Group
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Name of the group as it will appear in the tree.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initialises a new instance of <see cref="TreeGroup"/>
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>Id of the group</param>
        /// <param name="name">The name of the group</param>
        public TreeGroup(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
