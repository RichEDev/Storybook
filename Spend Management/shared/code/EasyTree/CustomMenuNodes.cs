using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spend_Management.shared.code.GreenLight;
using System.Text;
namespace Spend_Management.shared.code.EasyTree
{
    /// <summary>
    /// Summary description for Custom Menu Nodes.
    /// </summary>
    public class CustomMenuNodes
    {
        #region declaration
        /// <summary>
        /// Custom menu item list.
        /// </summary>
        private Dictionary<int, CustomMenuStructureItem> customMenuList;
        /// <summary>
        /// This store current user
        /// </summary>
        private CurrentUser user = cMisc.GetCurrentUser();
      
        #endregion

        /// <summary>
        /// Get child nodes for menu items.
        /// </summary>
        /// <param name="menuLoop"></param>
        /// <param name="result"></param>
        /// <param name="idCrumbs"></param>
        private void GetNodesForCustomMenuStructure(int? menuLoop, List<EasyTreeNode> result, string idCrumbs)
        {
            var customMenu = new CustomMenuStructure(user.AccountID);
            List<CustomMenuStructureItem> menuResult = customMenu.GetCustomMenusByParentId(menuLoop);

            foreach (var customObj in menuResult)
            {
                var text = customObj.CustomMenuName;
                var crumbText = idCrumbs + ":" + text;
                var isFolder = false;
                var customMenuId = customObj.CustomMenuId;

                if (this.CheckForChildrenNodes(customMenuId))
                {
                    isFolder = true;
                }
                var node = new CustomMenuNode
                {
                    internalId = customObj.CustomMenuId.ToString(),
                    isFolder = isFolder,
                    text = text,
                    liClass = isFolder ? "field" : "field f1",
                    crumbs = crumbText,
                    IconName = customObj.CustomMenuIcon,
                    Description = customObj.CustomMenuDescription,
                    ParentId = customObj.CustomParentId,
                    IsSystemMenu = customObj.SystemMenu,
                    Order = customObj.OrderBy
                };
                if (node.isFolder)
                {
                    node.children = new List<EasyTreeNode>();
                    this.GetNodesForCustomMenuStructure(customMenuId, node.children, node.crumbs);
                }
                result.Add(node);

            }
        }

        /// <summary>
        /// Get the loweset level of custom menu tree items plus their children down to the specified level.
        /// </summary>
        public List<EasyTreeNode> GetInitialMenuEasyTreeNodes()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var customMenu = new CustomMenuStructure(user.AccountID);
            this.customMenuList = customMenu.CacheList();
            var result = new List<EasyTreeNode>();
            this.GetNodesForCustomMenuStructure(0, result, "CustomMenu");
            return result;
        }
              

        /// <summary>
        /// Check whether custom menu has child nodes.
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private bool CheckForChildrenNodes(int parentId)
        {
            return this.customMenuList.Values.Count(item => item.CustomParentId == parentId) > 0;
        }
               
        /// <summary>
        /// Iterates up through the menu item parents and returns links until top menu hit
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="breadcrumblength"></param>
        /// <param name="isTruncated"></param>
        /// <returns>HTML string of menu links</returns>
        public string GenerateCustomMenuBreadcrumb(CustomMenuStructureItem menu,ref int breadcrumbLength,ref bool isTruncated)
        {           
            StringBuilder customMenuBreadcrumb = new StringBuilder();
            CurrentUser user = cMisc.GetCurrentUser();
            var customMenu = new CustomMenuStructure(user.AccountID);
            menu = customMenu.GetCustomMenuById(menu.CustomParentId);
            var entities = new cCustomEntities(user);

            if (menu.CustomParentId > 1)
            {
                customMenuBreadcrumb.Append(this.GenerateCustomMenuBreadcrumb(menu, ref breadcrumbLength, ref isTruncated));
            }
            var title = menu.CustomMenuName;
            var menuUrl = string.Empty;
            entities.GetMenuUrl(menu.CustomMenuId, ref title, ref menuUrl, menu.SystemMenu);
                      
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(menuUrl))
            {
                breadcrumbLength = breadcrumbLength + title.Trim().Length;             
                if (isTruncated)
                    return customMenuBreadcrumb.ToString();
                if (breadcrumbLength > 75)
                {
                    customMenuBreadcrumb.Append("<li><label class='breadcrumb_arrow'>/</label><span>...</span>");
                    isTruncated = true;
                }
                else
                {
                    customMenuBreadcrumb.AppendFormat("<li><label class='breadcrumb_arrow'>/</label><a class='breadcrumbtitle' href='{0}'>{1}</a></li>",menuUrl, title);
                }
            }
            return customMenuBreadcrumb.ToString();
        }
    }
}