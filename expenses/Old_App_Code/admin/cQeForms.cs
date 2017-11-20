namespace expenses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    using SpendManagementLibrary;

    using Spend_Management;

    using Utilities.DistributedCaching;

    /// <summary>
	/// Summary description for cQeForms.
	/// </summary>
	public class cQeForms
	{
        #region Fields

        private readonly Cache cache = new Cache();

        readonly int nAccountid;

        SortedList list;

        string strsql;

        public const string CacheArea = "quickEntryForms";

        #endregion

        #region Constructors and Destructors

        public cQeForms(int accountid)
		{
			this.nAccountid = accountid;
            
			this.InitialiseData();
		}

        #endregion

        #region Public Properties

        public int accountid
        {
            get { return this.nAccountid; }
        }

        public int count
        {
            get { return this.list.Count; }
        }

        #endregion

        #region Public Methods and Operators

        public int addForm(string name, string description, bool genmonth, int numrows)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
			int quickentryid;

			if (this.alreadyExists(name,0,0))
			{
				return -1;
			}

			this.strsql = "insert into quick_entry_forms ([name], description, genmonth, numrows) " +
				"values (@name, @description, @genmonth, @numrows); set @identity = @@identity;";
			
			expdata.sqlexecute.Parameters.AddWithValue("@name",name);
			expdata.sqlexecute.Parameters.AddWithValue("@description",description);
			expdata.sqlexecute.Parameters.AddWithValue("@genmonth",Convert.ToByte(genmonth));
			expdata.sqlexecute.Parameters.AddWithValue("@numrows",numrows);
			expdata.sqlexecute.Parameters.AddWithValue("@identity",SqlDbType.Int);
			expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;

			expdata.ExecuteSQL(this.strsql);
			quickentryid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

			this.ResetCache();

			return quickentryid;
		}

        public void deleteForm(int quickentryid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
			this.strsql = "delete from quick_entry_forms where quickentryid = @quickentryid";
			expdata.sqlexecute.Parameters.AddWithValue("@quickentryid", quickentryid);
			expdata.ExecuteSQL(this.strsql);
			expdata.sqlexecute.Parameters.Clear();

            this.ResetCache();
		}

        public DataSet generatePrintOutFields(int quickentryid, int employeeid)
        {
            /*
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string fieldsql;
            int i;
            System.Web.UI.WebControls.TableRow row = new System.Web.UI.WebControls.TableRow(); ;
            System.Web.UI.WebControls.TableCell cell;
            System.Data.SqlClient.SqlDataReader reader;
            int cellcount = 0;
            fieldsql = createPrintHeaderSQL(quickentryid);


            if (fieldsql == "")
            {
                return null;
            }
            else
            {
                fieldsql += " where employees.employeeid = " + employeeid;
            }
            DataSet ds = expdata.GetDataSet(fieldsql);
            return ds;
            */
            return this.createPrintHeader(quickentryid, employeeid);
        }

        public ArrayList getAvailableFields(int quickentryid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
			cMisc clsmisc = new cMisc(this.accountid);
			
			ArrayList fieldlst = new ArrayList();

            if (clsmisc.GetGeneralFieldByCode("otherdetails").display)
			{
                fieldlst.Add(clsmisc.GetGeneralFieldByCode("otherdetails").fieldid);
			}
            if (clsmisc.GetGeneralFieldByCode("currency").display)
			{
                fieldlst.Add(clsmisc.GetGeneralFieldByCode("currency").fieldid);
			}
            if (clsmisc.GetGeneralFieldByCode("country").display)
			{
                fieldlst.Add(clsmisc.GetGeneralFieldByCode("country").fieldid);
			}
            if (clsmisc.GetGeneralFieldByCode("reason").display)
			{
                fieldlst.Add(clsmisc.GetGeneralFieldByCode("reason").fieldid);
			}
			
			this.strsql = "select columnid from quick_entry_columns where columntype = 1 and quickentryid = @quickentryid";
			expdata.sqlexecute.Parameters.AddWithValue("@quickentryid",quickentryid);

		    using (SqlDataReader reader = expdata.GetReader(this.strsql))
		    {
		        expdata.sqlexecute.Parameters.Clear();
		        while (reader.Read())
		        {
		            if (fieldlst.Contains(reader.GetGuid(0)))
		            {
		                fieldlst.Remove(reader.GetGuid(0));
		            }
		        }
		        reader.Close();
		    }

		    return fieldlst;
		}

        public object[,] getAvailableFieldsForPrintout(int quickentryid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            
            object[,] fields;

            cFields cachedFields = new cFields(this.accountid);
            List<cField> lstPrintOutFields = cachedFields.getPrintoutFields();
            List<Guid> lstUsedFields = new List<Guid>();

            expdata.sqlexecute.Parameters.AddWithValue("@quickentryid", quickentryid);
            this.strsql = "SELECT fieldid FROM quick_entry_printout WHERE quickentryid=@quickentryid AND fieldid IS NOT NULL";

            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                while (reader.Read())
                {
                    lstUsedFields.Add(reader.GetGuid(0));
                }
                reader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();
            int totalFieldCount = lstPrintOutFields.Count - lstUsedFields.Count;

            fields = new object[totalFieldCount, 2];
            int i = 0;
            foreach (cField field in lstPrintOutFields)
            {
                if (!lstUsedFields.Contains(field.FieldID))
                {
                    fields[i, 0] = field.FieldID;
                    fields[i, 1] = field.Description;
                    i++;
                }
            }

            
            return fields;
        }

        public object[,] getAvailableSubcats(int quickentryid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
			int count;
			int i;
            
            this.strsql = "select count(*) from subcats where fromapp = 0 and toapp = 0 and subcatid not in (select subcatID from quick_entry_columns where columntype = 2 and quickentryid = @quickentryid)";
			
			expdata.sqlexecute.Parameters.AddWithValue("@quickentryid",quickentryid);
			count = expdata.getcount(this.strsql);

			object[,] subcats = new object[count,2];

            this.strsql = "select subcatid, subcat from subcats where fromapp = 0 and toapp = 0 and subcatid not in (select subcatID from quick_entry_columns where columntype = 2 and quickentryid = @quickentryid) order by subcat";

		    using (SqlDataReader reader = expdata.GetReader(this.strsql))
		    {
		        i = 0;
		        while (reader.Read())
		        {
		            subcats[i, 0] = reader.GetInt32(0);
		            subcats[i, 1] = reader.GetString(1);
		            i++;
		        }
		        reader.Close();
		    }

		    expdata.sqlexecute.Parameters.Clear();

			return subcats;
		}

        public cQeForm getFormById(int quickentryid)
        {
            return (cQeForm)this.list[quickentryid];
        }

        public DataSet getGrid()
        {
            DataSet ds = new DataSet();
            int i;
            object[] values;
            cQeForm reqform;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("quickentryid",Type.GetType("System.Int32"));
            tbl.Columns.Add("name",Type.GetType("System.String"));
            tbl.Columns.Add("description",Type.GetType("System.String"));
			
            for (i = 0; i < this.list.Count; i++)
            {
                reqform = (cQeForm)this.list.GetByIndex(i);
                values = new object[3];
                values[0] = reqform.quickentryid;
                values[1] = reqform.name;
                values[2] = reqform.description;
                tbl.Rows.Add(values);
            }
            ds.Tables.Add(tbl);
            return ds;
        }

        public int updateForm(int quickentryid, string name, string description, bool genmonth, int numrows)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            if (this.alreadyExists(name,quickentryid,2))
            {
                return -1;
            }

            this.strsql = "update quick_entry_forms set [name] = @name, description = @description, genmonth = @genmonth, numrows = @numrows where quickentryid = @quickentryid";
            expdata.sqlexecute.Parameters.AddWithValue("@name",name);
            expdata.sqlexecute.Parameters.AddWithValue("@description",description);
            expdata.sqlexecute.Parameters.AddWithValue("@quickentryid",quickentryid);
            expdata.sqlexecute.Parameters.AddWithValue("@genmonth",Convert.ToByte(genmonth));
            expdata.sqlexecute.Parameters.AddWithValue("@numrows",numrows);
            expdata.ExecuteSQL(this.strsql);
            expdata.sqlexecute.Parameters.Clear();

            ResetCache();
			
            return 0;
        }

        #endregion

        #region Methods

        private SortedList CacheList()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string name, description;
            int quickentryid, numrows;
            bool genmonth;
            SortedList list = new SortedList();
            cQeForm reqform;
            SortedList<int, List<cQePrintoutField>> lstPrintout = this.getPrintoutFields();
            List<cQePrintoutField> printout;

            SortedList<int, List<cQeColumn>> lstColumns = this.getColumns();
            List<cQeColumn> columns;
            this.strsql = "select quickentryid, name, description, genmonth, numrows from dbo.quick_entry_forms";
            expdata.sqlexecute.CommandText = this.strsql;

            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    quickentryid = reader.GetInt32(reader.GetOrdinal("quickentryid"));
                    name = reader.GetString(reader.GetOrdinal("name"));
                    if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                    {
                        description = reader.GetString(reader.GetOrdinal("description"));
                    }
                    else
                    {
                        description = "";
                    }
                    genmonth = reader.GetBoolean(reader.GetOrdinal("genmonth"));
                    numrows = reader.GetInt32(reader.GetOrdinal("numrows"));
                    lstPrintout.TryGetValue(quickentryid, out printout);
                    if (printout == null)
                    {
                        printout = new List<cQePrintoutField>();
                    }
                    lstColumns.TryGetValue(quickentryid, out columns);
                    if (columns == null)
                    {
                        columns = new List<cQeColumn>();
                    }
                    reqform = new cQeForm(this.accountid, quickentryid, name, description, genmonth, numrows, printout.ToArray(), columns.ToArray());
                    list.Add(quickentryid, reqform);
                }
                reader.Close();
            }

            this.cache.Add(this.nAccountid, CacheArea, "0", list);

            return list;
        }

        private void InitialiseData()
        {
            this.list = this.cache.Get(this.nAccountid, CacheArea, "0") as SortedList ?? this.CacheList();
        }

        private void ResetCache()
        {
            this.cache.Delete(this.nAccountid, CacheArea, "0");
            this.list = null;
        }

        private bool alreadyExists(string name, int quickentryid, int action)
        {
            cQeForm reqform;
            int i;

            for (i = 0; i < this.list.Count; i++)
            {
                reqform = (cQeForm)this.list.GetByIndex(i);
                if (action == 0)
                {
                    if (reqform.name.ToLower().Trim() == name.ToLower().Trim())
                    {
                        return true;
                    }
                }
                else
                {
                    if (reqform.name.ToLower().Trim() == name.ToLower().Trim() && quickentryid != reqform.quickentryid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private DataSet createPrintHeader(int quickentryid, int employeeID)
        {
            
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            
            expdata.sqlexecute.Parameters.AddWithValue("@quickentryid", quickentryid);
            this.strsql = "SELECT fieldid, freefield FROM quick_entry_printout WHERE quickentryid=@quickentryid";

            cFields cachedFields = new cFields(this.accountid);
            cTables cachedTables = new cTables(this.accountid);
            //cJoins cachedJoins = new cJoins(accountid, cAccounts.getConnectionString(accountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, new cTables(accountid), new cFields(accountid));
            cJoins cachedJoins = new cJoins(this.accountid);

            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();

            tbl.Columns.Add("description");
            
            StringBuilder sqlSelect = new StringBuilder("SELECT ");
            StringBuilder sqlJoins = new StringBuilder();
            SortedList<Guid, cField> usedFieldIDs = new SortedList<Guid, cField>();


            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        cField reqField = cachedFields.GetFieldByID(reader.GetGuid(0));
                        cTable reqTable = cachedTables.GetTableByID(reqField.TableID);
                        //cJoin reqJoin = cachedJoins.createJoinSQL();
                        tbl.Columns.Add(reqField.Description);
                        sqlSelect.Append(reqTable.TableName + "." + reqField.FieldName + " AS [" + reqField.Description + "],");

                        usedFieldIDs.Add(Guid.NewGuid(), reqField);
                    }
                    else
                    {
                        tbl.Columns.Add(reader.GetString(1));
                        sqlSelect.Append("NULL,");
                    }
                }
                reader.Close();
            }

            sqlSelect = sqlSelect.Remove(sqlSelect.Length - 1, 1);

            string joins = cachedJoins.createJoinSQL(usedFieldIDs, new Guid("618db425-f430-4660-9525-ebab444ed754"), null);

            sqlSelect.Append(" FROM employees ");

            sqlSelect.Append(joins);

            this.strsql = sqlSelect + "AND employees.employeeid=" + employeeID;

            DataSet ds_data = expdata.GetDataSet(this.strsql);
            
            ds.Tables.Add(tbl);

            expdata.sqlexecute.Parameters.Clear();

            //return ds;
            return ds_data;
        }

      private SortedList<int, List<cQeColumn>> getColumns()
        {
            SortedList<int, List<cQeColumn>> lst = new SortedList<int, List<cQeColumn>>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            
            string strsql;
            int quickentryid;

            List<cQeColumn> columns = new List<cQeColumn>();

            strsql = "SELECT quickentryid, columntype, columnid, [order], subcatID FROM quick_entry_columns order by [order]";
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    quickentryid = reader.GetInt32(reader.GetOrdinal("quickentryid"));
                    lst.TryGetValue(quickentryid, out columns);
                    if (columns == null)
                    {
                        columns = new List<cQeColumn>();
                        lst.Add(quickentryid, columns);
                    }
                    if (reader.GetByte(1) == 1) //field
                    {
                        columns.Add(new cQeFieldColumn(this.accountid, reader.GetGuid(2), reader.GetByte(3)));
                    }
                    else
                    {
                        columns.Add(new cQeSubcatColumn(this.nAccountid, reader.GetInt32(4), reader.GetByte(3)));
                    }

                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();

            return lst;

        }

        private SortedList<int, List<cQePrintoutField>> getPrintoutFields()
        {
            SortedList<int, List<cQePrintoutField>> lst = new SortedList<int, List<cQePrintoutField>>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string strsql;
            
            
            Guid fieldid;
            string freefield;
            byte position;
            int order, quickentryid;


            List<cQePrintoutField> fields = new List<cQePrintoutField>();


            strsql = "select * from quick_entry_printout order by [order]";
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    quickentryid = reader.GetInt32(reader.GetOrdinal("quickentryid"));
                    if (reader.IsDBNull(reader.GetOrdinal("fieldid")) == false)
                    {
                        fieldid = reader.GetGuid(reader.GetOrdinal("fieldid"));
                    }
                    else
                    {
                        fieldid = Guid.Empty;
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("freefield")) == false)
                    {
                        freefield = reader.GetString(reader.GetOrdinal("freefield"));
                    }
                    else
                    {
                        freefield = "";
                    }
                    position = reader.GetByte(reader.GetOrdinal("position"));
                    order = reader.GetInt32(reader.GetOrdinal("order"));
                    lst.TryGetValue(quickentryid, out fields);
                    if (fields == null)
                    {
                        fields = new List<cQePrintoutField>();
                        lst.Add(quickentryid, fields);
                    }
                    fields.Add(new cQePrintoutField(this.accountid, (Position)position, fieldid, freefield));
                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            return lst;
        }

        #endregion
	}

    [Serializable]
	public class cQeForm
	{
        #region Fields

        private readonly bool bGenmonth;

        private readonly int nAccountid;

        private readonly int nNumrows;

        private readonly int nQuickentryid;

        private readonly string sDescription;

        private readonly string sName;

        private cQeColumn[] clscolumns;
		private cQePrintoutField[] clsprintout;

        #endregion

        #region Constructors and Destructors

        public cQeForm (int accountid, int quickentryid, string name, string description, bool genmonth, int numrows, cQePrintoutField[] printout, cQeColumn[] columns)
		{
			this.nAccountid = accountid;
			this.nQuickentryid = quickentryid;
			this.sName = name;
			this.sDescription = description;
			this.bGenmonth = genmonth;
			this.nNumrows = numrows;
            this.clscolumns = columns;
            this.clsprintout = printout;
		}

        #endregion

        #region Public Properties

        public int accountid
        {
            get { return this.nAccountid; }
        }

        public cQeColumn[] columns
		{
			get {return this.clscolumns;}
		}

        public string description
        {
            get 
            {
                return this.sDescription;
            }
        }

        public bool genmonth
		{
			get {return this.bGenmonth;}
		}

        public string name
        {
            get {return this.sName;}
        }

        public int numrows
		{
			get {return this.nNumrows;}
		}
		public cQePrintoutField[] printout
		{
			get {return this.clsprintout;}
		}

        public int quickentryid
        {
            get {return this.nQuickentryid;}
        }

        #endregion

        #region Public Methods and Operators

        public DataSet getColumnGrid()
        {
            cQeFieldColumn fieldcol;
            cQeSubcatColumn subcatcol;
            object[] values;
            int i;
            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();
            tbl.Columns.Add("order", Type.GetType("System.Byte"));
            tbl.Columns.Add("columnname", Type.GetType("System.String"));

            for (i = 0; i < this.columns.Length; i++)
            {
                values = new object[2];
                values[0] = this.columns[i].order;
                if (this.columns[i].GetType().ToString() == "expenses.cQeFieldColumn")
                {
                    fieldcol = (cQeFieldColumn)this.columns[i];
                    values[1] = fieldcol.field.Description;
                }
                else
                {
                    subcatcol = (cQeSubcatColumn)this.columns[i];
                    values[1] = subcatcol.subcat.subcat;
                }
                tbl.Rows.Add(values);

            }

            ds.Tables.Add(tbl);
            return ds;
        }

        public void updateColumns(object[,] columns)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            int i;
            string strsql;
			
            this.clscolumns = new cQeColumn[columns.GetLength(0)];
            strsql = "delete from quick_entry_columns where quickentryid = @quickentryid";
            expdata.sqlexecute.Parameters.AddWithValue("@quickentryid", this.quickentryid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            strsql = "insert into quick_entry_columns (quickentryid, columntype, columnid, subcatid, [order]) values (@quickentryid, @columnType, @columnID, @subCatID, @order)";
            expdata.sqlexecute.Parameters.Add("@quickentryid", SqlDbType.Int);
            expdata.sqlexecute.Parameters.Add("@columnType", SqlDbType.Int);
            expdata.sqlexecute.Parameters.Add("@columnID", SqlDbType.UniqueIdentifier);
            expdata.sqlexecute.Parameters.Add("@subCatID", SqlDbType.Int);
            expdata.sqlexecute.Parameters.Add("@order", SqlDbType.Int);

            for (i = 0; i < columns.GetLength(0); i++)
            {

                if ((int)columns[i,0] == 1) //field
                {
                    expdata.sqlexecute.Parameters["@quickentryid"].Value = this.quickentryid;
                    expdata.sqlexecute.Parameters["@columnType"].Value = (int)columns[i, 0];
                    expdata.sqlexecute.Parameters["@columnID"].Value = new Guid(columns[i, 1].ToString());
                    expdata.sqlexecute.Parameters["@subCatID"].Value = DBNull.Value;
                    expdata.sqlexecute.Parameters["@order"].Value = (i + 1);
                    this.clscolumns[i] = new cQeFieldColumn(this.accountid, new Guid(columns[i, 1].ToString()), (i + 1));
                }
                else
                {
                    expdata.sqlexecute.Parameters["@quickentryid"].Value = this.quickentryid;
                    expdata.sqlexecute.Parameters["@columnType"].Value = (int)columns[i, 0];
                    expdata.sqlexecute.Parameters["@columnID"].Value = DBNull.Value;
                    expdata.sqlexecute.Parameters["@subCatID"].Value = Convert.ToInt32(columns[i, 1]);
                    expdata.sqlexecute.Parameters["@order"].Value = (i + 1);
                    this.clscolumns[i] = new cQeSubcatColumn(this.nAccountid, Convert.ToInt32(columns[i, 1]), (i + 1));
                }
                expdata.ExecuteSQL(strsql);
            }

            expdata.sqlexecute.Parameters.Clear();

            ResetCache();
        }


        public void updatePrintout(object[,] fields)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string strsql;
            int i;

            this.clsprintout = new cQePrintoutField[fields.GetLength(0)];
            strsql = "delete from quick_entry_printout where quickentryid = @quickentryid";
            expdata.sqlexecute.Parameters.AddWithValue("@quickentryid",this.quickentryid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            strsql = "INSERT INTO quick_entry_printout (quickentryid, [position], [order], fieldid, freefield) VALUES (@quickentryid, @position, @order, @fieldid, @freefield)";
            expdata.sqlexecute.Parameters.Add("@quickentryid", SqlDbType.Int);
            expdata.sqlexecute.Parameters.Add("@position", SqlDbType.TinyInt);
            expdata.sqlexecute.Parameters.Add("@order", SqlDbType.Int);
            expdata.sqlexecute.Parameters.Add("@fieldid", SqlDbType.UniqueIdentifier);
            expdata.sqlexecute.Parameters.Add("@freefield", SqlDbType.VarChar);

            for (i = 0; i < fields.GetLength(0); i++)
            {
                expdata.sqlexecute.Parameters["@quickentryid"].Value = this.quickentryid;
                expdata.sqlexecute.Parameters["@position"].Value = fields[i, 0];
                expdata.sqlexecute.Parameters["@order"].Value = (i + 1);

                if (fields[i, 1].ToString() == "0")
                {
                    expdata.sqlexecute.Parameters["@fieldid"].Value = DBNull.Value;
                    expdata.sqlexecute.Parameters["@freefield"].Value = fields[i, 2].ToString();

                    this.clsprintout[i] = new cQePrintoutField(this.accountid, (Position)fields[i, 0], Guid.Empty, (string)fields[i, 2]);
                }
                else
                {
                    expdata.sqlexecute.Parameters["@fieldid"].Value = new Guid(fields[i, 1].ToString());
                    expdata.sqlexecute.Parameters["@freefield"].Value = DBNull.Value;

                    this.clsprintout[i] = new cQePrintoutField(this.accountid, (Position)fields[i, 0], new Guid(fields[i, 1].ToString()), (string)fields[i, 2]);
                }
				
                expdata.ExecuteSQL(strsql);
            }
            expdata.sqlexecute.Parameters.Clear();
        }

        #endregion

        #region Private Methods

        private void ResetCache()
        {
            new Cache().Delete(this.nAccountid, cQeForms.CacheArea, "0");
        }
        
        #endregion
	}

    [Serializable]
	public class cQeColumn
	{
        #region Fields

        protected int nOrder;

        #endregion

        #region Public Properties

        public int order
		{
			get { return this.nOrder; }
		}

        #endregion
	}

    [Serializable]
	public class cQeFieldColumn : cQeColumn
	{
        #region Fields

        private readonly cField clsfield;

        #endregion

        #region Constructors and Destructors

        public cQeFieldColumn (int accountid, Guid fieldid, int order)
		{
			cFields clsfields = new cFields(accountid);
            this.clsfield = clsfields.GetFieldByID(fieldid);
			this.nOrder = order;
		}

        #endregion

        #region Public Properties

        public cField field
		{
			get 
			{
				return this.clsfield;
			}
		}

        #endregion
	}

    [Serializable]
	public class cQeSubcatColumn : cQeColumn
	{
        #region Fields

        private readonly cSubcat clssubcat;

        #endregion

        #region Constructors and Destructors

        public cQeSubcatColumn (int accountid, int subcatid, int order)
		{
			cSubcats clssubcats = new cSubcats(accountid);
			this.clssubcat = clssubcats.GetSubcatById(subcatid);
			this.nOrder = order;
		}

        #endregion

        #region Public Properties

        public cSubcat subcat
		{
			get {return this.clssubcat;}
		}

        #endregion
	}
    [Serializable]
	public class cQePrintoutField
	{
        #region Fields

        private readonly cField clsfield;

        private readonly Position pPos;

        private readonly string sFreetext;

        #endregion

        #region Constructors and Destructors

        public cQePrintoutField (int accountid, Position pos, Guid fieldid, string freetext)
		{
			this.pPos = pos;
			cFields clsfields = new cFields(accountid);
            this.clsfield = clsfields.GetFieldByID(fieldid);
			this.sFreetext = freetext;
		}

        #endregion

        #region Public Properties

        public cField field
		{
			get {return this.clsfield;}
		}
		public string freetext
		{
			get {return this.sFreetext;}
		}

        public Position pos
        {
            get {return this.pPos;}
        }

        #endregion
	}

	public enum Position
	{
	    Top_Left = 1,
        Top_Centre,
        Top_Right,
        Bottom_Left,
        Bottom_Centre,
        Bottom_Right
	}
	

}
