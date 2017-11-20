using System;

using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Data.SqlClient;

namespace Expenses_Reports
{
    //[Serializable()]
    //public class cFields : cFieldsBase
    //{
    //    private static SortedList<int, SortedList<Guid, cField>> lstfields = new SortedList<int, SortedList<Guid, cField>>();
    //    //private static List<int> accountsCaching;
    //    //public static bool CachingInProgress
    //    //{
    //        //get;
    //        //set;
    //    //}

    //    //private bool setCacheStatus(int accountid)
    //    //{
    //    //    if (accountsCaching == null)
    //    //    {
    //    //        accountsCaching = new List<int>();
    //    //    }

    //    //    if (accountsCaching.Contains(accountid))
    //    //    {
    //    //        return false;
    //    //    }
    //    //    else
    //    //    {
    //    //        accountsCaching.Add(accountid);
    //    //        return true;
    //    //    }
    //    //}

    //    public cFields(int accountid)
    //    {
    //        nAccountid = accountid;
    //        sConnectionString = cAccounts.GetConnectionString(accountid);
    //        sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
    //        clstables = new cTables(accountid);
    //        if (lstfields.ContainsKey(accountid) == false)
    //        {
    //            try
    //            {
    //                //CachingInProgress = true;

    //                fields = CacheFields();
    //                //bool isCachingOwner = setCacheStatus(accountid);

    //                //if (isCachingOwner)
    //                //{
    //                    lstfields.Add(accountid, fields);

    //                    //AggregateCacheDependency aggdep = new AggregateCacheDependency();

    //                    DBConnection fieldsDep = new DBConnection(metabaseconnectionstring);
    //                    string cachestrsql = "select fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, lookuptable, lookupfield, useforlookup from dbo.fields_base";
    //                    fieldsDep.sqlexecute.Notification = null;
    //                    SqlDependency dep = new SqlDependency(fieldsDep.sqlexecute);
    //                    fieldsDep.sqlexecute.CommandText = cachestrsql;
    //                    dep.OnChange += new OnChangeEventHandler(dep_OnChange);
    //                    fieldsDep.ExecuteSQL(cachestrsql);

    //                    DBConnection udfConnection = new DBConnection(connectionstring);
    //                    cachestrsql = "select userdefineid, attribute_name, display_name, fieldtype, specific, mandatory, description, [order], tooltip, maxlength, format, defaultvalue, fieldid, tableid, groupid, archived, [precision], relatedTable from dbo.userdefined";
    //                    udfConnection.sqlexecute.Notification = null;
    //                    SqlDependency udfDep = new SqlDependency(udfConnection.sqlexecute);
    //                    udfConnection.sqlexecute.CommandText = cachestrsql;
    //                    udfDep.OnChange += new OnChangeEventHandler(dep_OnChange);
    //                    udfConnection.ExecuteSQL(cachestrsql);

    //                    DBConnection ceAttConnection = new DBConnection(connectionstring);
    //                    cachestrsql = "select attributeid, attribute_name, display_name, mandatory, maxlength, is_audit_identity, precision, format, defaultvalue, is_key_field, relatedTable from dbo.custom_entity_attributes";
    //                    ceAttConnection.sqlexecute.Notification = null;
    //                    SqlDependency ceAttDep = new SqlDependency(ceAttConnection.sqlexecute);
    //                    ceAttConnection.sqlexecute.CommandText = cachestrsql;
    //                    ceAttDep.OnChange += new OnChangeEventHandler(dep_OnChange);
    //                    ceAttConnection.ExecuteSQL(cachestrsql);

    //                    DBConnection ceConnection = new DBConnection(connectionstring);
    //                    cachestrsql = "select tableid, entity_name, plural_name, description, enableAttachments, enableAudiences, allowdocmergeaccess from dbo.custom_entities";
    //                    ceConnection.sqlexecute.Notification = null;
    //                    SqlDependency ceDep = new SqlDependency(ceConnection.sqlexecute);
    //                    ceConnection.sqlexecute.CommandText = cachestrsql;
    //                    ceDep.OnChange += new OnChangeEventHandler(dep_OnChange);
    //                    ceConnection.ExecuteSQL(cachestrsql);

    //                    DBConnection liConnection = new DBConnection(connectionstring);
    //                    cachestrsql = "SELECT [userdefineid],[item],[order],[valueid] FROM [dbo].[userdefined_list_items]";
    //                    liConnection.sqlexecute.Notification = null;
    //                    SqlDependency liDep = new SqlDependency(liConnection.sqlexecute);
    //                    liConnection.sqlexecute.CommandText = cachestrsql;
    //                    liDep.OnChange += new OnChangeEventHandler(dep_OnChange);
    //                    liConnection.ExecuteSQL(cachestrsql);

    //                //    if (accountsCaching.Contains(accountid))
    //                //    {
    //                //        accountsCaching.Remove(accountid);
    //                //    }
    //                //}
    //            }
    //            finally
    //            {
    //                //CachingInProgress = false;
    //            }
    //        }
    //        else
    //        {
    //            fields = (SortedList<Guid, cField>)lstfields[accountid];
    //        }

    //    }
    //    public void dep_OnChange(object sender, SqlNotificationEventArgs e)
    //    {
    //        lstfields.Remove(accountid);
    //        SqlDependency dep = (SqlDependency)sender;
    //        dep.OnChange -= dep_OnChange;
    //    }

    //}
}

	

