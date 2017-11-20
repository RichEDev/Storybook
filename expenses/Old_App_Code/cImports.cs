using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using ExpensesLibrary;
using System.Data.SqlClient;
using SpendManagementLibrary;

namespace expenses.Old_App_Code
{
    public class cImports
    {

    }

    [Serializable()]
    public class cImport
    {

        protected bool bValidFile = true;
        protected string strsql;
        protected int nAccountid;
        protected byte[] bFile;
        protected List<List<object>> oFileContents = new List<List<object>>();
        protected List<cImportField> lstMatchingGrid;
        protected SortedList<Guid, string> lstDefaultValues;
        private Guid nTableid;
        protected int nHeaderCount;
        protected bool bHasFooter;
        protected int nFooterCount;

        protected string[] arrHeaderRow;
        protected string[] arrFooterRow;
        protected List<int> lstUsedColumnIndexes = new List<int>();

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public byte[] filedata
        {
            get { return bFile; }
        }
        public Guid tableid
        {
            get { return nTableid; }
            set { nTableid = value; }
        }
        public bool hasheader
        {
            get 
            {
                if (headercount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public int headercount
        {
            get { return nHeaderCount; }
            set { nHeaderCount = value; }
        }
        public List<List<object>> filecontents
        {
            get { return oFileContents; }
        }
        public bool hasfooter
        {
            get { return bHasFooter; }
            set { bHasFooter = value; }
        }
        public int footercount
        {
            get { return nFooterCount; }
        }
        
        public string[] headerrow
        {
            get { return arrHeaderRow; }
        }
        public string[] footerrow
        {
            get { return arrFooterRow; }
        }
        public bool isvalidfile
        {
            get { return bValidFile; }
        }

        public List<int> usedColumnIndexes
        {
            get { return lstUsedColumnIndexes; }
        }
        #endregion

        public void setMatchingGrid(Guid tableid, List<cImportField> grid)
        {
            nTableid = tableid;
            lstMatchingGrid = grid;
        }

        public void setDefaultValues(SortedList<Guid, string> values)
        {
            lstDefaultValues = values;
        }
        public virtual DataTable getSample()
        {
            object[] values;
            int samplecount;
            
            DataTable tbl = new DataTable();
            for (int i = 0; i < arrHeaderRow.GetLength(0); i++)
            {
                if (hasheader)
                {
                    if (tbl.Columns[arrHeaderRow[i]] != null)
                    {
                        tbl.Columns.Add(arrHeaderRow[i] + "_" + i, typeof(System.String));
                    }
                    else
                    {
                        tbl.Columns.Add(arrHeaderRow[i], typeof(System.String));
                    }
                }
                else
                {
                    tbl.Columns.Add("Column " + (i + 1), typeof(System.String));
                }
            }

            if (oFileContents.Count > 30)
            {
                samplecount = 30;
            }
            else
            {
                samplecount = oFileContents.Count;
            }

            
            for (int i = 0; i < samplecount; i++)
            {
                values = new object[tbl.Columns.Count];
                for (int column = 0; column < tbl.Columns.Count; column++)
                {
                    values[column] = oFileContents[i][column];
                }
                tbl.Rows.Add(values);
            }

            return tbl;
        }
        private void checkFieldsOnlyUsedOnce(ref List<string> errors)
        {
            List<cImportField> temp = new List<cImportField>();
            cFields clsfields = new cFields(accountid);
            cField reqfield;
            Guid id;
            foreach (cImportField i in lstMatchingGrid)
            {
                if (temp.Contains(i) && i.destinationcolumn != Guid.Empty)
                {
                    
                    if (i.lookupcolumn != Guid.Empty)
                    {
                        id = i.lookupcolumn;
                    }
                    else
                    {
                        id = i.destinationcolumn;
                    }
                    reqfield = clsfields.getFieldById(id);
                    errors.Add(reqfield.description + " has been matched to more than one column in the source file.");
                }
                else
                {
                    temp.Add(i);
                }
            }
        }

        public DataTable getColumnsForDefaultGrid()
        {
            cFields clsfields = new cFields(accountid);
            List<cField> fields = clsfields.getFieldsForImport(tableid);
            List<cField> unmatchedfields = new List<cField>();
            bool containsField = false;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("column", typeof(System.Guid));
            tbl.Columns.Add("defaultvalue", typeof(System.String));
            foreach (cField field in fields)
            {
                containsField = false;
                foreach (cImportField importfield in lstMatchingGrid)
                {
                    
                    if (importfield.destinationcolumn == field.fieldid && field.lookupfield == null)
                    {
                        containsField = true;
                        break;
                    }
                }
                if (!containsField && field.lookupfield == null)
                {
                    unmatchedfields.Add(field);
                }
            }
            
            foreach (cField field in unmatchedfields)
            {
                tbl.Rows.Add(new object[] { field.fieldid, "" });
            }
            return tbl;
        }
        private void allMandatoryFieldsSelected(ref List<string> errors)
        {
            cFields clsfields = new cFields(accountid);
            List<cField> fields = clsfields.getFieldsByTable(tableid);
            bool matchedtolookup;
            bool containsfield;
            foreach (cField field in fields)
            {
                matchedtolookup = false;
                if (field.mandatory)
                {
                    if (field.lookuptable != null)
                    {
                        List<cField> lookupfields = clsfields.getLookupFields(field.lookuptable.tableid);
                        foreach (cField lookupfield in lookupfields)
                        {
                            foreach (cImportField importfield in lstMatchingGrid)
                            {
                                if ((importfield.lookupcolumn == lookupfield.fieldid && importfield.destinationcolumn == field.fieldid) || importfield.defaultvalue != "")
                                {
                                    matchedtolookup = true;
                                    break;
                                }
                            }
                        }
                        if (!matchedtolookup)
                        {
                            errors.Add(field.description + " is a mandatory field but has not been matched to a column in the source file.");
                        }
                    }
                    else
                    {
                        containsfield = false;
                        
                        foreach (cImportField importfield in lstMatchingGrid)
                        {
                            if (importfield.destinationcolumn == field.fieldid)
                            {
                                containsfield = true;                                
                            }
                        }
                        if (!containsfield)
                        {
                            errors.Add(field.description + " is a mandatory field but has not been matched to a column in the source file.");
                        }
                    }
                }
            }
        }

        
        public List<string> validateImport()
        {
            cFields clsfields = new cFields(accountid);
            cField typefield;
            List<string> errors = new List<string>();
            List<Guid> mandatory = getMandatoryFields();
            string[] arrid;
            Guid id;

            allMandatoryFieldsSelected(ref errors);
            checkFieldsOnlyUsedOnce(ref errors);
            

            for (int i = 0; i < oFileContents.Count; i++)
            {
                for (int x = 0; x < oFileContents[i].Count; x++)
                {
                    
                    id = lstMatchingGrid[x].destinationcolumn;
                    if (mandatory.Contains(id))
                    {
                        //check to see whether a value exists
                        if (oFileContents[i][x].ToString().Length == 0)
                        {
                            errors.Add("Row " + i + ", Column " + x + " cannot be imported. The column is mandatory and a value is expected.");
                        }
                    }
                    //check type
                    if (lstMatchingGrid[x].lookupcolumn != Guid.Empty)
                    {
                        typefield = clsfields.getFieldById(lstMatchingGrid[x].lookupcolumn);
                        if (getLookupValue(typefield, oFileContents[i][x]) == null)
                        {
                            errors.Add("Row " + i + ", Column " + x + " cannot be imported. The column does not have an associated lookup value.");
                        }
                    }
                    else
                    {
                        typefield = clsfields.getFieldById(id);
                        if (typefield != null)
                        {
                            switch (typefield.fieldtype)
                            {
                                case "N": //Integer
                                    try
                                    {
                                        int.Parse(oFileContents[i][x].ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.description + " is an Integer field. " + oFileContents[i][x] + " cannot be converted to an integer value.");
                                    }
                                    break;
                                case "C": //currency
                                    try
                                    {
                                        decimal.Parse(oFileContents[i][x].ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.description + " is a Currency field. " + oFileContents[i][x] + " cannot be converted to a currency value.");
                                    }
                                    break;
                                case "X":
                                    try
                                    {
                                        Convert.ToBoolean(oFileContents[i][x].ToString());
                                    }
                                    catch
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.description + " is a Boolean field. " + oFileContents[i][x] + " cannot be converted to a boolean value.");
                                    }
                                    break;
                                case "D": //date
                                    try
                                    {
                                        DateTime.Parse(oFileContents[i][x].ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.description + " is a Date field. " + oFileContents[i][x] + " cannot be converted to a date value.");
                                    }
                                    break;
                                case "S":
                                    //check length
                                    if (oFileContents[i][x].ToString().Length > typefield.width)
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. The value of this field can be no more than " + typefield.width + " characters. \"" + oFileContents[i][x] + "\" is " + oFileContents[i][x].ToString().Length + " characters.");
                                    }
                                    break;
                            }
                        }
                    }
                    
                }
            }

