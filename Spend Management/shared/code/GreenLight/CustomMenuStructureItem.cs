
namespace Spend_Management.shared.code.GreenLight
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class to represent details about the custom menus.
    /// </summary>
    [Serializable]
    public class CustomMenuStructureItem
    {

        /// <summary>
        /// Custom menu identifier.
        /// </summary>
        public int CustomMenuId { get; set; }

        /// <summary>
        /// Custom menu name.
        /// </summary>
        public string CustomMenuName { get; set; }

        /// <summary>
        /// Parent of the custom menu.
        /// </summary>
        public int CustomParentId { get; set; }

        /// <summary>
        /// Description about the custom menu.
        /// </summary>
        public string CustomMenuDescription { get; set; }

        /// <summary>
        /// Custom menu icon.
        /// </summary>
        public string CustomMenuIcon { get; set; }

        /// <summary>
        /// Order to display.
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// Created employee id.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Modified employee id.
        /// </summary>
        public int? ModifiedBy { get; set; }
        /// <summary>
        /// Check for system menu.
        /// </summary>
        public bool SystemMenu { get; set; }

        /// <summary>
        /// Auto generated reference ID from JS for new menu items.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        ///  ParentID for new menu items created under a new menu items which have auto generated ID's from JS
        /// </summary>
        public string ReferenceDynamicParentId { get; set; }

        /// <summary>
        /// List of child custom menu.
        /// </summary>
        public List<CustomMenuStructureItem> CustomMenuList { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="CustomMenuStructureItem"/> class.
        /// </summary>
        /// <param name="customMenuId">
        ///  Custom mMenu identifier.
        /// </param>
        /// <param name="customMenuName">
        ///  Custom menu name.
        /// </param>
        /// <param name="customParentid">
        ///  Custom parent identifier.
        /// </param>
        /// <param name="customMenuDescription">
        ///  Custom menu description.
        /// </param>
        /// <param name="customMenuIcon">
        ///  Custom menu Icon.
        /// </param>
        ///  <param name="orderBy">
        ///  Order of display.
        /// </param>
        /// <param name="systemMenu"> Check for system menu.
        /// </param>
        public CustomMenuStructureItem(
            int customMenuId,
            string customMenuName,
            int customParentid,
            string customMenuDescription,
            string customMenuIcon,
            int orderBy,bool systemMenu)
        {
            this.CustomMenuId = customMenuId;
            this.CustomMenuName = customMenuName;
            this.CustomParentId = customParentid;
            this.CustomMenuDescription = customMenuDescription;
            this.CustomMenuIcon =  customMenuIcon;
            this.OrderBy = orderBy;
            this.SystemMenu = systemMenu;
            
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CustomMenuStructureItem"/> class.
        /// </summary>
        /// <param name="customMenuId"></param>
        /// <param name="customMenuName"></param>
        /// <param name="customParentid"></param>
        /// <param name="customMenuDescription"></param>
        /// <param name="customMenuIcon"></param>
        /// <param name="createdBy"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="orderBy"></param>
        /// <param name="referenceId"> Reference id</param>
        /// <param name="referenceDynamicParentId"> Reference parent id</param>
        public CustomMenuStructureItem(
           int customMenuId,
           string customMenuName,
           int customParentid,
           string customMenuDescription,
           string customMenuIcon,
           int? createdBy,
           int? modifiedBy,
           int orderBy, string referenceId = null, string referenceDynamicParentId= null)
        {
            this.CustomMenuId = customMenuId;
            this.CustomMenuName = customMenuName;
            this.CustomParentId = customParentid;
            this.CustomMenuDescription = customMenuDescription;
            this.CustomMenuIcon = customMenuIcon;
            this.CreatedBy = createdBy;
            this.ModifiedBy = modifiedBy;
            this.OrderBy = orderBy;
            this.ReferenceId = referenceId;
            this.ReferenceDynamicParentId = referenceDynamicParentId;
        }

    }
}
