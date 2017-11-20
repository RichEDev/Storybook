

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using expenses;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using expenses.Old_App_Code.admin;
using SpendManagementLibrary;

/// <summary>
/// Summary description for cNewUserDefined
/// </summary>
public class cNewUserDefined
{
    string strsql;
    int nAcountid = 0;

    private System.Collections.SortedList list;
    private System.Collections.SortedList fieldDefinitions;
    private System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

    public cNewUserDefined(int accountid)
    {
        nAcountid = accountid;
        InitialiseData();
    }

    #region properties
    public int accountid
    {
        get { return nAcountid; }
    }
    #endregion
    private void InitialiseData()
    {

        list = (System.Collections.SortedList)Cache["userdefinedfields" + accountid];
        if (Cache["userdefinedfields" + accountid] == null)
        {
            list = CacheList();
        }
        
    }

    private System.Collections.SortedList CacheList()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        int maxorder = 1;
        System.Collections.SortedList list = new System.Collections.SortedList();
        System.Data.SqlClient.SqlDataReader reader;
        cUserDefinedField requserdef;
        int userdefineid, order;
        Guid tableid;
        string tooltip;
        string label, description;
        cAttribute attribute;
        FieldType fieldtype;
        cTables clstables = new cTables(accountid);
        cTable table;
        bool specific, mandatory;
        strsql = "select  userdefineid, attribute_name, fieldtype, tableid, specific, mandatory, description, other, [order], CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, tooltip from dbo.userdefined order by [order]";
        int createdby, modifiedby;
        DateTime createdon, modifiedon;
      
        expdata.sqlexecute.CommandText = strsql;
        SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
        reader = expdata.GetReader(strsql);
        while (reader.Read())
        {
            attribute = null;
            userdefineid = reader.GetInt32(reader.GetOrdinal("userdefineid"));
            label = reader.GetString(reader.GetOrdinal("attribute_name"));
            if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
            {
                description = reader.GetString(reader.GetOrdinal("description"));
            }
            else
            {
                description = "";
            }
            fieldtype = (FieldType)reader.GetByte(reader.GetOrdinal("fieldtype"));
            tableid = reader.GetGuid(reader.GetOrdinal("tableid"));
            table = clstables.getTableById(tableid);
            specific = reader.GetBoolean(reader.GetOrdinal("specific"));
            mandatory = reader.GetBoolean(reader.GetOrdinal("mandatory"));
            order = maxorder;
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
            if (reader.IsDBNull(reader.GetOrdinal("tooltip")) == true)
            {
                tooltip = "";
            }
            else
            {
                tooltip = reader.GetString(reader.GetOrdinal("tooltip"));
            }
            lstItems.TryGetValue(userdefineid, out listitems);
            if (listitems == null)
            {
                listitems = new SortedList<int, cListItem>();
            }
            lstSubcats.TryGetValue(userdefineid, out subcats);
            if (subcats == null)
            {
                subcats = new List<int>();
            }
            requserdef = new cUserDefinedField(userdefineid, label, description, fieldtype, appliesto, specific, mandatory, order, listitems, subcats, createdon, createdby, modifiedon, modifiedby, tooltip);
            switch (fieldtype)
            {
                case FieldType.Text:
                    attribute = new cTextAttribute(userdefineid, label, label, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 0, AttributeFormat.SingleLine);
                    break;
            }
            requserdef = new cUserDefinedField(userdefineid, table, order, getSubcats(userdefineid), createdon, createdby, modifiedon, modifiedby, attribute);
            list.Add(userdefineid, requserdef);
            maxorder++;
        }
        reader.Close();
        