            return errors;
        }

        private List<Guid> getMandatoryFields()
        {
            cFields clsfields = new cFields(accountid);
            List<Guid> mandatory = new List<Guid>();
            Guid id;
            foreach (cImportField s in lstMatchingGrid)
            {
                
                id = s.destinationcolumn;
                if (id != Guid.Empty)
                {
                    if (clsfields.getFieldById(id).mandatory)
                    {
                        mandatory.Add(id);
                    }
                }
            }
            return mandatory;
        }
        public DataTable getMatchingGrid()
        {

            DataTable tbl = new DataTable();
            tbl.Columns.Add("sourcecolumn", typeof(System.String));
            tbl.Columns.Add("destinationcolumn", typeof(System.String));
            tbl.Columns.Add("defaultvalue", typeof(System.String));
            Guid id;

            for (int i = 0; i < arrHeaderRow.GetLength(0); i++)
            {
                try
                {
                    if (lstMatchingGrid != null)
                    {
                        id = lstMatchingGrid[i].destinationcolumn;
                    }
                    else
                    {
                        id = Guid.Empty;
                    }
                    if (hasheader)
                    {
                        tbl.Rows.Add(new object[] { arrHeaderRow[i], id });

                    }
                    else
                    {
                        if (oFileContents.Count > 0)
                        {
                            tbl.Rows.Add(oFileContents[0][i], id);
                        }
                        else
                        {
                            tbl.Rows.Add(new object[] { "Column " + (i + 1), id });
                        }


                    }
                }
                catch
                {
                    break;
                }
            }

            return tbl;
        }

