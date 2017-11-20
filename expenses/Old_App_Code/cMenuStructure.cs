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
using Spend_Management;

/* 
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 * LEAVE IN THIS PROJECT UNTIL THE EXPENSES MASTER PAGE 'MENU.MASTER' IS NO LONGER IN USE ANYWHERE.
 */

namespace expenses.Old_App_Code
{
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
            string strsql = "select menuid, menu_name, parentid, custom from menu_structure";
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    menuid = reader.GetInt32(reader.GetOrdinal("menuid"));
                    menuname = reader.GetString(reader.GetOrdinal("menu_name"));
                    parentid = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("parentid")))
                    {
                        parentid = reader.GetInt32(reader.GetOrdinal("parentid"));
                    }
                    custom = reader.GetBoolean(reader.GetOrdinal("custom"));
                    menuitems.Add(menuid, new cMenuElement(menuid, menuname, parentid, custom));
                }
                reader.Close();
            }
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                Cache.Insert("menustructure" + accountid, menuitems, null, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Medium),
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

        public void createMenu(int menuid, menu mnu)
        {
            cMenuElement parent;

            lstMenu.TryGetValue(menuid, out parent);
            foreach (cMenuElement child in parent.children)
            {
                if (child.iscustom)
                {
                    mnu.AddMenuItem("exit", 48, child.menuname, child.menuname, "custommenu.aspx?menuid=" + child.menuid);
                }
            }
        }

        public List<ListItem> CreateDropDown()
        {
            SortedList<string, cMenuElement> sorted = sortList();

            List<ListItem> items = new List<ListItem>();

            foreach (cMenuElement element in sorted.Values)
            {
                items.Add(new ListItem(element.menuname, element.menuid.ToString()));
            }
            return items;
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
    }
}
