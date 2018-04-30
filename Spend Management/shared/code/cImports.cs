using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.IO;

using System.Collections.Generic;

using System.Data.SqlClient;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.Cards;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Hotels;
using Syncfusion.XlsIO;

namespace Spend_Management
{
    using System.Text;

    using BusinessLogic.ImportExport;

    using SpendManagementLibrary.Helpers;

    using SQLDataAccess.ImportExport;

    using Utilities.DistributedCaching;

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
        public List<int> usedColumnIndexes
        {
            get { return lstUsedColumnIndexes; }
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
                    if (!tbl.Columns.Contains(arrHeaderRow[i]))
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
                    reqfield = clsfields.GetFieldByID(id);
                    errors.Add(reqfield.Description + " has been matched to more than one column in the source file.");
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

                    if (importfield.destinationcolumn == field.FieldID && field.GetLookupField() == null)
                    {
                        containsField = true;
                        break;
                    }
                }
                if (!containsField && field.GetLookupField() == null)
                {
                    unmatchedfields.Add(field);
                }
            }
            
            foreach (cField field in unmatchedfields)
            {
                tbl.Rows.Add(new object[] { field.FieldID, "" });
            }
            return tbl;
        }
        private void allMandatoryFieldsSelected(ref List<string> errors)
        {
            cFields clsfields = new cFields(accountid);
            List<cField> fields = clsfields.GetFieldsByTableID(tableid);
            bool matchedtolookup;
            bool containsfield;
            foreach (cField field in fields)
            {
                matchedtolookup = false;
                if (field.Mandatory)
                {
                    if (field.GetLookupTable() != null)
                    {
                        List<cField> lookupfields = clsfields.getLookupFields(field.GetLookupTable().TableID);
                        foreach (cField lookupfield in lookupfields)
                        {
                            foreach (cImportField importfield in lstMatchingGrid)
                            {
                                if ((importfield.lookupcolumn == lookupfield.FieldID && importfield.destinationcolumn == field.FieldID) || importfield.defaultvalue != "")
                                {
                                    matchedtolookup = true;
                                    break;
                                }
                            }
                        }
                        if (!matchedtolookup)
                        {
                            errors.Add(field.Description + " is a mandatory field but has not been matched to a column in the source file.");
                        }
                    }
                    else
                    {
                        containsfield = false;
                        
                        foreach (cImportField importfield in lstMatchingGrid)
                        {
                            if (importfield.destinationcolumn == field.FieldID)
                            {
                                containsfield = true;                                
                            }
                        }
                        if (!containsfield)
                        {
                            errors.Add(field.Description + " is a mandatory field but has not been matched to a column in the source file.");
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
                        typefield = clsfields.GetFieldByID(lstMatchingGrid[x].lookupcolumn);
                        if (getLookupValue(typefield, oFileContents[i][x]) == null)
                        {
                            errors.Add("Row " + i + ", Column " + x + " cannot be imported. The column does not have an associated lookup value.");
                        }
                    }
                    else
                    {
                        typefield = clsfields.GetFieldByID(id);
                        if (typefield != null)
                        {
                            switch (typefield.FieldType)
                            {
                                case "N": //Integer
                                    try
                                    {
                                        int.Parse(oFileContents[i][x].ToString());
                                    }
                                    catch
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.Description + " is an Integer field. " + oFileContents[i][x] + " cannot be converted to an integer value.");
                                    }
                                    break;
                                case "C": //currency
                                    try
                                    {
                                        decimal.Parse(oFileContents[i][x].ToString());
                                    }
                                    catch
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.Description + " is a Currency field. " + oFileContents[i][x] + " cannot be converted to a currency value.");
                                    }
                                    break;
                                case "X":
                                    try
                                    {
                                        Convert.ToBoolean(oFileContents[i][x].ToString());
                                    }
                                    catch
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.Description + " is a Boolean field. " + oFileContents[i][x] + " cannot be converted to a boolean value.");
                                    }
                                    break;
                                case "D": //date
                                    try
                                    {
                                        DateTime.Parse(oFileContents[i][x].ToString());
                                    }
                                    catch
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. " + typefield.Description + " is a Date field. " + oFileContents[i][x] + " cannot be converted to a date value.");
                                    }
                                    break;
                                case "S":
                                    //check length
                                    if (oFileContents[i][x].ToString().Length > typefield.Width)
                                    {
                                        errors.Add("Row " + (i + 1) + ", Column " + (x + 1) + " cannot be imported. The value of this field can be no more than " + typefield.Width + " characters. \"" + oFileContents[i][x] + "\" is " + oFileContents[i][x].ToString().Length + " characters.");
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
                    if (clsfields.GetFieldByID(id).Mandatory)
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
            Guid id = Guid.Empty;

            for (int i = 0; i < arrHeaderRow.GetLength(0); i++)
            {
                try
                {
                    if (lstMatchingGrid != null) 
                    {
                        if (lstMatchingGrid.Count != 0)
                        {
                            id = lstMatchingGrid[i].destinationcolumn;
                        }
                    }
                    
                    if (hasheader)
                    {
                        if (id != Guid.Empty)
                        {
                            tbl.Rows.Add(new object[] { arrHeaderRow[i], id });
                        }
                        else
                        {
                            tbl.Rows.Add(new object[] { arrHeaderRow[i], "" });
                        }

                    }
                    else
                    {
                        if (oFileContents.Count > 0)
                        {
                            if (id != Guid.Empty)
                            {
                                tbl.Rows.Add(oFileContents[0][i], id);
                            }
                            else
                            {
                                tbl.Rows.Add(oFileContents[0][i], "");
                            }
                        }
                        else
                        {
                            if (id != Guid.Empty)
                            {
                                tbl.Rows.Add(new object[] { "Column " + (i + 1), id });
                            }
                            else
                            {
                                tbl.Rows.Add(new object[] { "Column " + (i + 1), "" });
                            }
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

        /// <summary>
        /// Import data from an external source
        /// </summary>
        /// <param name="importFileFactory">An instance of <see cref="ImportFileFactory"/></param>
        /// <returns>A <see cref="List{T}"/>of status for each line or a single entry if the import fails.</returns>
        public virtual List<string> importData(ImportFileFactory importFileFactory)
        {
            var newStyleImport = importFileFactory.New(this.tableid);
            if (newStyleImport != null)
            {
                var matchingFields = new List<ImportField>();
                foreach (cImportField importField in this.lstMatchingGrid)
                {
                    matchingFields.Add(new ImportField(importField.destinationcolumn, importField.lookupcolumn, importField.defaultvalue));
                }

                return newStyleImport.Import(this.lstDefaultValues, matchingFields, this.oFileContents);
            }

            var currentUser = cMisc.GetCurrentUser();
            List<string> status;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                cFields clsfields = new cFields(this.accountid);
                cField currentfield;
                cField keyfield = this.getKeyField();
                int recordcount = 0;
                int keyindex = this.getKeyFieldIndex(keyfield);
                Guid id;
                int recordid = 0;
                status = new List<string>();
                bool keyFieldValSet = false;

                if (keyindex != -1)
                {
                    for (int i = 0; i < this.oFileContents.Count; i++)
                    {
                        keyFieldValSet = false;
                        //does the record already exist?
                        expdata.sqlexecute.Parameters.AddWithValue("@value", this.oFileContents[i][keyindex]);
                        recordcount = expdata.ExecuteScalar<int>("select count(*) from [" + keyfield.GetParentTable().TableName + "] where [" + keyfield.GetParentTable().TableName + "].[" + keyfield.FieldName + "] = @value");
                        expdata.sqlexecute.Parameters.Clear();
                        var parentTable = keyfield.GetParentTable();
                        if (recordcount > 0) //record exists so update
                        {
                            var strsql = new StringBuilder("update [" + parentTable.TableName + "] set ");
                            
                            for (int x = 0; x < this.oFileContents[i].Count; x++)
                            {

                                id = this.lstMatchingGrid[x].destinationcolumn;

                                if (id != Guid.Empty)
                                {
                                    currentfield = clsfields.GetFieldByID(id);

                                    if (!currentfield.FieldName.StartsWith("udf"))
                                    {
                                        if (id != keyfield.FieldID)
                                        {
                                            strsql.Append("[" + keyfield.GetParentTable().TableName + "].[" + currentfield.FieldName + "] = @value" + x + ",");
                                        }
                                        if (this.lstMatchingGrid[x].lookupcolumn != Guid.Empty) //lookup
                                        {
                                            cField lookupfield = clsfields.GetFieldByID(this.lstMatchingGrid[x].lookupcolumn);
                                            int? lookupvalue = this.getLookupValue(lookupfield, this.oFileContents[i][x]);
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, lookupvalue);
                                            keyFieldValSet = true;
                                        }
                                        else if (keyfield.FieldID != id)
                                        {
                                            if (currentfield.ValueList)
                                            {
                                                int key = 0;
                                                if (int.TryParse(this.oFileContents[i][x].ToString(), out key))
                                                {
                                                    if (currentfield.ListItems.ContainsKey(key))
                                                    {
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, key);
                                                        keyFieldValSet = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                switch (currentfield.FieldType)
                                                {
                                                    case "S":
                                                    case "FS":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, this.oFileContents[i][x].ToString());
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "N":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, int.Parse(this.oFileContents[i][x].ToString()));
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "C":
                                                    case "M":
                                                    case "FD":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, decimal.Parse(this.oFileContents[i][x].ToString()));
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "D":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, DateTime.Parse(this.oFileContents[i][x].ToString()));
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "X":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, Convert.ToByte(Convert.ToBoolean(this.oFileContents[i][x].ToString())));
                                                        keyFieldValSet = true;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (KeyValuePair<Guid, string> defaultvalue in this.lstDefaultValues)
                            {
                                currentfield = clsfields.GetFieldByID(defaultvalue.Key);

                                if (!currentfield.FieldName.StartsWith("udf"))
                                {

                                    strsql.Append("[" + keyfield.GetParentTable().TableName + "].[" + currentfield.FieldName + "] = @defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key) + ",");

                                    if (currentfield.ValueList)
                                    {
                                        expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), currentfield.ListItems[defaultvalue.Value]);
                                        keyFieldValSet = true;
                                    }
                                    else
                                    {
                                        switch (currentfield.FieldType)
                                        {
                                            case "S":
                                            case "FS":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), defaultvalue.Value);
                                                keyFieldValSet = true;
                                                break;
                                            case "N":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), int.Parse(defaultvalue.Value));
                                                keyFieldValSet = true;
                                                break;
                                            case "C":
                                            case "M":
                                            case "FD":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), decimal.Parse(defaultvalue.Value));
                                                keyFieldValSet = true;
                                                break;
                                            case "D":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), DateTime.Parse(defaultvalue.Value));
                                                keyFieldValSet = true;
                                                break;
                                            case "X":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), Convert.ToByte(Convert.ToBoolean(defaultvalue.Value)));
                                                keyFieldValSet = true;
                                                break;
                                        }
                                    }
                                }
                            }

                            
                            if (keyFieldValSet)
                            {
                                strsql = strsql.Remove(strsql.Length - 1, 1);
                                strsql.AppendFormat(" where [{0}].[{1}] = @keyvalue", parentTable.TableName, keyfield.FieldName);
                                expdata.sqlexecute.Parameters.AddWithValue("@keyvalue", this.oFileContents[i][keyindex]);
                                expdata.ExecuteSQL(strsql.ToString());
                            }

                            expdata.sqlexecute.Parameters.Clear();
                            strsql = new StringBuilder(string.Format("select [{0}].[{1}] from [{0}] where [{2}] = @keyvalue", parentTable.TableName, parentTable.GetPrimaryKey().FieldName, keyfield.FieldName));
                            expdata.sqlexecute.Parameters.AddWithValue("@keyvalue", this.oFileContents[i][keyindex]);
                            recordid = expdata.ExecuteScalar<int>(strsql.ToString());
                            this.ClearCache(parentTable, recordid);
                            expdata.sqlexecute.Parameters.Clear();
                        }
                        else //record doesn't exist, insert new
                        {
                            keyFieldValSet = false;
                            var strsql = new StringBuilder("insert into [" + keyfield.GetParentTable().TableName + "] (");
                            var subAccountField = parentTable.GetSubAccountIDField();
                            if (subAccountField != null)
                            {
                                strsql.Append("[" + parentTable.TableName + "].[" + subAccountField.FieldName + "],");
                            }
                            foreach (cImportField field in this.lstMatchingGrid)
                            {
                                id = field.destinationcolumn;
                                if (id != Guid.Empty)
                                {
                                    currentfield = clsfields.GetFieldByID(id);

                                    if (!currentfield.FieldName.StartsWith("udf"))
                                    {
                                        strsql.Append( "[" + parentTable.TableName + "].[" + currentfield.FieldName + "],");
                                    }
                                }
                            }
                            foreach (KeyValuePair<Guid, string> defaultvalue in this.lstDefaultValues)
                            {
                                currentfield = clsfields.GetFieldByID(defaultvalue.Key);

                                strsql.Append("[" + keyfield.GetParentTable().TableName + "].[" + currentfield.FieldName + "],");
                            }
                            strsql = strsql.Remove(strsql.Length - 1, 1);
                            strsql.Append(") values (");
                            if (subAccountField != null)
                            {
                                strsql.Append( "@subAccountId,");
                                expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", currentUser.CurrentSubAccountId);
                            }

                            for (int x = 0; x < this.oFileContents[i].Count; x++)
                            {
                                id = this.lstMatchingGrid[x].destinationcolumn;

                                if (id != Guid.Empty)
                                {
                                    currentfield = clsfields.GetFieldByID(id);
                                    if (!currentfield.FieldName.StartsWith("udf"))
                                    {
                                        strsql.Append( "@value" + x + ",");
                                        keyFieldValSet = true;

                                        if (this.lstMatchingGrid[x].lookupcolumn != Guid.Empty) //lookup
                                        {
                                            cField lookupfield = clsfields.GetFieldByID(this.lstMatchingGrid[x].lookupcolumn);
                                            int? lookupvalue = this.getLookupValue(lookupfield, this.oFileContents[i][x]);
                                            expdata.sqlexecute.Parameters.AddWithValue("@value" + x, lookupvalue);
                                            keyFieldValSet = true;
                                        }
                                        else
                                        {

                                            if (currentfield.ValueList)
                                            {
                                                int key = 0;
                                                if (int.TryParse(this.oFileContents[i][x].ToString(), out key))
                                                {
                                                    if (currentfield.ListItems.ContainsKey(key))
                                                    {
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, key);
                                                        keyFieldValSet = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                switch (currentfield.FieldType)
                                                {
                                                    case "S":
                                                    case "FS":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, this.oFileContents[i][x].ToString());
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "N":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, int.Parse(this.oFileContents[i][x].ToString()));
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "C":
                                                    case "M":
                                                    case "FD":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, decimal.Parse(this.oFileContents[i][x].ToString()));
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "D":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, DateTime.Parse(this.oFileContents[i][x].ToString()));
                                                        keyFieldValSet = true;
                                                        break;
                                                    case "X":
                                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + x, Convert.ToByte(Convert.ToBoolean(this.oFileContents[i][x].ToString())));
                                                        keyFieldValSet = true;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (KeyValuePair<Guid, string> defaultvalue in this.lstDefaultValues)
                            {
                                if (!defaultvalue.Value.StartsWith("udf"))
                                {
                                    strsql.Append( "@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key) + ",");
                                    currentfield = clsfields.GetFieldByID(defaultvalue.Key);
                                    if (currentfield.ValueList)
                                    {
                                        expdata.sqlexecute.Parameters.AddWithValue("@value" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), currentfield.ListItems[defaultvalue.Value]);
                                    }
                                    else
                                    {
                                        switch (currentfield.FieldType)
                                        {
                                            case "S":
                                            case "FS":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), defaultvalue.Value);
                                                keyFieldValSet = true;
                                                break;
                                            case "N":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), int.Parse(defaultvalue.Value));
                                                keyFieldValSet = true;
                                                break;
                                            case "C":
                                            case "M":
                                            case "FD":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), decimal.Parse(defaultvalue.Value));
                                                keyFieldValSet = true;
                                                break;
                                            case "D":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), DateTime.Parse(defaultvalue.Value));
                                                keyFieldValSet = true;
                                                break;
                                            case "X":
                                                expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue" + this.lstDefaultValues.IndexOfKey(defaultvalue.Key), Convert.ToByte(Convert.ToBoolean(defaultvalue.Value)));
                                                keyFieldValSet = true;
                                                break;
                                        }
                                    }
                                }
                            }

                            if (keyFieldValSet)
                            {
                                strsql = strsql.Remove(strsql.Length - 1, 1);
                                strsql.Append(");select @identity = scope_identity();");
                                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
                                expdata.ExecuteSQL(strsql.ToString());
                                recordid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                            }
                            else
                            {
                                expdata.sqlexecute.Parameters.Clear();
                                status.Add("File not imported no key field selected.");
                                return status;
                            }
                            expdata.sqlexecute.Parameters.Clear();
                        }
                        this.insertUserDefinedValues(recordid, this.oFileContents[i], keyfield);
                        status.Add("Row " + i + " imported successfully.");
                    }

                    // distributed cache needs clearing if the table affected uses that instead of brokers
                    this.DeleteCollectionCache(keyfield.GetParentTable().TableID.ToString());

                    status.Add("File imported successfully.");
                }
                else
                {
                    status.Add("File not imported no key field selected.");
                }
            }

            return status;
        }

        private void ClearCache(cTable parentTable, int recordid)
        {
            var parentTableName = parentTable.TableName.ToLower();
            if (parentTableName.Contains("employee"))
            {
                    CacheDelete(recordid, accountid, Employee.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeAccessRoles.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeBroadcastMessages.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeContextualHelp.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeCostBreakdown.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeEmailNotifications.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeGridSorts.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeHomeAddresses.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeItemRoles.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeNewGridSorts.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeSubCategories.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeViews.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeWorkAddresses.CacheArea);
                    CacheDelete(recordid, accountid, EmployeeHomeAddresses.CacheArea);
            }

            if (parentTableName.Contains("statement"))
            {
                CacheDelete(recordid, accountid, cCardStatements.CacheArea);
            }

            if (parentTableName.Contains("mileage"))
            {
                CacheDelete(recordid, accountid, cMileagecats.CacheArea);
            }
            
            if (parentTableName.Contains("team"))
            {
                CacheDelete(recordid, accountid, cTeams.CacheArea);
            }

            if (parentTableName.Contains("address"))
            {
                CacheDelete(recordid, accountid, Addresses.CacheArea);
            }

            if (parentTableName.Contains("corporate"))
            {
                CacheDelete(recordid, accountid, CorporateCards.CacheArea);
            }

            if (parentTableName.Contains("hotel"))
            {
                CacheDelete(recordid, accountid, Hotel.cacheArea);
            }

        }

        private void CacheDelete(int employeeId, int accountId, string cacheArea)
        {
            var cache = new Cache();
            cache.Delete(accountId, cacheArea, employeeId.ToString(CultureInfo.InvariantCulture));
        }

        private void DeleteCollectionCache(string tableId)
        {
            switch (tableId.ToUpperInvariant())
            {
                case "83077E08-FE7D-4C1A-A306-BE4327C349C1": // reasons
                    var cache = new Cache();
                    cache.Delete(accountid, cReasons.CacheArea, "0");
                    break;
            }
        }

        private void insertUserDefinedValues(int recordid, List<object> oFileContents, cField keyfield)
        {
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(accountid); ;
            SortedList<int, object> lstUdfs = new SortedList<int,object>();
            cFields clsfields = new cFields(accountid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SqlDataReader reader;
            int userdefineid;
            Guid id;
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cTables clsTables = new cTables(accountid);
            cTable table = keyfield.GetParentTable().GetUserdefinedTable();

            if (table != null)
            {
                for (int x = 0; x < oFileContents.Count; x++)
                {
                    id = lstMatchingGrid[x].destinationcolumn;

                    if (id != Guid.Empty)
                    {
                        strsql = "select userdefineid from userdefined where fieldid = @fieldid";
                        expdata.sqlexecute.Parameters.AddWithValue("@fieldid", id);
                        using (reader = expdata.GetReader(strsql))
                        {
                            expdata.sqlexecute.Parameters.Clear();
                            userdefineid = 0;

                            while (reader.Read())
                            {
                                userdefineid = reader.GetInt32(reader.GetOrdinal("userdefineid"));
                            }
                            reader.Close();
                        }
                        if (userdefineid > 0)
                        {
                            lstUdfs.Add(userdefineid, oFileContents[x].ToString());
                        }
                    }
                }

                clsuserdefined.SaveValues(table, recordid, lstUdfs, clsTables, clsfields, currentUser);
            }
        }

        private int? getLookupValue(cField lookupfield, object value)
        {
            int? lookupvalue = null;
            System.Data.SqlClient.SqlDataReader reader;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string lookupsql = "select [" + lookupfield.GetParentTable().TableName + "].[" + lookupfield.GetParentTable().GetPrimaryKey().FieldName + "] from [" + lookupfield.GetParentTable().TableName + "] where [" + lookupfield.GetParentTable().TableName + "].[" + lookupfield.FieldName + "] = @lookupvalue";
            expdata.sqlexecute.Parameters.AddWithValue("@lookupvalue", value);
            using (reader = expdata.GetReader(lookupsql))
            {
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        lookupvalue = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }
            return lookupvalue;
        }
        private cField getKeyField()
        {
            cTables clstables = new cTables(accountid);
            cTable reqtable = clstables.GetTableByID(tableid);
            cFields clsfields = new cFields(accountid);
            return reqtable.GetKeyField();
        }
        private int getKeyFieldIndex(cField keyfield)
        {
            foreach (cImportField s in lstMatchingGrid)
            {

                Guid id = s.destinationcolumn;
                if (keyfield.FieldID == id)
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
                //ExcelEngine exceleng = new ExcelEngine();
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

                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(delimiter + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                foreach (string row in rows)
                {
                    splitrows.Add(r.Split(row));
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

                oFileContents.Clear();
                ExcelEngine exceleng = new ExcelEngine();
                IApplication app = exceleng.Excel;
                IWorkbook workbook = app.Workbooks.Open(stream, delimiter);
                IWorksheet worksheet = workbook.Worksheets[0];
                int startrow = 0;
                if (hasheader || headercount > 0)
                {
                    startrow = headercount;
                }

                //get the header row
                arrHeaderRow = new string[worksheet.Columns.GetLength(0)];
                for (int i = 0; i < worksheet.Columns.GetLength(0); i++)
                {
                    arrHeaderRow[i] = worksheet.Rows[0].Cells[i].Value.ToString();
                }

                //get the footer row
                arrFooterRow = new string[worksheet.Columns.GetLength(0)];
                for (int i = 0; i < worksheet.Columns.GetLength(0); i++)
                {
                    arrFooterRow[i] = worksheet.Rows[worksheet.Rows.GetLength(0) - 1].Cells[i].Value.ToString();
                }

                List<object> values;
                for (int row = startrow; row < worksheet.Rows.GetLength(0) - footercount; row++)
                {
                    values = new List<object>();
                    for (int column = 0; column < worksheet.Columns.GetLength(0); column++)
                    {
                        values.Add(worksheet.Rows[row].Cells[column].Value);
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
        private int[] nSign;
        public cFixedWidthFileImport(int accountid, byte[] data, int headerrows, int footerrows, int[,] fields, int[] sign)
        {
            nAccountid = accountid;
            nHeaderCount = headerrows;
            nFooterCount = footerrows;
            bFile = data;
            arrfields = fields;
            this.nSign = sign;
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
                int sign;
                //ExcelEngine exceleng = new ExcelEngine();
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
                        sign = this.nSign[column];
                        if (sign != -1)
                        {
                            values.Add(rows[row].Substring(sign, 1) + rows[row].Substring(startpos, endpos - startpos));
                        }
                        else
                        {
                            values.Add(rows[row].Substring(startpos, endpos - startpos));
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
            using (var exceleng = new ExcelEngine())
            {
            IWorkbook workbook = null;
            try
            {
                    int startrow = 0;
                using (MemoryStream stream = new MemoryStream(filedata))
                {
                        startrow = 0;

                    oFileContents.Clear();
                        var app = exceleng.Excel;

                    workbook = app.Workbooks.Open(stream);
                    }

                    //get the worksheet names
                    for (int i = 0; i < workbook.Worksheets.Count; i++)
                    {
                        lstWorksheets.Add(workbook.Worksheets[i].Name);
                    }

                    var worksheet = workbook.Worksheets[nWorksheet];

                    if (hasheader || headercount > 0)
                    {
                        startrow = headercount;
                    }

                    //get the header row
                    arrHeaderRow = new string[worksheet.Columns.GetLength(0)];
                    for (int i = 0; i < worksheet.Columns.GetLength(0); i++)
                    {
                        arrHeaderRow[i] = worksheet.Rows[0].Cells[i].Value.ToString();
                    }

                   var lastPopulatedDataRow = hasfooter ? worksheet.Rows.GetLength(0) - 1 : worksheet.Rows.GetLength(0);

                   for (int i = 0; i < lastPopulatedDataRow; i++)
                            {
                                if (!worksheet.Rows[i].IsBlank) {continue;}
                                lastPopulatedDataRow = i;
                                break;
                            }

                    if (hasfooter)
                    {                    
                    arrFooterRow = new string[worksheet.Columns.GetLength(0)];
                    for (int i = 0; i < worksheet.Columns.GetLength(0); i++)
                    {
                        arrFooterRow[i] = worksheet.Rows[lastPopulatedDataRow + 1].Cells[i].Value.ToString();
                    }
                    }

                    List<object> values;
                    for (int row = startrow; row < lastPopulatedDataRow; row++)
                    {
                        values = new List<object>();
                        for (int column = 0; column < worksheet.Columns.GetLength(0); column++)
                        {
                            values.Add(worksheet.Rows[row].Cells[column].Value);
                        }

                        oFileContents.Add(values);

                        if (isSample && oFileContents.Count > 30)
                        {
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + " " + ex.StackTrace);
                bValidFile = false;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
                exceleng.ThrowNotSavedOnDestroy = false;
                exceleng.Dispose();
            }
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
