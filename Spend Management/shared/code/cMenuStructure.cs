using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Globalization;

    using BusinessLogic.Modules;

    public class cMenuStructure
    {
        SortedList<int, cMenuElement> lstMenu;
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        private int nAccountid;

        public cMenuStructure(int accountid)
        {
            nAccountid = accountid;
            InitialiseData();
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion

        private void InitialiseData()
        {
            lstMenu = (SortedList<int, cMenuElement>)Cache["menustructure" + accountid];
            if (lstMenu == null)
            {
                lstMenu = CacheList();
            }
        }

        private SortedList<int, cMenuElement> CacheList()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SortedList<int, cMenuElement> menuitems = new SortedList<int, cMenuElement>();
            SqlDataReader reader;
            int menuid;
            string menuname;
            int? parentid;
            bool custom;
            string strsql = "SELECT CustomMenuId ,name ,ParentMenuId ,CAST((1 ^ SystemMenu)AS BIT) as custom FROM CustomMenu";
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    menuid = reader.GetInt32(reader.GetOrdinal("CustomMenuId"));
                    menuname = reader.GetString(reader.GetOrdinal("name"));
                    parentid = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("ParentMenuId")))
                    {
                        parentid = reader.GetInt32(reader.GetOrdinal("ParentMenuId"));
                    }
                    custom = reader.GetBoolean(reader.GetOrdinal("custom"));
                    menuitems.Add(menuid, new cMenuElement(menuid, menuname, parentid, custom));
                }
                reader.Close();
            }
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                Cache.Insert("menustructure" + accountid, menuitems, null, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Permanent),
                    CacheItemPriority.Default, null);
            }
            associateChildrenAndParent(menuitems);
            return menuitems;
        }

        private void associateChildrenAndParent(SortedList<int, cMenuElement> lstMenu)
        {
            cMenuElement parent;
            foreach (cMenuElement element in lstMenu.Values)
            {
                if (element.parentid != null)
                {
                    lstMenu.TryGetValue((int)element.parentid, out parent);
                    if (parent != null)
                    {
                        element.setParent(parent);
                        parent.addChild(element);
                    }
                }
            }
        }

        public void createMenu(int menuid, smMenu mnu)
        {
            cMenuElement parent;

            lstMenu.TryGetValue(menuid, out parent);
            foreach (cMenuElement child in parent.children)
            {
                if (child.iscustom)
                {
                    mnu.addMenuItem("exit", 48, child.menuname, child.menuname, "custommenu.aspx?menuid=" + child.menuid);
                }
            }
        }

        /// <summary>
        /// The create drop down.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ListItem> CreateDropDown()
        {
            SortedList<string, cMenuElement> sorted = this.sortList();

            ICurrentUser curUser = cMisc.GetCurrentUser();
            var showPolicyInfo = curUser.CurrentActiveModule == Modules.Expenses
                                 || curUser.CurrentActiveModule == Modules.Greenlight
                                 || curUser.CurrentActiveModule == Modules.GreenlightWorkforce;

            var list = new List<ListItem>();
            foreach (cMenuElement element in sorted.Values)
            {
                if (element.menuname != "Policy Information" || showPolicyInfo)
                {
                    list.Add(new ListItem(element.menuname, element.menuid.ToString(CultureInfo.InvariantCulture)));
                }
            }

            return list;
        }

        private SortedList<string, cMenuElement> sortList()
        {
            SortedList<string, cMenuElement> sorted = new SortedList<string, cMenuElement>();

            foreach (cMenuElement element in lstMenu.Values)
            {
                sorted.Add(element.menuname, element);
            }
            return sorted;
        }

        public cMenuElement getMenuItemById(int menuid)
        {
            if (lstMenu.ContainsKey(menuid))
            {
                return lstMenu[menuid];
            }
            return null;
        }
    }
}