        Cache.Insert("userdefinedfields" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30), System.Web.Caching.CacheItemPriority.NotRemovable,null);
        return list;
    }

    public void addItemsToPage(ref Table tbl, cTable table, bool itemspecific, string subcatid, Dictionary<int, object> values, List<int> udfs )
    {
        TableRow row;
        TableCell cell;
        TextBox txtbox;
        RequiredFieldValidator reqval;
        CompareValidator compval;
        CustomValidator custval;
        DropDownList ddlst;
        object value;
        string valid = "";
        cUserDefinedField field;
        string id;
        for (int i = 0; i < list.Count; i++)
        {
            field = (cUserDefinedField)list.GetByIndex(i);
            if (field.table == table && (table.tablename != "savedexpenses" || (table.tablename == "savedexpenses" && itemspecific == false && field.itemspecific == false) || (table.tablename == "savedexpenses" && itemspecific == true && udfs.Contains(field.userdefineid))))
            {
                id = "";
                valid = "";
                row = new TableRow();
                cell = new TableCell();
                cell.CssClass = "labeltd";
                cell.Text = field.label;
                if (field.label.Substring(field.label.Length - 1, 1) != ":")
                {
                    cell.Text += ":";
                }
                row.Cells.Add(cell);
                cell = new TableCell();
                cell.CssClass = "inputtd";
                switch (field.fieldtype)
                {
                    case FieldType.Text:
                    case FieldType.Currency:
                    case FieldType.Number:
                    case FieldType.DateTime:
                        txtbox = new TextBox();
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += subcatid;
                        }
                        txtbox.ID = id;
                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                txtbox.Text = value.ToString();
                            }
                        }
                        cell.Controls.Add(txtbox);
                        
                        break;
                    case FieldType.List:
                        ddlst = new DropDownList();
                        id = "cmbudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += subcatid;
                        }
                        ddlst.ID = id;
                        foreach (string listitem in field.items.Values)
                        {
                            ddlst.Items.Add(new ListItem(listitem));
                        }
                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                if (ddlst.Items.FindByValue(value.ToString()) != null)
                                {
                                    ddlst.Items.FindByValue(value.ToString()).Selected = true;
                                }
                            }
                        }

                        cFilterRules clsfilterrules = new cFilterRules(accountid);

                        if (field.table.tablename == "savedexpenses" && field.itemspecific == false)
                        {
                            clsfilterrules.filterDropdown(ref ddlst, FilterType.Userdefined, "");
                        }
                        else if (field.table.tablename == "savedexpenses" && field.itemspecific)
                        {
                            clsfilterrules.filterDropdown(ref ddlst, FilterType.Userdefined, subcatid);
                        }

                        cell.Controls.Add(ddlst);
                        break;
                    case FieldType.TickBox:
                        ddlst = new DropDownList();
                        id = "cmbudf" + field.userdefineid;
                        
                        if (itemspecific)
                        {
                            id += subcatid;
                        }
                        ddlst.ID = id;
                        ddlst.Items.Add(new ListItem("Yes", "yes"));
                        ddlst.Items.Add(new ListItem("No", "no"));
                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                if (ddlst.Items.FindByValue(value.ToString()) != null)
                                {
                                    ddlst.Items.FindByValue(value.ToString()).Selected = true;
                                }
                            }
                        }
                        cell.Controls.Add(ddlst);
                        break;
                }
                row.Cells.Add(cell);

                if (field.mandatory && (field.fieldtype != FieldType.List && field.fieldtype != FieldType.TickBox))
                {
                    cell = new TableCell();
                    if (field.mandatory)
                    {

                        if (itemspecific)
                        {
                            custval = new CustomValidator();
                            custval.ID = "custmand" + field.label.Substring(0, 2) + subcatid;
                            custval.ControlToValidate = id;
                            custval.ClientValidationFunction = "checkMandatory";
                            custval.ErrorMessage = field.label + " is a mandatory field. Please enter a value in the box provided.";
                            custval.Text = "*";
                            custval.ValidateEmptyText = true;
                            cell.Controls.Add(custval);

                        }
                        else
                        {
                            reqval = new RequiredFieldValidator();
                            valid = "requdf" + field.userdefineid;
                            if (itemspecific)
                            {
                                valid += subcatid;
                            }
                            reqval.ID = valid;

                            reqval.ControlToValidate = id;
                            reqval.ErrorMessage = field.label + " is a mandatory field. Please enter a value in the box provided.";
                            reqval.Text = "*";
                            cell.Controls.Add(reqval);
                        }
                    }
                }
                switch (field.fieldtype)
                {
                    case FieldType.Currency:
                        compval = new CompareValidator();
                        valid = "compudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            valid += subcatid;
                        }
                        compval.ID = valid;
                        compval.ControlToValidate = id;
                        compval.Type = ValidationDataType.Currency;
                        compval.ErrorMessage = "The value you have entered for " + field.label + " is invalid. Valid characters are the numbers 0-9 and a full stop (.)";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        cell.Controls.Add(compval);
                        break;
                    case FieldType.Number:
                        compval = new CompareValidator();
                        valid = "compudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            valid += subcatid;
                        }
                        compval.ID = valid;
                        compval.ControlToValidate = id;
                        compval.Type = ValidationDataType.Double;
                        compval.ErrorMessage = "The value you have entered for " + field.label + " is invalid. Valid characters are the numbers 0-9 and a full stop (.)";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        cell.Controls.Add(compval);
                        break;
                    case FieldType.DateTime:
                        compval = new CompareValidator();
                        valid = "compudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            valid += subcatid;
                        }
                        compval.ID = valid;
                        compval.ControlToValidate = id;
                        compval.Type = ValidationDataType.Date;
                        compval.ErrorMessage = "The value you have entered for " + field.label + " is invalid. Valid characters are the numbers 0-9 and a forward slash (/)";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        cell.Controls.Add(compval);
                        break;

                }

                    row.Cells.Add(cell);
                    if (field.tooltip != "")
                    {
                            Literal lit = new Literal();
                            lit.Text = "<td><img id=\"imgtooltipudf" + field.userdefineid + "\" onclick=\"showUserDefinedTooltip('imgtooltipudf" + field.userdefineid + "'," + field.userdefineid + "," + accountid + ");\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/></td>";
                            cell.Controls.Add(lit);
                            row.Cells.Add(cell);
                    }
                tbl.Rows.Add(row);
            }
        }
    }
    private SortedList<int, List<int>> getSubcats()
    {

        SortedList<int, List<int>> lst = new SortedList<int, List<int>>();
        List<int> subcats;
        int userdefineid, subcatid;
        
        int i = 0;
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        string strsql;
        System.Data.SqlClient.SqlDataReader reader;

        strsql = "select subcatid, userdefineid from subcats_userdefined";
        reader = expdata.GetReader(strsql);
        while (reader.Read())
        {
            subcatid = reader.GetInt32(0);
            userdefineid = reader.GetInt32(1);
            lst.TryGetValue(userdefineid, out subcats);
            if (subcats == null)
            {
                subcats = new List<int>();
                lst.Add(userdefineid, subcats);
            }
            subcats.Add(subcatid);
        }
        reader.Close();


        return lst;
    }
    private SortedList<int,SortedList<int, cListItem>> getListItems()
    {

        SortedList<int, SortedList<int, cListItem>> lst = new SortedList<int, SortedList<int, cListItem>>();
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        string strsql = "";
        System.Data.SqlClient.SqlDataReader reader;
        int userdefineid;
        int itemid;

        //if (blankListItemExists(userdefineid) == 0)
        //{
        //    addListItem(userdefineid, "", "", 0);
        //}

        string item, comment;
        cListItem reqitem;
        SortedList<int, cListItem> items;
        int createdby, modifiedby;
        DateTime createdon, modifiedon;


        strsql = "select * from defineditems";
        reader = expdata.GetReader(strsql);
        while (reader.Read())
        {
            userdefineid = reader.GetInt32(reader.GetOrdinal("userdefineid"));
            itemid = reader.GetInt32(reader.GetOrdinal("itemid"));
            item = reader.GetString(reader.GetOrdinal("item"));
            if (reader.IsDBNull(reader.GetOrdinal("comment")) == false)
            {
                comment = reader.GetString(reader.GetOrdinal("comment"));
            }
            else
            {
                comment = "";
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
            lst.TryGetValue(userdefineid, out items);
            if (items == null)
            {
                items = new SortedList<int, cListItem>();
                lst.Add(userdefineid, items);
            }
            
            reqitem = new cListItem(userdefineid, itemid, item, comment, createdon, createdby, modifiedon, modifiedby);
            items.Add(itemid, reqitem);
        }
        reader.Close();
        


        return lst;

    }
    public Dictionary<int, object> getItemsFromPage(ref Table tbl, cTable table, bool itemspecific, string subcatid)
    {
        
        Dictionary<int, object> values = new Dictionary<int, object>();
        TextBox txtbox;
        
        DropDownList ddlst;
        object value;
        cUserDefinedField field;
        string id;
        for (int i = 0; i < list.Count; i++)
        {
            field = (cUserDefinedField)list.GetByIndex(i);
            if (field.table == table && (table.tablename != "savedexpenses" || (table.tablename == "savedexpenses" && itemspecific == field.itemspecific)))
            {
                id = "";
                
                switch (field.fieldtype)
                {
                    case FieldType.Text:
                    case FieldType.Currency:
                    case FieldType.Number:
                    case FieldType.DateTime:
                        
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += subcatid;
                        }
                        txtbox = (TextBox)tbl.FindControl(id);
                        if (txtbox != null)
                        {
                            values.Add(field.userdefineid, txtbox.Text);
                        }
                        

                        break;
                    case FieldType.List:
                    case FieldType.TickBox:
                        id = "cmbudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += subcatid;
                        }
                        ddlst = (DropDownList)tbl.FindControl(id);
                        if (ddlst != null)
                        {
                            values.Add(field.userdefineid, ddlst.SelectedValue);
                        }
                        break;
                }
                
            }
        }

        return values;
    }
    public System.Data.DataTable getGrid()
    {
        cUserDefinedField requserdef;
        object[] values;
        System.Data.DataTable tbl = new System.Data.DataTable();
        System.Collections.SortedList sorted = new System.Collections.SortedList();
        int i;

        for (i = 0; i < list.Count; i++)
        {
            requserdef = (cUserDefinedField)list.GetByIndex(i);
            sorted.Add(requserdef.label, requserdef);
        }

        tbl.Columns.Add("userdefineid", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("label", System.Type.GetType("System.String"));
        tbl.Columns.Add("description", System.Type.GetType("System.String"));
        tbl.Columns.Add("fieldtype", System.Type.GetType("System.Byte"));
        tbl.Columns.Add("appliesto", System.Type.GetType("System.Byte"));
        tbl.Columns.Add("specific", System.Type.GetType("System.Boolean"));
        tbl.Columns.Add("mandatory", System.Type.GetType("System.Boolean"));

        for (i = 0; i < sorted.Count; i++)
        {
            requserdef = (cUserDefinedField)sorted.GetByIndex(i);
            values = new object[7];
            values[0] = requserdef.userdefineid;
            values[1] = requserdef.label;
            values[2] = requserdef.description;
            values[3] = requserdef.fieldtype;
            values[4] = requserdef.appliesto;
            values[5] = requserdef.itemspecific;
            values[6] = requserdef.mandatory;
            tbl.Rows.Add(values);
        }
        return tbl;
    }

    public int addUserDefined(string label, FieldType fieldtype, AppliesTo appliesto, bool itemspecific, bool mandatory, string description, int order, int userid, string tooltip)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        if (checkExistance(label) == true)
        {
            return 1;
        }
        DateTime createdon = DateTime.Now.ToUniversalTime();
        int userdefineid;
        
        expdata.sqlexecute.Parameters.AddWithValue("@label", label);
        expdata.sqlexecute.Parameters.AddWithValue("@fieldtype", (byte)fieldtype);
        expdata.sqlexecute.Parameters.AddWithValue("@appliesto", (byte)appliesto);
        expdata.sqlexecute.Parameters.AddWithValue("@specific", Convert.ToByte(itemspecific));
        expdata.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(mandatory));
        expdata.sqlexecute.Parameters.AddWithValue("@description", description);
        expdata.sqlexecute.Parameters.AddWithValue("@order", order);
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters.AddWithValue("@tooltip", tooltip);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        strsql = "insert into userdefined (label, fieldtype, appliesto, specific, mandatory, description, [order], createdon, createdby, tooltip) " +
            "values (@label,@fieldtype,@appliesto,@specific,@mandatory,@description,@order, @createdon, @createdby, @tooltip);select @identity = @@identity";
        expdata.ExecuteSQL(strsql);
        userdefineid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();

        //cUserDefinedField clsfield = new cUserDefinedField(userdefineid, label, description, fieldtype, appliesto, itemspecific, mandatory, order, new SortedList<int, cListItem>(), new int[] { }, createdon, userid, new DateTime(1900, 01, 01), 0, tooltip);
        //list.Add(userdefineid, clsfield);

        

        
        return 0;

    }

    private bool checkExistance(string label)
    {
        int i;
        cUserDefinedField requserdef;
        for (i = 0; i < list.Count; i++)
        {
            requserdef = (cUserDefinedField)list.GetByIndex(i);
            if (requserdef.label == label)
            {
                return true;
            }
        }

        return false;

    }

    public List<cUserDefinedField> getFieldsByTable(cTable table)
    {
        List<cUserDefinedField> fields = new List<cUserDefinedField>();
        foreach (cUserDefinedField field in list.Values)
        {
            if (field.table == table)
            {
                fields.Add(field);
            }
        }
        return fields;
    }

    public List<cUserDefinedField> getFieldsByAppliesToAndType(cTable table, FieldType type)
    {
        List<cUserDefinedField> fields = new List<cUserDefinedField>();
        foreach (cUserDefinedField field in list.Values)
        {
            if (field.table == table && field.fieldtype == type)
            {
                fields.Add(field);
            }
        }
        return fields;
    }



    public void updateUserdefined(int userdefineid, string label, string description, bool itemspecific, bool mandatory, int order, int userid, string tooltip)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        DateTime modifiedon = DateTime.Now.ToUniversalTime();
        strsql = "update userdefined set [label] = @label, description = @description, specific = @specific, mandatory = @mandatory, [order] = @order, modifiedon = @modifiedon, modifiedby = @modifiedby, tooltip = @tooltip " +
            "where userdefineid = @userdefineid";
        expdata.sqlexecute.Parameters.AddWithValue("@label", label);
        expdata.sqlexecute.Parameters.AddWithValue("@specific", Convert.ToByte(itemspecific));
        expdata.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(mandatory));
        expdata.sqlexecute.Parameters.AddWithValue("@description", description);
        expdata.sqlexecute.Parameters.AddWithValue("@order", order);
        expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", userdefineid);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@tooltip", tooltip);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        cUserDefinedField oldfield = getUserDefinedById(userdefineid);
        //cUserDefinedField newfield = new cUserDefinedField(userdefineid, label, description, oldfield.fieldtype, oldfield.appliesto, itemspecific, mandatory, order, oldfield.items, oldfield.selectedSubcats, new DateTime(1900, 01, 01), 0, modifiedon, userid, tooltip);

        //updateFieldLabel(oldfield, newfield);
        if (list[userdefineid] != null)
        {
            //list[userdefineid] = newfield;
        }
    }

    
    public cUserDefinedField getUserDefinedById(int userdefineid)
    {
        if (list != null)
        {
            return (cUserDefinedField)list[userdefineid];
        }
        else
        {
            return null;
        }
    }

    public void deleteUserDefined(int userdefineid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
        string companyid;
        string label;
        cAccounts clsaccounts = new cAccounts();
        
        cUserDefinedField requserdefined;
        requserdefined = getUserDefinedById(userdefineid);
        expenses.cMisc clsmisc = new expenses.cMisc(accountid);
        cAccount reqaccount = clsaccounts.getAccountById(accountid) ;

        
        companyid = reqaccount.companyid;



        

        //get the label

        label = requserdefined.label;

        strsql = "delete from userdefined where userdefineid = @userdefineid";
        expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", userdefineid);
        expdata.ExecuteSQL(strsql);

        expdata.sqlexecute.Parameters.Clear();
        

        list.Remove(userdefineid);

    }

    

    public Dictionary<int, object> getValues(AppliesTo appliesto,int id)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        Dictionary<int, object> values = new Dictionary<int, object>();
        System.Data.SqlClient.SqlDataReader reader;

        int userdefineid;
        object value;
        strsql = "select userdefined_values.userdefineid, userdefined_values.value from userdefined_values inner join userdefined on userdefined.userdefineid = userdefined_values.userdefineid where userdefined_values.recordid = @id and userdefined.appliesto = @appliesto";
       
        expdata.sqlexecute.Parameters.AddWithValue("@id", id);
        expdata.sqlexecute.Parameters.AddWithValue("@appliesto", (byte)appliesto);
        

        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            userdefineid = reader.GetInt32(0);
            value = reader.GetValue(1);
            values.Add(userdefineid, value);
        }
        reader.Close();
        return values;
    }

    private void deleteValues(AppliesTo appliesto, int id)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from userdefined_values where userdefined_values.recordid = @id and userdefined_values.userdefineid in (select userdefineid from userdefined where appliesto = @appliesto)";

        expdata.sqlexecute.Parameters.AddWithValue("@id", id);
        expdata.sqlexecute.Parameters.AddWithValue("@appliesto", (byte)appliesto);
        
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }

    public void addValues (AppliesTo appliesto, int id, Dictionary<int, object> values)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        deleteValues(appliesto, id);
        if (values != null)
        {
            foreach (KeyValuePair<int, object> value in values)
            {
                strsql = "insert into userdefined_values (userdefineid, recordid, value) " +
                    "values (@userdefineid, @recordid, @value)";
                expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", value.Key);
                expdata.sqlexecute.Parameters.AddWithValue("@value", value.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@recordid", id);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }
    }

    public int getNextOrder()
    {
        int order = 0;
        foreach (cUserDefinedField field in list.Values)
        {
            if (field.order > order)
            {
                order++;
            }
        }
       
        order++;
        return order;
    }

    public sOnlineUserdefinedInfo getModifiedUserdefinedFields(DateTime date)
    {
        sOnlineUserdefinedInfo onlineUserdefined = new sOnlineUserdefinedInfo();
        Dictionary<int, cUserDefinedField> lstUserdefined = new Dictionary<int, cUserDefinedField>();
        List<int> lstUserdefinedids = new List<int>();
        Dictionary<int, cListItem> lstListitems = new Dictionary<int, cListItem>();
        List<int> lstListitemids = new List<int>();

        foreach (cUserDefinedField val in list.Values)
        {
            if (val.createdon > date || val.modifiedon > date)
            {
                lstUserdefined.Add(val.userdefineid, val);
            }

            lstUserdefinedids.Add(val.userdefineid);

            //**todo**
            //foreach (string item in val.items.Values)
            //{
            //    if (item.createdon > date || item.modifiedon > date)
            //    {
            //        lstListitems.Add(item.itemid, item);
            //    }

            //    lstListitemids.Add(item.itemid);
            //}
        }

        onlineUserdefined.lstUserdefined = lstUserdefined;
        onlineUserdefined.lstUserdefinedids = lstUserdefinedids;
        onlineUserdefined.lstListitems = lstListitems;
        onlineUserdefined.lstListitemids = lstListitemids;

        return onlineUserdefined;
    }

    #region list items
    public int addListItem(int userdefineid, string item, string comment, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        int itemid;
        DateTime createdon = DateTime.Now.ToUniversalTime();
        expdata.sqlexecute.Parameters.Clear();
        expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", userdefineid);
        expdata.sqlexecute.Parameters.AddWithValue("@item", item);
        expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        strsql = "insert into defineditems (userdefineid, item, comment, createdon, createdby) " +
            "values (@userdefineid,@item,@comment, @createdon, @createdby);select @identity = @@identity";
        expdata.ExecuteSQL(strsql);
        itemid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();
        cUserDefinedField field = getUserDefinedById(userdefineid);
        if (field != null)
        {
            //field.items.Add(itemid, new cListItem(userdefineid, itemid, item, comment, createdon, userid, new DateTime(1900, 01, 01), 0));
        }
        return 0;
    }



    public int updateListItem(int userdefineid, int itemid, string item, string comment, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        DateTime modifiedon = DateTime.Now.ToUniversalTime();
        expdata.sqlexecute.Parameters.AddWithValue("@item", item);
        expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
        expdata.sqlexecute.Parameters.AddWithValue("@itemid", itemid);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        strsql = "update defineditems set item = @item, comment = @comment, modifiedon = @modifiedon, modifiedby = @modifiedby where itemid = @itemid";
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
        cUserDefinedField field = getUserDefinedById(userdefineid);
        if (field.items.ContainsKey(itemid))
        {
            //field.items[itemid] = new cListItem(field.userdefineid,itemid,item,comment, new DateTime(1900, 01, 01), 0, modifiedon, userid);
        }
        return 0;
    }
    public void deleteListItem(int userdefineid, int itemid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        expdata.sqlexecute.Parameters.AddWithValue("@itemid", itemid);
        strsql = "delete from defineditems where itemid = " + itemid;
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
        cUserDefinedField field = getUserDefinedById(userdefineid);
        field.items.Remove(itemid);
    }

    public int blankListItemExists(int userdefineid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        expdata.sqlexecute.Parameters.Clear();
        strsql = "SELECT COUNT(*) FROM dbo.defineditems WHERE item = '' AND userdefineid = @userdefineid;";
        expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", userdefineid);
        return expdata.getcount(strsql);
    }
    #endregion
}


