using System;
using System.Web.Caching;
using System.Collections;
using System.Collections.Generic;
using ExpensesLibrary;
using System.Configuration;
using expenses.Old_App_Code;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
namespace expenses
{
    /// <summary>
    /// Summary description for cSystemInfo.
    /// </summary>
    public class cSystemInfo
    {
        public cSystemInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }

    [Serializable()]
    public class cFields : cBaseFields
    {
        
        [NonSerialized()]
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        
        public cFields(int accountid)
        {
            nAccountid = accountid;
            sConnectionString = cAccounts.getConnectionString(accountid);
            sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString;
            clstables = new cTables(accountid);
            fields = (SortedList<Guid,cField>)Cache["fields" + accountid];
            if (fields == null)
            {
                fields = CacheFields();
                string strsql = "select fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, lookuptable, lookupfield, useforlookup from [fields_base]";
                DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
                expdata.sqlexecute.CommandText = strsql;
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("fields" + accountid, fields, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }

        }


        public sOnlineViewgroupInfo getModifiedViewgroups(DateTime date)
        {
            System.Data.SqlClient.SqlDataReader reader;
            System.Collections.SortedList list = new System.Collections.SortedList();
            cTables clstables = new cTables();
            cTable table, lookuptable;
            cField curfield, lookupfield;
            int fieldid, tableid, viewgroupid, width, lookupfieldid;
            string field, fieldtype, description, comment;
            bool normalview, idfield, genlist, cantotal;
            bool valuelist, printout, allowimport, mandatory;
            DateTime amendedon;
            bool useforlookup;
            int lookuptableid;
            SortedList items;
            strsql = "select fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, lookuptable, lookupfield, useforlookup from dbo.[fields]";
            expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                fieldid = reader.GetInt32(reader.GetOrdinal("fieldid"));
                tableid = reader.GetInt32(reader.GetOrdinal("tableid"));
                viewgroupid = reader.GetInt32(reader.GetOrdinal("viewgroupid"));
                width = reader.GetInt32(reader.GetOrdinal("width"));
                field = reader.GetString(reader.GetOrdinal("field"));
                fieldtype = reader.GetString(reader.GetOrdinal("fieldtype"));
                if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                {
                    description = reader.GetString(reader.GetOrdinal("description"));
                }
                else
                {
                    description = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("comment")) == false)
                {
                    comment = reader.GetString(reader.GetOrdinal("comment"));
                }
                else
                {
                    comment = "";
                }
                normalview = reader.GetBoolean(reader.GetOrdinal("normalview"));
                idfield = reader.GetBoolean(reader.GetOrdinal("idfield"));
                genlist = reader.GetBoolean(reader.GetOrdinal("genlist"));
                cantotal = reader.GetBoolean(reader.GetOrdinal("cantotal"));
                valuelist = reader.GetBoolean(reader.GetOrdinal("valuelist"));
                if (valuelist)
                {
                    items = getList(fieldid);
                }
                else
                {

                    items = new SortedList();
                }
                allowimport = reader.GetBoolean(reader.GetOrdinal("allowimport"));
                mandatory = reader.GetBoolean(reader.GetOrdinal("mandatory"));
                printout = reader.GetBoolean(reader.GetOrdinal("printout"));
                if (reader.IsDBNull(reader.GetOrdinal("amendedon")) == true)
                {
                    amendedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    amendedon = reader.GetDateTime(reader.GetOrdinal("amendedon"));
                }
                table = clstables.getTableById(tableid);
                if (reader.IsDBNull(reader.GetOrdinal("lookuptable")) == true)
                {
                    lookuptableid = 0;
                }
                else
                {
                    lookuptableid = reader.GetInt32(reader.GetOrdinal("lookuptable"));
                }
                if (lookuptableid > 0)
                {
                    lookuptable = clstables.getTableById(lookuptableid);
                }
                else
                {
                    lookuptable = null;
                }
                if (reader.IsDBNull(reader.GetOrdinal("lookupfield")) == true)
                {
                    lookupfieldid = 0;
                }
                else{
                    lookupfieldid = reader.GetInt32(reader.GetOrdinal("lookupfield"));
                }
                useforlookup = reader.GetBoolean(reader.GetOrdinal("useforlookup"));
                curfield = new cField(fieldid, tableid, table, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, cantotal, width, valuelist, items,0, allowimport, mandatory, printout, amendedon, lookuptable,lookupfieldid, useforlookup);
                list.Add(fieldid, curfield);
            }
            reader.Close();
            

            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "select * from fields_userdefined";
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                
                fieldid = reader.GetInt32(reader.GetOrdinal("fieldid"));
                tableid = reader.GetInt32(reader.GetOrdinal("tableid"));
                viewgroupid = reader.GetInt32(reader.GetOrdinal("viewgroupid"));
                field = reader.GetString(reader.GetOrdinal("field"));
                fieldtype = reader.GetString(reader.GetOrdinal("fieldtype"));
                switch (fieldid)
                {
                    case 6:
                    case 18:
                    case 27:
                    case 31:
                    case 38:
                    case 42:
                    case 88:
                    case 90:
                    case 247:
                        description = getCustomDescription(fieldid);
                        if (description == "")
                        {
                            description = reader.GetString(reader.GetOrdinal("description"));
                        }
                        break;
                    default:

                        if (reader.IsDBNull(reader.GetOrdinal("description")) == true)
                        {
                            description = "";
                        }
                        else
                        {
                            description = reader.GetString(reader.GetOrdinal("description"));
                        }
                        break;
                }
                if (reader.IsDBNull(reader.GetOrdinal("comment")) == true)
                {
                    comment = "";
                }
                else
                {
                    comment = reader.GetString(reader.GetOrdinal("comment"));
                }
                normalview = reader.GetBoolean(reader.GetOrdinal("normalview"));
                genlist = reader.GetBoolean(reader.GetOrdinal("genlist"));
                width = reader.GetInt32(reader.GetOrdinal("width"));
                cantotal = reader.GetBoolean(reader.GetOrdinal("cantotal"));
                printout = reader.GetBoolean(reader.GetOrdinal("printout"));
                
