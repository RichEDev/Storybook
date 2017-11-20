using System;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web.Caching;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for categories.
    /// </summary>
    public class cCategories
    {
        private string strsql;
        private int nAccountid = 0;
        private System.Collections.SortedList list;
        private System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        //System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpContext.Current.Cache;

        public cCategories(int accountid)
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


        public List<cCategory> CachedList()
        {
            List<cCategory> lstCats = new List<cCategory>();
            foreach(cCategory cat in list.Values)
            {
                if(lstCats.Contains(cat) == false)
                {
                    lstCats.Add(cat);
                }
            }

            return lstCats;
        }

        public System.Data.DataSet getGrid()
        {
            System.Collections.SortedList sorted = sortList();
            cCategory category;
            object[] values;
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();
            tbl.Columns.Add("categoryid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("category", System.Type.GetType("System.String"));
            tbl.Columns.Add("description", System.Type.GetType("System.String"));

            for(int i = 0; i < sorted.Count; i++)
            {
                category = (cCategory) sorted.GetByIndex(i);
                values = new object[3];
                values[0] = category.categoryid;
                values[1] = category.category;
                values[2] = category.description;
                tbl.Rows.Add(values);
            }
            ds.Tables.Add(tbl);
            return ds;
        }

        private System.Collections.SortedList CacheList()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int categoryid, createdby, modifiedby;
            DateTime createdon, modifiedon;
            string category, description;
            cCategory newcat;
            System.Collections.SortedList list = new System.Collections.SortedList();

            strsql = "select categoryid, category, description, createdon, createdby, modifiedon, modifiedby from dbo.categories";
            expdata.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("categories" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
            }

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    categoryid = reader.GetInt32(reader.GetOrdinal("categoryid"));
                    category = reader.GetString(reader.GetOrdinal("category"));
                    if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                    {
                        description = reader.GetString(reader.GetOrdinal("description"));
                    }
                    else
                    {
                        description = "";
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                    {
                        modifiedon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                    {
                        modifiedby = 0;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    newcat = new cCategory(categoryid, category, description, createdon, createdby, modifiedon, modifiedby);
                    list.Add(categoryid, newcat);

                }
                reader.Close();
            }

            return list;
        }

        private System.Collections.SortedList sortList()
        {
            System.Collections.SortedList sorted = new System.Collections.SortedList();
            cCategory category;

            for(int i = 0; i < list.Count; i++)
            {
                category = (cCategory) list.GetByIndex(i);
                sorted.Add(category.category, category);
            }
            return sorted;
        }



        public int addCategory(string category, string description, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cAuditLog clsaudit = new cAuditLog(accountid, userid);
            if(checkExistance(category, 0, 1) == true)
            {
                return 1;
            }
            int categoryid;

            DateTime createdon = DateTime.Now.ToUniversalTime();

            expdata.addParameter("@categoryid", 0);
            expdata.addParameter("@category", category);
            if(description.Length > 3999)
            {
                expdata.addParameter("@description", description.Substring(0, 3999));
            }
            else
            {
                expdata.addParameter("@description", description);
            }
            expdata.addParameter("@date", createdon);

            expdata.addParameter("@userid", userid);
            categoryid = expdata.executeSaveCommand("saveCategory");

            //add entry to audit log
            clsaudit.addRecord(SpendManagementElement.ExpenseCategories, category, categoryid);

            cCategory newcat = new cCategory(categoryid, category, description, createdon, userid, new DateTime(1900, 01, 01), 0);
            list.Add(categoryid, newcat);
            return categoryid;
        }

        private bool checkExistance(string category, int categoryid, int action)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int count = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@category", category);


            if(action == 2)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@categoryid", categoryid);
                strsql = "select count(*) from categories where categoryid <> @categoryid and category = @category";
            }
            else
            {
                strsql = "select count(*) from categories where category = @category";
            }
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();
            if(count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Updates the ExpenseCategory.
        /// </summary>
        /// <param name="categoryid">The Id of the category to update.</param>
        /// <param name="category">The Label for the category.</param>
        /// <param name="description">The description of the category.</param>
        /// <param name="userid">The user Id of the user making this call.</param>
        /// <returns>A success or failure code.</returns>
        public int updateCategory(int categoryid, string category, string description, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            if(checkExistance(category, categoryid, 2) == true)
            {
                return 1;
            }
            cCategory reqcat = FindById(categoryid);

            cAuditLog clsaudit = new cAuditLog(accountid, userid);

            DateTime modifiedon = DateTime.Now.ToUniversalTime();

            expdata.addParameter("@category", category);
            if(description.Length > 3999)
            {
                expdata.addParameter("@description", description.Substring(0, 3999));
            }
            else
            {
                expdata.addParameter("@description", description);
            }
            expdata.addParameter("@categoryid", categoryid);
            expdata.addParameter("@date", modifiedon);
            expdata.addParameter("@userid", userid);

            expdata.executeSaveCommand("saveCategory");


            if(reqcat.category != category)
            {
                clsaudit.editRecord(categoryid, reqcat.category, SpendManagementElement.ExpenseCategories, new Guid("44C2A53A-DB33-45AF-AF66-D0055AAD48EC"), reqcat.category, category);
            }
            if(reqcat.description != description)
            {

                clsaudit.editRecord(categoryid, reqcat.category, SpendManagementElement.ExpenseCategories, new Guid("0744BEC1-5C4D-4B5E-90CF-8D3D9F187DBF"), reqcat.description, description);
            }
            list[categoryid] = new cCategory(categoryid, category, description, reqcat.createdon, reqcat.createdby, modifiedon, userid);

            return 0;
        }

        public int deleteCategory(int categoryid, ICurrentUser currentUser = null)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int returncode;
            cCategory reqcat = FindById(categoryid);
            expdata.addParameter("@categoryid", categoryid);
            returncode = expdata.executeSaveCommand("deleteCategory");

            if (returncode != 0)
            {
                return returncode;
            }

            cAuditLog clsaudit = (currentUser == null)
                ? new cAuditLog()
                : new cAuditLog(currentUser.AccountID, currentUser.EmployeeID);

            clsaudit.deleteRecord(SpendManagementElement.ExpenseCategories, categoryid, reqcat.category, currentUser);
            list.Remove(categoryid);
            return 0;
        }


        private void InitialiseData()
        {
            list = (System.Collections.SortedList) Cache["categories" + accountid];
            if(list == null)
            {
                list = CacheList();
            }

        }


        public cCategory FindById(int categoryid)
        {
            return (cCategory) list[categoryid];

        }

        public void CreateStringDropDown(ref System.Text.StringBuilder output, int categoryid, int roleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@categoryid", categoryid);
            expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);

            if(roleid == 0)
            {
                strsql = "select category, categories.categoryid from categories order by category";
            }
            else
            {
                strsql = "select distinct category, categories.categoryid from categories inner join subcats on categories.categoryid = subcats.categoryid left join rolesubcats on rolesubcats.subcatid = subcats.subcatid where roleid = @roleid order by category";
            }

            using (System.Data.SqlClient.SqlDataReader catreader = expdata.GetReader(strsql))
            {
                output.Append("<option value=0 ");
                if (categoryid == 0)
                {
                    output.Append("selected");
                }
                output.Append(">Please select an option</option>");

                while (catreader.Read())
                {
                    output.Append("<option value=\"" + catreader.GetInt32(catreader.GetOrdinal("categoryid")) + "\"");
                    if (categoryid == catreader.GetInt32(catreader.GetOrdinal("categoryid")))
                    {
                        output.Append("selected");
                    }
                    output.Append(">" + catreader.GetString(catreader.GetOrdinal("category")) + "</option>");
                }
                catreader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
        }


        public System.Web.UI.WebControls.ListItem[] CreateDropDown(int roleid)
        {
            System.Web.UI.WebControls.ListItem[] tempItems;

            tempItems = CacheDropDown(roleid);



            return tempItems;
        }



        private System.Web.UI.WebControls.ListItem[] CacheDropDown(int roleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string[] dependency;


            expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
            
            int count = 0;
            if(roleid == 0)
            {
                strsql = "select count(*) from categories";
                dependency = new string[1];
                dependency[0] = "categoriesdependency" + accountid;
            }
            else
            {
                strsql = "select count(distinct categories.categoryid) as mycount from categories inner join subcats on categories.categoryid = subcats.categoryid inner join rolesubcats on rolesubcats.subcatid = subcats.subcatid where roleid = @roleid";
                dependency = new string[2];
                dependency[0] = "categoriesdependency" + accountid;
                dependency[1] = "rolesdependency" + accountid;
            }
            count = expdata.getcount(strsql);
            System.Web.UI.WebControls.ListItem[] tempItems = new System.Web.UI.WebControls.ListItem[count + 1];

            if(roleid == 0)
            {
                strsql = "select category, categories.categoryid from categories order by category";
            }
            else
            {
                strsql = "select distinct category, categories.categoryid from categories inner join subcats on categories.categoryid = subcats.categoryid left join rolesubcats on rolesubcats.subcatid = subcats.subcatid where roleid = @roleid order by category";
            }

            using (System.Data.SqlClient.SqlDataReader catreader = expdata.GetReader(strsql))
            {
                int i = 0;

                tempItems[0] = new System.Web.UI.WebControls.ListItem();
                tempItems[0].Text = "Please select an option";
                tempItems[0].Value = "0";


                while (catreader.Read())
                {
                    tempItems[i + 1] = new System.Web.UI.WebControls.ListItem();
                    tempItems[i + 1].Text = catreader.GetString(catreader.GetOrdinal("category"));
                    tempItems[i + 1].Value = catreader.GetInt32(catreader.GetOrdinal("categoryid")).ToString();
                    tempItems[i + 1].Selected = false;

                    i++;
                }
                catreader.Close();
            }

            System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(null, dependency);
            if(roleid == 0)
            {
                //	Cache.Insert("dropdowncategories" + accountid,tempItems,dep);
            }
            else
            {
                //	Cache.Insert("dropdowncategories" + accountid + roleid,tempItems,dep);
            }
            expdata.sqlexecute.Parameters.Clear();
            return tempItems;
        }

        public Dictionary<int, cCategory> getModifiedCategories(DateTime date)
        {
            Dictionary<int, cCategory> lst = new Dictionary<int, cCategory>();
            foreach(cCategory cat in list.Values)
            {
                if(cat.createdon > date || cat.modifiedon > date)
                {
                    lst.Add(cat.categoryid, cat);
                }
            }
            return lst;
        }

        public List<int> getCategoryIds()
        {
            List<int> ids = new List<int>();
            foreach(cCategory val in list.Values)
            {
                ids.Add(val.categoryid);
            }
            return ids;
        }

        public cCategory getCategoryByName(string name)
        {
            cCategory reqCategory = null;
            foreach(cCategory val in list.Values)
            {
                if(val.category == name)
                {
                    reqCategory = val;
                    break;
                }
            }
            return reqCategory;
        }

        /// <summary>
        /// Force removal of cached records for the current account ID
        /// </summary>
        public void refreshCache()
        {
            Cache.Remove("categories" + accountid);
        }

        /// <summary>
        ///  Gets a SortedList of categories from a List of <see cref="SubcatItemRoleBasic"> SubcatItemRoleBasic</see>
        /// </summary>
        /// <param name="roleitems">The <see cref="SubcatItemRoleBasic"> SubcatItemRoleBasic</see></param>
        /// <returns></returns>
        public SortedList<string,int> GetExpenseCategoriesFromItemRoles(List<SubcatItemRoleBasic> roleitems)
        {
            var categories = new SortedList<string, int>();
      
            foreach (SubcatItemRoleBasic rolesub in roleitems)
            {
                cCategory category = this.FindById(rolesub.CategoryId);

                if (!categories.ContainsValue(category.categoryid))
                {
                    categories.Add(category.category, category.categoryid);
                }
            }

            return categories;
        }

        /// <summary>
        ///  Builds up a List of ListItems from a List of <see cref="SubcatItemRoleBasic"> SubcatItemRoleBasic</see>. Used for the categories drop down list 
        /// </summary>
        /// <param name="roleitems">The <see cref="SubcatItemRoleBasic"> SubcatItemRoleBasic</see></param>
        /// <returns></returns>
        public List<ListItem> PopulateCategoriesDropDownList(List<SubcatItemRoleBasic> roleitems)
        {
            var expenseCategories = GetExpenseCategoriesFromItemRoles(roleitems);
            return expenseCategories.Select(expenseCategory => new ListItem(expenseCategory.Key, expenseCategory.Value.ToString())).ToList();
        }
    }
}
