namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// The list item class.
    /// </summary>
    [Serializable()]
    public class cListItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cListItem"/> class.
        /// </summary>
        /// <param name="userdefineid">
        /// The userdefined field id.
        /// </param>
        /// <param name="itemid">
        /// The item id.
        /// </param>
        /// <param name="item">
        /// The item text.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="createdon">
        /// The createdon date.
        /// </param>
        /// <param name="createdby">
        /// The createdby employee ID.
        /// </param>
        /// <param name="modifiedon">
        /// The modified on date.
        /// </param>
        /// <param name="modifiedby">
        /// The modified by employee ID.
        /// </param>
        public cListItem(int userdefineid, int itemid, string item, string comment, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby)
        {
            this.itemid = itemid;
            this.userdefineid = userdefineid;
            this.item = item;
            this.comment = comment;
            this.createdon = createdon;
            this.createdby = createdby;
            this.modifiedon = modifiedon;
            this.modifiedby = modifiedby;
        }

        #region properties

        /// <summary>
        /// Gets the list item ID.
        /// </summary>
        public int itemid { get; }

        /// <summary>
        /// Gets the userdefined field ID.
        /// </summary>
        public int userdefineid { get; }

        /// <summary>
        /// Gets the item display text.
        /// </summary>
        public string item { get; }

        /// <summary>
        /// Gets the comment for the field.
        /// </summary>
        public string comment { get; }

        /// <summary>
        /// Gets the created on <see cref="DateTime"/>.
        /// </summary>
        public DateTime createdon { get; }

        /// <summary>
        /// Gets the created by employee ID.
        /// </summary>
        public int createdby { get; }

        /// <summary>
        /// Gets the modified on <see cref="DateTime"/>.
        /// </summary>
        public DateTime modifiedon { get; }

        /// <summary>
        /// Gets the modified by employee ID.
        /// </summary>
        public int modifiedby { get; }

        #endregion
    }
}
