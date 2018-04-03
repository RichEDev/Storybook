namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A representation of an item role.
    /// </summary>
    [Serializable]
    public class ItemRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRole"/> class.
        /// </summary>
        /// <param name="itemRoleId">
        /// The id of the item role.
        /// </param>
        /// <param name="rolename">
        /// The name of the item role.
        /// </param>
        /// <param name="description">
        /// The description of the item role.
        /// </param>
        /// <param name="subcats">
        /// The list of expense items associated to this item role.
        /// </param>
        /// <param name="createdOn">
        /// When the item role was created.
        /// </param>
        /// <param name="createdby">
        /// The id of the employee who created the item role.
        /// </param>
        /// <param name="modifiedon">
        /// The last time the item role was modified.
        /// </param>
        /// <param name="modifiedby">
        /// The id of the employee last to modify the item role.
        /// </param>
        public ItemRole(int itemRoleId, string rolename, string description, Dictionary<int, RoleSubcat> subcats, DateTime createdOn, int createdby, DateTime modifiedon, int modifiedby)
        {
            this.ItemRoleId = itemRoleId;
            this.Rolename = rolename;
            this.Description = description;
            this.Items = subcats;
            this.CreatedOn = createdOn;
            this.CreatedBy = createdby;
            this.ModifiedOn = modifiedon;
            this.ModifiedBy = modifiedby;
        }

        #region properties
        /// <summary>
        /// Gets the id of the item role
        /// </summary>
        public int ItemRoleId { get; }

        /// <summary>
        /// Gets the name of the item role
        /// </summary>
        public string Rolename { get; }

        /// <summary>
        /// Gets the description of the item role
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a list of the expense items associated to this item role
        /// </summary>
        public Dictionary<int, RoleSubcat> Items { get; }

        /// <summary>
        /// Gets when the item role was created
        /// </summary>
        public DateTime CreatedOn { get; }

        /// <summary>
        /// Gets the id of the employee that created the item role
        /// </summary>
        public int CreatedBy { get; }

        /// <summary>
        /// Gets when the item role was last modified
        /// </summary>
        public DateTime ModifiedOn { get; }

        /// <summary>
        /// Gets the id of the employee who last modified the item role
        /// </summary>
        public int ModifiedBy { get; }
        #endregion

        /// <summary>
        /// Removes the association between this item role and the supplied expense item
        /// </summary>
        /// <param name="subcatid">
        /// The id of the expense item to remove the association from.
        /// </param>
        public void DeleteSubcat(int subcatid)
        {
            this.Items.Remove(subcatid);
        }

        /// <summary>
        /// Returns an instance of a <see cref="RoleSubcat"/> associated to this item role.
        /// </summary>
        /// <param name="id">
        /// The id of the assciated expense item to return.
        /// </param>
        /// <returns>
        /// The <see cref="RoleSubcat"/>.
        /// </returns>
        public RoleSubcat GetRoleSubcatById(int id)
        {
            foreach (RoleSubcat r in this.Items.Values)
            {
                if (r.RolesubcatId == id)
                {
                    return r;
                }
            }

            return null;
        }
    }
}