        public virtual List<string> importData()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cFields clsfields = new cFields(accountid);
            cField currentfield;
            cField keyfield = getKeyField();
            int recordcount;
            int keyindex = getKeyFieldIndex(keyfield);
            int lookupfieldid;
            string[] arrid;
            Guid id;
            int recordid;
            List<string> status = new List<string>();
            string strsql;
            for (int i = 0; i < oFileContents.Count; i++)
            {
                //does the record already exist?
                if (keyindex == -1)
                {
                    recordcount = 0;
                }
                else
                {
                    strsql = "select count(*) from [" + keyfield.table.tablename + "] where [" + keyfield.table.tablename + "].[" + keyfield.field + "] = @value";
                    expdata.sqlexecute.Parameters.AddWithValue("@value", oFileContents[i][keyindex]);
                    recordcount = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
                if (recordcount > 0) //record exists so update
                {

                    strsql = "update [" + keyfield.table.tablename + "] set ";
                    for (int x = 0; x < oFileContents[i].Count; x++)
                    {
                        
                        id = lstMatchingGrid[x].destinationcolumn;
                        currentfield = clsfields.getFieldById(id);
                        if (id != keyfield.fieldid && id != Guid.Empty)
                        {
                            strsql += "[" + keyfield.table.tablename + "].[" + currentfield.field + "] = @value" + x + ",";
                        }
                        if (lstMatchingGrid[x].lookupcolumn != Guid.Empty) //lookup
                        {
                            cField lookupfield = clsfields.getFieldById(lstMatchingGrid[x].lookupcolumn);
                            int? lookupvalue = getLookupValue(lookupfield, oFileContents[i][x]);
                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, lookupvalue);

                        }
                        else if (id != Guid.Empty && keyfield.fieldid != id)
                        {
                            if (currentfield.isValuelist)
                            {
                                expdata.sqlexecute.Parameters.AddWithValue("@value" + x, currentfield.listitems[oFileContents[i][x].ToString()]);
                            }
                            else
                            {
                                switch (currentfield.fieldtype)
                                {
                                    case "S":
                                    case "FS":
                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, oFileContents[i][x].ToString());
                                        break;
                                    case "N":
                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, int.Parse(oFileContents[i][x].ToString()));
                                        break;
                                    case "C":
                                    case "M":
                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, decimal.Parse(oFileContents[i][x].ToString()));
                                        break;
                                    case "D":
                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, DateTime.Parse(oFileContents[i][x].ToString()));
                                        break;
                                    case "X":
                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, Convert.ToByte(Convert.ToBoolean(oFileContents[i][x].ToString())));
                                        break;
                                }
                            }
                        }
                    }
                    foreach (KeyValuePair<Guid, string> defaultvalue in lstDefaultValues)
                    {
                        currentfield = clsfields.getFieldById(defaultvalue.Key);
                        strsql += "[" + keyfield.table.tablename + "].[" + currentfield.field + "] = @defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key) + ",";
                        if (currentfield.isValuelist)
                        {
                            expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), currentfield.listitems[defaultvalue.Value]);
                        }
                        else
                        {
                            switch (currentfield.fieldtype)
                            {
                                case "S":
                                case "FS":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), defaultvalue.Value);
                                    break;
                                case "N":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), int.Parse(defaultvalue.Value));
                                    break;
                                case "C":
                                case "M":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), decimal.Parse(defaultvalue.Value));
                                    break;
                                case "D":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), DateTime.Parse(defaultvalue.Value));
                                    break;
                                case "X":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), Convert.ToByte(Convert.ToBoolean(defaultvalue.Value)));
                                    break;
                            }
                        }
                    }
                    strsql = strsql.Remove(strsql.Length - 1, 1);
                    strsql += " where [" + keyfield.table.tablename + "].[" + keyfield.field + "] = @keyvalue";
                    expdata.sqlexecute.Parameters.AddWithValue("@keyvalue", oFileContents[i][keyindex]);
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    strsql = "select [" + keyfield.table.tablename + "].[" + keyfield.table.primarykeyfield.field + "] from [" + keyfield.table.tablename + "] where [" + keyfield.field + "] = @keyvalue";
                    expdata.sqlexecute.Parameters.AddWithValue("@keyvalue", oFileContents[i][keyindex]);
                    recordid = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
                else //record doesn't exist, insert new
                {
                    strsql = "insert into [" + keyfield.table.tablename + "] (";
                    foreach (cImportField field in lstMatchingGrid)
                    {
                        id = field.destinationcolumn;
                        if (id != Guid.Empty)
                        {
                            currentfield = clsfields.getFieldById(id);
                            strsql += "[" + keyfield.table.tablename + "].[" + currentfield.field + "],";
                        }
                    }
                    foreach (KeyValuePair<Guid, string> defaultvalue in lstDefaultValues)
                    {
                        currentfield = clsfields.getFieldById(defaultvalue.Key);
                         
                        strsql += "[" + keyfield.table.tablename + "].[" + currentfield.field + "],";
                    }
                    strsql = strsql.Remove(strsql.Length - 1, 1);
                    strsql += ") values (";
                    for (int x = 0; x < oFileContents[i].Count; x++)
                    {
                        
                        
                        id = lstMatchingGrid[x].destinationcolumn;
                        if (id != Guid.Empty)
                        {
                            strsql += "@value" + x + ",";
                        }
                        if (lstMatchingGrid[x].lookupcolumn != Guid.Empty) //lookup
                        {
                            cField lookupfield = clsfields.getFieldById(lstMatchingGrid[x].lookupcolumn);
                            int? lookupvalue = getLookupValue(lookupfield, oFileContents[i][x]);
                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, lookupvalue);
                        }
                        else
                        {
                            if (id != Guid.Empty)
                            {

                                currentfield = clsfields.getFieldById(id);
                                if (currentfield.isValuelist)
                                {
                                    expdata.sqlexecute.Parameters.AddWithValue("@value" + x, currentfield.listitems[oFileContents[i][x].ToString()]);
                                }
                                else
                                {
                                    switch (currentfield.fieldtype)
                                    {
                                        case "S":
                                        case "FS":
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, oFileContents[i][x].ToString());
                                            break;
                                        case "N":
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, int.Parse(oFileContents[i][x].ToString()));
                                            break;
                                        case "C":
                                        case "M":
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, decimal.Parse(oFileContents[i][x].ToString()));
                                            break;
                                        case "D":
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, DateTime.Parse(oFileContents[i][x].ToString()));
                                            break;
                                        case "X":
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, Convert.ToByte(Convert.ToBoolean(oFileContents[i][x].ToString())));
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    foreach (KeyValuePair<Guid, string> defaultvalue in lstDefaultValues)
                    {

                        strsql += "@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key) + ",";
                        currentfield = clsfields.getFieldById(defaultvalue.Key);
                        if (currentfield.isValuelist)
                        {
                            expdata.sqlexecute.Parameters.AddWithValue("@value" + lstDefaultValues.IndexOfKey(defaultvalue.Key), currentfield.listitems[defaultvalue.Value]);
                        }
                        else
                        {
                            switch (currentfield.fieldtype)
                            {
                                case "S":
                                case "FS":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), defaultvalue.Value);
                                    break;
                                case "N":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), int.Parse(defaultvalue.Value));
                                    break;
                                case "C":
                                case "M":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), decimal.Parse(defaultvalue.Value));
                                    break;
                                case "D":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), DateTime.Parse(defaultvalue.Value));
                                    break;
                                case "X":
                                    expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + lstDefaultValues.IndexOfKey(defaultvalue.Key), Convert.ToByte(Convert.ToBoolean(defaultvalue.Value)));
                                    break;
                            }
                        }

                    }
                    strsql = strsql.Remove(strsql.Length - 1, 1);
                    strsql += ");select @identity = scope_identity();";
                    expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                    expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
                    expdata.ExecuteSQL(strsql);
                    recordid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                    expdata.sqlexecute.Parameters.Clear();
                }
                //insertUserDefinedValues(recordid, oFileContents[i], keyfield);
                status.Add("Row " + i + " imported successfully.");
            }

            status.Add("File imported successfully.");
            return status;
        }

        private void insertUserDefinedValues(int recordid, List<object> oFileContents, cField keyfield)
        {
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;

            cFields clsfields = new cFields(accountid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SqlDataReader reader;
            cField currentfield;
            int userdefineid, count;
            Guid id;
            for (int x = 0; x < oFileContents.Count; x++)
            {

                id = lstMatchingGrid[x].destinationcolumn;
                if (1== 1)//if (id >= 1000)
                {
                    currentfield = clsfields.getFieldById(id);

                    strsql = "select userdefineid from userdefined where label = @label";
                    expdata.sqlexecute.Parameters.AddWithValue("@label", currentfield.description);
                    reader = expdata.GetReader(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    userdefineid = 0;
                    while (reader.Read())
                    {
                        userdefineid = reader.GetInt32(0);
                    }
                    reader.Close();
                    if (userdefineid > 0)
                    {
                        strsql = "select count(*) from userdefined_values where userdefineid = @userdefineid and recordid = @recordid";
                        expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", userdefineid);
                        expdata.sqlexecute.Parameters.AddWithValue("@recordid", recordid);
                        count = expdata.getcount(strsql);
                        if (count > 0)
                        {
                            strsql = "update userdefined_values set [value] = @value where userdefineid = @userdefineid and recordid = @recordid";
                        }
                        else
                        {
                            strsql = "insert into userdefined_values (userdefineid, recordid, [value]) values (@userdefineid, @recordid, @value)";
                        }
                        expdata.sqlexecute.Parameters.AddWithValue("@value", oFileContents[x].ToString());
                        expdata.ExecuteSQL(strsql);
                        expdata.sqlexecute.Parameters.Clear();
                    }

                }


            }
                
            
        }
        private int? getLookupValue(cField lookupfield, object value)
        {
            int? lookupvalue = null;
            System.Data.SqlClient.SqlDataReader reader;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string lookupsql = "select [" + lookupfield.table.tablename + "].[" + lookupfield.table.primarykeyfield.field + "] from [" + lookupfield.table.tablename + "] where [" + lookupfield.table.tablename + "].[" + lookupfield.field + "] = @lookupvalue";
            expdata.sqlexecute.Parameters.AddWithValue("@lookupvalue", value);
            reader = expdata.GetReader(lookupsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    lookupvalue = reader.GetInt32(0);
                }
            }
            reader.Close();
            return lookupvalue;
        }
        private cField getKeyField()
        {
            cTables clstables = new cTables(accountid);
            cTable reqtable = clstables.getTableById(tableid);
            cFields clsfields = new cFields(accountid);
            return reqtable.stringkeyfield;
        }
        private int getKeyFieldIndex(cField keyfield)
        {
            foreach (cImportField s in lstMatchingGrid)
            {

                Guid id = s.destinationcolumn;
                if (keyfield.fieldid == id)
                {
                    return (lstMatchingGrid.IndexOf(s));
                }
            }
            return -1;
        }

        
    }

    [Serializable()]
    public class cFlatFileImport : cImport, IImport
    {
        private string sDelimiter;
        private bool bReplaceCarriageReturn;

        public cFlatFileImport(int accountid, byte[] data, string delimiter, int headerrows, int footerrows, bool replacecarriagereturn)
        {
            nAccountid = accountid;
            sDelimiter = delimiter;
            bFile = data;
            nHeaderCount = headerrows;
            nFooterCount = footerrows;
            bReplaceCarriageReturn = replacecarriagereturn;
        }

        #region properties
        public string delimiter
        {
            get { return sDelimiter; }
        }
        #endregion

        public void extractDataFromFile(bool isSample)
        {
            try
            {
                MemoryStream stream = new MemoryStream(filedata);
                List<string> rows = new List<string>();
                List<string[]> splitrows = new List<string[]>();
                oFileContents.Clear();
                ExcelEngine exceleng = new ExcelEngine();
                int startrow = 0;
                string currentrow;
                if (hasheader || headercount > 0)
                {
                    startrow = headercount;
                }

                StreamReader reader = new StreamReader(stream);
                if (bReplaceCarriageReturn)
                {
                    string data = reader.ReadToEnd();
                    data = data.Replace("\n", "¬");
                    data = data.Replace("\r¬", "\r\n");
                    string[] temp = data.Split(new char[] { '\r', '\n' });
                    for (int i = 0; i < temp.GetLength(0); i++)
                    {
                        if (temp[i] != "")
                        {
                            rows.Add(temp[i]);
                        }
                    }
                }
                else
                {
                    while ((currentrow = reader.ReadLine()) != null)
                    {
                        rows.Add(currentrow);
                    }
                }
                reader.Close();

                foreach (string row in rows)
                {
                    splitrows.Add(row.Split(Convert.ToChar(delimiter)));
                }

                //get the header row
                arrHeaderRow = new string[splitrows[0].GetLength(0)];
                for (int i = 0; i < splitrows[0].GetLength(0); i++)
                {
                    arrHeaderRow[i] = splitrows[0][i];
                }
                //get the footer row
                arrFooterRow = new string[splitrows[splitrows.Count - 1].GetLength(0)];
                for (int i = 0; i < splitrows[splitrows.Count - 1].GetLength(0); i++)
                {
                    arrFooterRow[i] = splitrows[splitrows.Count - 1][i];
                }

                List<object> values;
                for (int row = startrow; row < splitrows.Count - footercount; row++)
                {
                    values = new List<object>();
                    for (int column = 0; column < splitrows[row].GetLength(0); column++)
                    {

                        values.Add(splitrows[row][column]);

                    }
                    oFileContents.Add(values);
                }
            }
            catch
            {
                bValidFile = false;
            }
        }

        
        

        
        
        #region properties
        
        
        #endregion
    }

    [Serializable()]
    public class cFlatFileExcelImport : cImport, IImport
    {
        private string sDelimiter;

        public cFlatFileExcelImport(int accountid, byte[] data, string delimiter, int headerrows, int footerrows)
        {
            nAccountid = accountid;
            sDelimiter = delimiter;
            bFile = data;
            nHeaderCount = headerrows;
            nFooterCount = footerrows;
        }

        #region properties
        public string delimiter
        {
            get { return sDelimiter; }
        }
        #endregion

        public void extractDataFromFile(bool isSample)
        {
            try
            {
                MemoryStream stream = new MemoryStream(filedata);
                List<string> rows = new List<string>();
                List<string[]> splitrows = new List<string[]>();

                int columnCount;
                int rowCount;

                oFileContents.Clear();
                ExcelEngine exceleng = new ExcelEngine();
                IApplication app = exceleng.Excel;
                IWorkbook workbook = app.Workbooks.Open(stream, delimiter);
                IWorksheet worksheet = workbook.Worksheets[0];
                int startrow = 0;
                string currentrow;
                if (hasheader || headercount > 0)
                {
                    startrow = headercount;
                }

                if (isSample)
                {
                    columnCount = usedColumnIndexes.Count;
                }
                else
                {
                    columnCount = worksheet.Columns.GetLength(0);
                }

                //get the header row
                arrHeaderRow = new string[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    arrHeaderRow[i] = worksheet.Rows[0].Cells[i].Value.ToString();
                }

                //get the footer row
                arrFooterRow = new string[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    arrFooterRow[i] = worksheet.Rows[worksheet.Rows.GetLength(0) - 1].Cells[i].Value.ToString();
                }

                List<object> values;

                if (isSample)
                {
                    rowCount = int.Parse(ConfigurationManager.AppSettings["ImportSampleRowCount"]);
                }
                else
                {
                    rowCount = worksheet.Rows.GetLength(0);
                }


                for (int row = startrow; row < rowCount - footercount; row++)
                {
                    values = new List<object>();

                    if (isSample)
                    {
                        for (int column = 0; column < worksheet.Columns.GetLength(0); column++)
                        {
                            values.Add(worksheet.Rows[row].Cells[column].Value);
                        }
                    }
                    else
                    {
                        foreach (int column in usedColumnIndexes)
                        {
                            values.Add(worksheet.Rows[row].Cells[column].Value);
                        }
                    }
                    oFileContents.Add(values);
                }
            }
            catch
            {
                bValidFile = false;
            }
        }

        




        #region properties


        #endregion
    }
    [Serializable()]
    public class cXMLFileImport : cImport, IImport
    {

        public cXMLFileImport(int accountid, byte[] data)
        {
            nAccountid = accountid;

            bFile = data;
        }
        public void extractDataFromFile(bool isSample)
        {
            MemoryStream stream = new MemoryStream(filedata);
            DataSet ds = new DataSet();
            ds.ReadXml(stream);
            oFileContents.Clear();
            //get the header row
            arrHeaderRow = new string[ds.Tables[0].Columns.Count];
            foreach (DataColumn column in ds.Tables[0].Columns)
            {
                arrHeaderRow[ds.Tables[0].Columns.IndexOf(column)] = column.ColumnName;
            }
            


            List<object> values;
            for (int row = 0; row < ds.Tables[0].Rows.Count; row++)
            {
                values = new List<object>();
                for (int column = 0; column < ds.Tables[0].Columns.Count; column++)
                {
                   values.Add(ds.Tables[0].Rows[row][column]);
                }
                oFileContents.Add(values);
            }
        }
    }

    [Serializable()]
    public class cFixedWidthFileImport : cImport, IImport
    {

        int[,] arrfields;
        public cFixedWidthFileImport(int accountid, byte[] data, int headerrows, int footerrows, int[,] fields)
        {
            nAccountid = accountid;
            nHeaderCount = headerrows;
            nFooterCount = footerrows;
            bFile = data;
            arrfields = fields;
        }

        public void extractDataFromFile(bool isSample)
        {
            try
            {
                MemoryStream stream = new MemoryStream(filedata);
                List<string> rows = new List<string>();
                oFileContents.Clear();
                int startpos;
                int endpos;
                ExcelEngine exceleng = new ExcelEngine();
                int startrow = 0;
                string currentrow;
                if (hasheader || headercount > 0)
                {
                    startrow = headercount;
                }

                StreamReader reader = new StreamReader(stream);
                while ((currentrow = reader.ReadLine()) != null)
                {
                    rows.Add(currentrow);
                }
                reader.Close();



                //get the header row
                arrHeaderRow = new string[fields.GetLength(0)];
                for (int i = 0; i < fields.GetLength(0); i++)
                {
                    startpos = fields[i, 0];
                    endpos = fields[i, 1];
                    arrHeaderRow[i] = rows[0].Substring(startpos, endpos - startpos);
                }
                //get the footer row
                if (footercount > 0)
                {
                    arrFooterRow = new string[fields.GetLength(0)];
                    arrFooterRow[0] = rows[rows.Count - 1];

                }

                List<object> values;
                for (int row = startrow; row < rows.Count - footercount; row++)
                {
                    values = new List<object>();
                    for (int column = 0; column < fields.GetLength(0); column++)
                    {
                        startpos = fields[column, 0];
                        endpos = fields[column, 1];
                        values.Add(rows[row].Substring(startpos, endpos - startpos));

                    }
                    oFileContents.Add(values);
                }
            }
            catch
            {
                bValidFile = false;
            }
        }

        #region properties
        public int[,] fields
        {
            get { return arrfields; }
        }
        #endregion
    }
    [Serializable()]
    public class cExcelFileImport : cImport, IImport
    {
        private int nWorksheet;
        private List<string> lstWorksheets = new List<string>();
        public cExcelFileImport(int accountid, byte[] data, int headerrows, int footerrows, int worksheet)
        {
            nAccountid = accountid;
            nHeaderCount = headerrows;
            nFooterCount = footerrows;
            bFile = data;
            nWorksheet = worksheet;
        }

        public void extractDataFromFile(bool isSample)
        {
            try
            {
                MemoryStream stream = new MemoryStream(filedata);

                ExcelEngine exceleng = new ExcelEngine();
                int startrow = 0;

                int columnCount;
                int rowCount;
                
                oFileContents.Clear();
                IApplication app = exceleng.Excel;

                IWorkbook workbook = app.Workbooks.Open(stream);

                //get the worksheet names
                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    lstWorksheets.Add(workbook.Worksheets[i].Name);
                }

                IWorksheet worksheet = workbook.Worksheets[worksheetnumber];

                if (hasheader || headercount > 0)
                {
                    startrow = headercount;
                }


                if (isSample)
                {
                    columnCount = worksheet.Columns.GetLength(0);
                }
                else
                {
                    columnCount = usedColumnIndexes.Count;
                    
                }

                //get the header row
                arrHeaderRow = new string[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    arrHeaderRow[i] = worksheet.Rows[0].Cells[i].Value.ToString();
                }

                //get the footer row
                arrFooterRow = new string[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    arrFooterRow[i] = worksheet.Rows[worksheet.Rows.GetLength(0) - 1].Cells[i].Value.ToString();
                }

                List<object> values;

                if (isSample)
                {
                    rowCount = int.Parse(ConfigurationManager.AppSettings["ImportSampleRowCount"]);
                }
                else
                {
                    rowCount = worksheet.Rows.GetLength(0);
                }

                for (int row = startrow; row < rowCount; row++)
                {
                    values = new List<object>();

                    if (isSample)
                    {
                        for (int column = 0; column < worksheet.Columns.GetLength(0); column++ )
                        {
                            values.Add(worksheet.Rows[row].Cells[column].Value);
                        }
                    }
                    else
                    {
                        foreach (int column in usedColumnIndexes)
                        {
                            values.Add(worksheet.Rows[row].Cells[column].Value);
                        }
                    }

                    oFileContents.Add(values);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", ex.Message + " " + ex.StackTrace);
                bValidFile = false;
            }

        }

        public List<ListItem> CreateWorkSheetDropDown()
        {
            List<ListItem> items = new List<ListItem>();
            foreach (string i in lstWorksheets)
            {
                items.Add(new ListItem(i, lstWorksheets.IndexOf(i).ToString()));
            }
            return items;
        }

        #region properties
        public int worksheetnumber
        {
            get { return nWorksheet; }
            set { nWorksheet = value; } //extractDataFromFile();
        }
        #endregion
    }
    public interface IImport
    {
        void extractDataFromFile(bool isSample);
        DataTable getSample();
        DataTable getMatchingGrid();
        List<string> validateImport();
    }

    [Serializable()]
    public class cImportField
    {
        private Guid nDestinationColumn;
        private Guid nLookupColumn;
        private string sDefaultValue;

        public cImportField(Guid destinationcolumn, Guid lookupcolumn, string defaultvalue)
        {
            nDestinationColumn = destinationcolumn;
            nLookupColumn = lookupcolumn;
            sDefaultValue = defaultvalue;
        }

        #region properties
        
        public Guid destinationcolumn
        {
            get { return nDestinationColumn; }
        }
        public Guid lookupcolumn
        {
            get { return nLookupColumn; }
        }
        public string defaultvalue
        {
            get { return sDefaultValue; }
        }
        #endregion
    }
}
