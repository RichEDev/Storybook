namespace SpendManagementLibrary.GreenLight
{
    using System;

    /// <summary>
    /// Represents a view that has been disabled in a menu.
    /// </summary>
    [Serializable]
    public class MenuView
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MenuView"/> class.
        /// </summary>
        /// <param name="menuId">
        /// The menu id.
        /// </param>
        /// <param name="viewId">
        /// The view id.
        /// </param>
        public MenuView(int menuId, int viewId)
        {
            this.MenuId = menuId;
            this.ViewId = viewId;
        }

        /// <summary>
        /// Gets or sets the ViewId.
        /// </summary>
        public int ViewId { get; set; }

        /// <summary>
        /// Gets or sets the MenuId.
        /// </summary>
        public int MenuId { get; set; }
    }
}