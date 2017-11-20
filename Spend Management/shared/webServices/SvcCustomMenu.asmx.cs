using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using Spend_Management.shared.code.EasyTree;

namespace Spend_Management.shared.webServices
{
    using Newtonsoft.Json;
    using Spend_Management.shared.code.GreenLight;
    using System.Web.Script.Serialization;
    using System;
    using System.Linq;

    using code;
    /// <summary>
    /// Summary description for svcCustomMenu
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SvcCustomMenu : WebService
    {
        /// <summary>
        /// Custom menu id.
        /// </summary>
        [JsonProperty("ID")]
        public string Id { get; set; }

        /// <summary>
        /// Custom menu name.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Custom menu description.
        /// </summary>
        [JsonProperty("Description")]
        public string Description { get; set; }
        /// <summary>
        /// Parent menu id.
        /// </summary>
        [JsonProperty("ParentId")]
        public int ParentId { get; set; }
        /// <summary>
        /// Menu icon name.
        /// </summary>
        [JsonProperty("IconName")]
        public string IconName { get; set; }

        /// <summary>
        /// Menu order.
        /// </summary>
        [JsonProperty("Order")]
        public int Order { get; set; }

        /// <summary>
        /// Reference Parent ID for dynamically created menu items.
        /// </summary>
        [JsonProperty("ReferenceDynamicParentId")]
        public string ReferenceDynamicParentId { get; set; }

        /// <summary>
        /// Current User.
        /// </summary>
        public new CurrentUser User;

        /// <summary>
        /// Custom menu structure object.
        /// </summary>
        public CustomMenuStructure CustomMenuItems;


        /// <summary>
        /// Default constractor.
        /// </summary>
        public SvcCustomMenu()
        {
            this.User = cMisc.GetCurrentUser();
            this.CustomMenuItems = new CustomMenuStructure(this.User.AccountID);
        }

        /// <summary>
        /// Save new custom menu
        /// </summary>
        /// <param name="customMenu"> custom menu details</param>
        [WebMethod(EnableSession = true)]
        public string ManageCustomMenu(List<string> customMenu)
        {
            var failedCustomMenu = new List<string>();
            var menuItemToSave = JObject.Parse(customMenu[0])["New"].ToObject<SvcCustomMenu[]>();
            var menuItemToUpdate = JObject.Parse(customMenu[0])["Edited"].ToObject<SvcCustomMenu[]>();
            var menuItemToDelete = JObject.Parse(customMenu[0])["Deleted"].ToObject<SvcCustomMenu[]>();

            var customMenuList = this.GenerateCustomMenuObject(menuItemToSave, menuItemToUpdate);
            if(customMenuList.Count>0)
            {
                failedCustomMenu = this.CustomMenuItems.AddOrUpdateCustomMenu(customMenuList);
            }

            foreach (var menuItem in menuItemToDelete)
            {
                this.CustomMenuItems.DeleteCustomMenu(Convert.ToInt32(menuItem.Id));
            }
            return  failedCustomMenu.Count > 0?new JavaScriptSerializer().Serialize(string.Join(",", failedCustomMenu.ToArray())) :0.ToString();
        }

        /// <summary>
        /// Check if custom menu has view.
        /// </summary>
        /// <param name="customMenuId">Custom menu id</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int MenuItemHasView(int customMenuId)
        {
            return this.CustomMenuItems.HasView(customMenuId)?1:0;
        }

        /// <summary>
        /// Get the custom menu tree data down to a specified level for the easy tree control.
        /// </summary>
        [WebMethod(EnableSession = true)]
        public List<EasyTreeNode> GetEasyTreeNodesForCustomMenu()
        {
            var customeMenuNodes = new CustomMenuNodes();
          return customeMenuNodes.GetInitialMenuEasyTreeNodes();
        }
        /// <summary>
        /// Generate custom menu item object.
        /// </summary>
        /// <param name="saveItems"></param>
        /// <param name="updateItems"></param>
        /// <returns></returns>
        public List<CustomMenuStructureItem> GenerateCustomMenuObject(SvcCustomMenu[] saveItems, SvcCustomMenu[] updateItems)
        {
            var menuObject = saveItems.Select(item => new CustomMenuStructureItem(0, item.Name, item.ParentId, item.Description, item.IconName, this.User.EmployeeID, null, item.Order, item.Id, item.ReferenceDynamicParentId)).ToList();
            menuObject.AddRange(updateItems.Select(item => new CustomMenuStructureItem(Convert.ToInt32(item.Id), item.Name, item.ParentId, item.Description, item.IconName, null, this.User.EmployeeID, item.Order)));
            return menuObject;
        }

    }
}