                table = clstables.getTableById(tableid);
                curfield = new cField(fieldid, tableid, table, field, fieldtype, description, comment, normalview, false, viewgroupid, genlist, cantotal, width, false, null,accountid, true,false, printout, new DateTime(1900, 01, 01), null, 0, false);
                list.Add(fieldid, curfield);
            }
            reader.Close();
            fields = list;
            //update the primary keys

            for (int i = 0; i < clstables.list.Count; i++)
            {
                table = (cTable)clstables.list.GetByIndex(i);
                if (table.primarykey != 0)
                {
                    curfield = getFieldById(table.primarykey);
                    table.setPrimarykeyfield(curfield);
                }
                if (table.keyfield != 0)
                {
                    curfield = getFieldById(table.keyfield);
                    table.setKeyfield(curfield);
                }
            }

            foreach (cField clsfield in fields.Values)
            {
                if (clsfield.lookupfieldid > 0)
                {
                    clsfield.lookupfield = getFieldById(clsfield.lookupfieldid);
                }
            }
            Cache.Insert("fields" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

            return list;
        }


        public string getCustomDescription(int fieldid)
        {
            string description = "";
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "select description from addscreen where fieldid = @fieldid";
            expdata.sqlexecute.Parameters.AddWithValue("@fieldid", fieldid);
            System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    description = reader.GetString(0);
                }
            }
            reader.Close();

            return description;
        }
        public cField getFieldById(int fieldid)
        {
            return (cField)fields[fieldid];
        }

        public SortedList<string, cField> getFieldsByViewGroup(int id)
        {
            SortedList<string, cField> lst = new SortedList<string,cField>();

            foreach (cField field in fields.Values)
            {
                if (field.viewgroupid == id && field.width > 0 && ! lst.ContainsKey(field.description))
                {
                    lst.Add(field.description, field);
                }
            }
            return lst;
        }
        public SortedList<string, string> getFieldsForImport(int tableid, bool withLookup)
        {
            int userdefinedtableid = 0;

            switch (tableid)
            {
                case 21: //employees
                    userdefinedtableid = 50;
                    break;
            }
            cFields clsfields = new cFields(accountid);
            List<cField> lookupfields;
            SortedList<string,string> lst = new SortedList<string,string>();
            foreach (cField field in fields.Values)
            {
                if ((field.tableid == tableid || field.tableid == userdefinedtableid) && field.allowimport)
                {
                    if (field.lookuptable != null)
                    {
                        if (withLookup)
                        {
                            //add lookup fields
                            lookupfields = clsfields.getLookupFields(field.lookuptable.tableid);
                            foreach (cField lookupfield in lookupfields)
                            {
                                lst.Add(field.description + "<lookup using " + lookupfield.description + ">", field.fieldid + "," + lookupfield.fieldid);
                            }
                        }
                    }
                    else
                    {
                        lst.Add(field.description, field.fieldid.ToString());
                    }
                }
            }
            return lst;
        }
        public List<cField> getFieldsForImport(int tableid)
        {
            cFields clsfields = new cFields(accountid);
            List<cField> lookupfields;
            List<cField> lst = new List<cField>();
            foreach (cField field in fields.Values)
            {
                if (field.tableid == tableid && field.allowimport)
                {
                    if (field.lookuptable == null)
                    {
                        lst.Add(field);
                    }
                }
            }
            return lst;
        }

        public List<cField> getPrintoutFields()
        {
            List<cField> lst = new List<cField>();
            foreach (cField field in fields.Values)
            {
                if (field.printout)
                {
                    lst.Add(field);
                }
            }
            return lst;
        }

        public cField getFieldByName(string name)
        {
            int i;
            cField reqfield;

            for (i = 0; i < fields.Count; i++)
            {
                reqfield = (cField)fields.GetByIndex(i);
                if (reqfield.description == name)
                {
                    return reqfield;
                }
            }
            return null;
        }
        public cField getUserDefinedFieldByName(int accountid, string name)
        {
            int i;
            cField reqfield;

            for (i = 0; i < fields.Count; i++)
            {
                reqfield = (cField)fields.GetByIndex(i);
                if (reqfield.description == name && reqfield.accountid == accountid)
                {
                    return reqfield;
                }
            }
            return null;
        }
        private SortedList getList(int fieldid)
        {

            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
            cViewGroup vgroup;
            sOnlineViewgroupInfo viewgroupinfo = new sOnlineViewgroupInfo();
            Dictionary<int, cViewGroup> lstviewgroups = new Dictionary<int, cViewGroup>();
            List<int> viewgroupids = new List<int>();
            int viewgroupid, parentid, level;
            string groupname;
            DateTime amendedon;

            string strsql = "SELECT * FROM viewgroups WHERE amendedon > @date";
            expdata.sqlexecute.Parameters.AddWithValue("@date", date);
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                viewgroupid = reader.GetInt32(reader.GetOrdinal("viewgroupid"));
                groupname = reader.GetString(reader.GetOrdinal("groupname"));
                if (reader.IsDBNull(reader.GetOrdinal("parentid")) == true)
                {
                    parentid = 0;
                }
                else
                {
                    parentid = reader.GetInt32(reader.GetOrdinal("parentid"));
                }
                
                level = reader.GetInt32(reader.GetOrdinal("level"));
                if (reader.IsDBNull(reader.GetOrdinal("amendedon")) == true)
                {
                    amendedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    amendedon = reader.GetDateTime(reader.GetOrdinal("amendedon"));
                }

                vgroup = new cViewGroup(viewgroupid, groupname, parentid, level, amendedon);

                lstviewgroups.Add(viewgroupid, vgroup);
            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();

            strsql = "SELECT viewgroupid FROM viewgroups";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                viewgroupids.Add(reader.GetInt32(reader.GetOrdinal("viewgroupid")));
            }
            reader.Close();
            viewgroupinfo.lstonlineviewgroups = lstviewgroups;
            viewgroupinfo.lstviewgroupids = viewgroupids;

            return viewgroupinfo;
        }
    }

    [Serializable()]
    public class cTables : cBaseTables
    {

        [NonSerialized()]
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public cTables(int accountid)
        {
            sConnectionString = cAccounts.getConnectionString(accountid);
            nAccountid = accountid;
            tables = (SortedList<Guid,cTable>)Cache["tables" + accountid];
            if (tables == null)
            {
                tables = CacheTables();

                string strsql = "SELECT     tableid, tablename, jointype, allowreporton, description, primarykey, allowimport, keyfield, amendedon, allowworkflow, allowentityrelationship FROM tables_base";
                DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
                expdata.sqlexecute.CommandText = strsql;
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("tables" + accountid, tables, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }

        }

        

    }

    public class cJoins
    {
        private Dictionary<int, cJoin> lstJoins;
        DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
        string strsql;
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        private int nAccountid;

        public cJoins(int accountid)
        {
            nAccountid = accountid;
            lstJoins = (Dictionary<int, cJoin>)Cache["joins"];
            if (lstJoins == null)
            {
                lstJoins = CacheJoins();
            }
        }

        private Dictionary<int, cJoin> CacheJoins()
        {
            int jointableid, basetableid, destinationtableid;
            string description;
            System.Data.SqlClient.SqlDataReader reader;
            Dictionary<int, cJoin> lst = new Dictionary<int, cJoin>();
            DateTime amendedon;
            strsql = "select jointableid, tableid, basetableid, description, amendedon from dbo.jointables";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);
            cJoin join;
            while (reader.Read())
            {
                jointableid = reader.GetInt32(reader.GetOrdinal("jointableid"));
                basetableid = reader.GetInt32(reader.GetOrdinal("basetableid"));
                destinationtableid = reader.GetInt32(reader.GetOrdinal("tableid"));
                if (reader.IsDBNull(reader.GetOrdinal("description")) == true)
                {
                    description = "";
                }
                else
                {
                    description = reader.GetString(reader.GetOrdinal("description"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("amendedon")) == true)
                {
                    amendedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    amendedon = reader.GetDateTime(reader.GetOrdinal("amendedon"));
                }
                join = new cJoin(accountid, jointableid, basetableid, destinationtableid, description, getJoinSteps(jointableid), amendedon);
                lst.Add(jointableid, join);
            }
            reader.Close();
            Cache.Insert("joins", lst, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            return lst;
        }

        private SortedList<int, cJoinStep> getJoinSteps(int jointableid)
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;
            cJoinStep step;
            cTables clstables = new cTables();
            cFields clsfields = new cFields(accountid);
            cTable destinationtable, sourcetable;
            cField joinkey, destinationkey;
            SortedList<int, cJoinStep> lst = new SortedList<int, cJoinStep>();
            int joinbreakdownid, sourcetableid, destinationtableid, joinkeyid, destinationkeyid;
            byte order;
            DateTime amendedon;
            strsql = "select * from joinbreakdown where jointableid = @jointableid";
            expdata.sqlexecute.Parameters.AddWithValue("@jointableid", jointableid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                joinbreakdownid = reader.GetInt32(reader.GetOrdinal("joinbreakdownid"));
                sourcetableid = reader.GetInt32(reader.GetOrdinal("sourcetable"));
                destinationtableid = reader.GetInt32(reader.GetOrdinal("tableid"));
                joinkeyid = reader.GetInt32(reader.GetOrdinal("joinkey"));
                order = reader.GetByte(reader.GetOrdinal("order"));
                if (reader.IsDBNull(reader.GetOrdinal("amendedon")) == true)
                {
                    amendedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    amendedon = reader.GetDateTime(reader.GetOrdinal("amendedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("destinationkey")) == true)
                {
                    destinationkeyid = 0;
                }
                else
                {
                    destinationkeyid = reader.GetInt32(reader.GetOrdinal("destinationkey"));
                }
                sourcetable = clstables.getTableById(sourcetableid);
                destinationtable = clstables.getTableById(destinationtableid);
                joinkey = clsfields.getFieldById(joinkeyid);
                destinationkey = clsfields.getFieldById(destinationkeyid);
                step = new cJoinStep(joinbreakdownid, jointableid, order, destinationtable, sourcetable, joinkey, amendedon, destinationkey, accountid);
                lst.Add(order, step);
            }
            reader.Close();

            return lst;
        }

        public void getJoinSQL(ref List<string> joins, int jointableid, int AccountID)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cTables clstables = new cTables();
            cFields clsfields = new cFields(accountid);
            cField fldjoinkey;
            cField destinationkey;
            cTable sourcetable;
            cTable destinationtable;

            cJoin join = getJoinTableById(jointableid);
            SortedList<int, cJoinStep> lstSteps = join.steps;

            foreach (cJoinStep step in lstSteps.Values)
            {
                step.isReport = false;
                if (joins.Contains(step.SQL(AccountID)) == false)
                {
                    joins.Add(step.SQL(AccountID));
                }

            }

        }

        public string createJoinSQL(List<cField> flds, UserView userview, bool defaultView)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            List<int> tableids = new List<int>();
            cJoin join;
            //get list of tables
            
            int basetable;

            if (defaultView)
            {
                basetable = 40;
            }
            else
            {
                if (userview == UserView.Previous)
                {
                    basetable = 73;
                }
                else
                {
                    basetable = 72;
                }
            }
            foreach (cField fld in flds)
            {

                if (fld.fieldtype != "FD" && fld.fieldtype != "FS")
                {
                    if (tableids.Contains(fld.table.tableid) == false && fld.table.tableid != basetable)
                    {
                        tableids.Add(fld.table.tableid);
                    }
                }

            }
            List<string> joins = new List<string>();

            foreach (int tbl in tableids)
            {
                join = getJoinByTable(basetable,tbl);
                getJoinSQL(ref joins, join.jointableid, accountid);
            }

            foreach (string strjoin in joins)
            {
                output.Append(strjoin);
            }
            return output.ToString() ;
        }

        public string createJoinSQL(List<cField> flds, int basetable, int AccountID)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            List<int> tableids = new List<int>();
            cJoin join;
            //get list of tables

            foreach (cField fld in flds)
            {

                if (fld.fieldid != 88 && fld.fieldid != 90 && fld.fieldid != 247 || ((fld.fieldid == 88 || fld.fieldid == 90 || fld.fieldid == 247) && basetable != 40 && basetable != 72 && basetable != 73))
                {
                    if (tableids.Contains(fld.table.tableid) == false && fld.table.tableid != basetable)
                    {
                        tableids.Add(fld.table.tableid);
                    }
                }

            }
            List<string> joins = new List<string>();

            foreach (int tbl in tableids)
            {
                join = getJoinByTable(basetable,tbl);
                getJoinSQL(ref joins, join.jointableid, AccountID);
            }

            foreach (string strjoin in joins)
            {
                output.Append(strjoin);
            }
            return output.ToString();
        }

        private cJoin getJoinByTable(int basetable, int destinationtable)
        {
            foreach (cJoin join in lstJoins.Values)
            {
                if (join.basetableid == basetable && join.destinationtableid == destinationtable)
                {
                    return join;
                }
            }
            return null;
        }

        public cJoin getJoinTableById(int id)
        {
            cJoin join;
            lstJoins.TryGetValue(id, out join);
            return join;
        }

        public sOnlineJoinInfo getModifiedJoins(DateTime date)
        {
            sOnlineJoinInfo joininfo = new sOnlineJoinInfo();
            Dictionary<int, cJoin> lstjoins = new Dictionary<int, cJoin>();
            List<int> joinids = new List<int>();
            Dictionary<int, cJoinStep> lstjoinsteps = new Dictionary<int, cJoinStep>();
            List<int> joinstepids = new List<int>();

            foreach (cJoin join in lstJoins.Values)
            {
                if (join.amendedon > date)
                {
                    lstjoins.Add(join.jointableid, join);
                }

                joinids.Add(join.jointableid);

                foreach (cJoinStep step in join.steps.Values)
                {
                    if (step.amendedon > date)
                    {
                        lstjoinsteps.Add(step.joinbreakdownid, step);
                    }

                    joinstepids.Add(step.joinbreakdownid);
                }

            }

            joininfo.lstonlinejoins = lstjoins;
            joininfo.lstjoinids = joinids;
            joininfo.lstonlinejoinsteps = lstjoinsteps;
            joininfo.lstjoinstepids = joinstepids;

            return joininfo;
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
    }
}
