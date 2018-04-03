using System;

namespace SpendManagementLibrary
{
    /// <summary>
    /// A representation of an item role to expense item association.
    /// </summary>
    [Serializable]
    public class RoleSubcat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleSubcat"/> class.
        /// </summary>
        public RoleSubcat()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleSubcat"/> class.
        /// </summary>
        /// <param name="rolesubcatid">
        /// The id of the rolesubat.
        /// </param>
        /// <param name="roleid">
        /// The id of the item role.
        /// </param>
        /// <param name="subCatId">
        /// The id of the expense item.
        /// </param>
        /// <param name="maximumLimitWithoutReceipt">
        /// The maximum limit without receipt.
        /// </param>
        /// <param name="maximumLimitWithReceipt">
        /// The maximum limit with receipt.
        /// </param>
        /// <param name="isadditem">
        /// Whether to include this item on the add expense general template by default
        /// </param>
        public RoleSubcat(int rolesubcatid, int roleid, int subCatId, decimal maximumLimitWithoutReceipt, decimal maximumLimitWithReceipt, bool isadditem)
        {
            this.RolesubcatId = rolesubcatid;
            this.RoleId = roleid;
            this.SubcatId = subCatId;
            this.MaximumLimitWithoutReceipt = maximumLimitWithoutReceipt;
            this.MaximumLimitWithReceipt = maximumLimitWithReceipt;
            this.Isadditem = isadditem;
        }

        #region properties
        /// <summary>
        /// Gets yhe unique key of the role subcat
        /// </summary>
        public int RolesubcatId { get; }

        /// <summary>
        /// Gets the id of the item role the rolesubcatid is associated to
        /// </summary>
        public int RoleId { get; }

        /// <summary>
        /// Gets the id of the expense item the rolesubcatid is associated to
        /// </summary>
        public int SubcatId { get; }

        /// <summary>
        /// Gets or sets the maximum limit without a receipt
        /// </summary>
        public decimal MaximumLimitWithoutReceipt { get; set; }

        /// <summary>
        /// Gets or sets the maximum limit with a receipt
        /// </summary>
        public decimal MaximumLimitWithReceipt { get; set; }

        /// <summary>
        /// Gets or sets whether to include this rolesubcat on the default add expense view
        /// </summary>
        public bool Isadditem { get; set; }
        #endregion
    }

}
