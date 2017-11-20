using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SpendManagementLibrary
{
    public class cFWDBConnection
    {
        private cFWSettings localFWS = new cFWSettings();
        private DBConnectionType Type;    // 1 = SQL Server, 2 = MS Access
        private string Name;
        private string Server;    // Only required for SQL Server
        private string UserName;       // Only required for SQL Server
        private string Password;       // Only required for SQL Server
        private int Timeout;        // Only required for SQL Server
        private bool Status;     // Open = True, Closed = False
        private System.Data.SqlClient.SqlConnection ActiveConnection;
        private string ActiveConnectionString;
        private System.Data.SqlClient.SqlCommand SQLCmd;
        public DataSet glDBWorkA;
        public DataSet glDBWorkB;
        public DataSet glDBWorkC;
        public DataSet glDBWorkD;
        public DataSet glDBWorkI;
        public DataSet glDBWorkL;
        private DataSet glDBWorkX;
        private System.Collections.SortedList glDBParamList;
        private System.Collections.SortedList glDBTables;
        public int glIdentity;
        public int glNumRowsReturned;
        public Exception glException;
        public string glErrorMessage;
        public bool glError;

        // Areas used for primary read
        public bool FWDbFlag;
        // Field Names
        private System.Collections.SortedList FWDbF;
        // Values
        private System.Collections.SortedList FWDbV;
        // Field Types (after read only)
        private System.Collections.SortedList FWDbT;
        private System.Collections.SortedList FWDbParamList;
        private System.Collections.SortedList FWDbConditionParamList;

        // Areas used for secondary read
        public bool FWDb2Flag;  // True = record read, False = no record read
        private System.Collections.SortedList FWDb2F;
        // Values
        private System.Collections.SortedList FWDb2V;
        // Field Types (after read only)
        private System.Collections.SortedList FWDb2T;

        // Areas used for tertiary read
        public bool FWDb3Flag; // True = record read, False = no record read
        private System.Collections.SortedList FWDb3F;
        // Values
        private System.Collections.SortedList FWDb3V;
        // Field Types (after read only)
        private System.Collections.SortedList FWDb3T;

        private System.Text.StringBuilder Sql;
        private DataSet FWDbRec;
        private UserInfo cUInfo;

        public const int SQLType = 1;
        public const int ACCESSType = 2;
        public const int TMPARRAYMAX = 100;

        public enum DBConnectionType
        {
            None = 0,
            SQLServer_SQLAuthentication = 1,
            OLEDB_WindowsAuthentication = 2,
            ODBC = 3,
            OLEDB_SQLAuthentication = 4
        }

        #region properties
        public UserInfo user_info
        {
            get
            {
                return cUInfo;
            }
            set
            {
                cUInfo = value;
            }
        }

        private cFWSettings DB_FWS
        {
            get
            {
                return localFWS;
            }
            set
            {
                localFWS = value;
            }
        }

        public DBConnectionType DBType
        {
            get
            {
                return Type;
            }
            set
            {
                Type = value;
            }
        }

        public string DBName
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        public string DBServer
        {
            get
            {
                return Server;
            }
            set
            {
                Server = value;
            }
        }

        public string DBUserName
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
            }
        }

        public string DBPassword
        {
            get
            {
                return Password;
            }
            set
            {
                Password = value;
            }
        }

        public int DBTimeout
        {
            get
            {
                return Timeout;
            }
            set
            {
                Timeout = value;
            }
        }

        public object DBConnection
        {
            get
            {
                return ActiveConnection;
            }
        }

        public bool DBIsOpen
        {
            get
            {
                return Status;
            }
        }
        #endregion

        private void DBSQLCommand()
        {
            if (SQLCmd == null)
            {
                switch (DBType)
                {
                    case DBConnectionType.SQLServer_SQLAuthentication:
                        SQLCmd = new System.Data.SqlClient.SqlCommand();
                        break;
                    default:
                        SQLCmd = new System.Data.SqlClient.SqlCommand();
                        break;
                }
            }

            return;
        }

        public cFWDBConnection(UserInfo ui)
        {
            user_info = ui;
        }

        public cFWDBConnection()
        {
            user_info = new UserInfo();
        }

        private bool InitDBConnection(bool LoadTables)
        {
            try
            {
                FWDbT = new System.Collections.SortedList();
                FWDbT = new System.Collections.SortedList();
                FWDbV = new System.Collections.SortedList();
                FWDbF = new System.Collections.SortedList();
                FWDb2T = new System.Collections.SortedList();
                FWDb2V = new System.Collections.SortedList();
                FWDb2F = new System.Collections.SortedList();
                FWDb3T = new System.Collections.SortedList();
                FWDb3V = new System.Collections.SortedList();
                FWDb3F = new System.Collections.SortedList();
                FWDbParamList = new System.Collections.SortedList();
                FWDbConditionParamList = new System.Collections.SortedList();
                glDBParamList = new System.Collections.SortedList();
                if (LoadTables == true)
                {
                    glDBTables = new System.Collections.SortedList();
                }

                // Initialise work recordsets
                glDBWorkA = new DataSet();

                glDBWorkB = new DataSet();

                glDBWorkC = new DataSet();

                glDBWorkD = new DataSet();

                glDBWorkI = new DataSet();

                glDBWorkL = new DataSet();

                glDBWorkX = new DataSet();

                return true;
            }
            catch (Exception ex)
            {
                glError = true;
                glErrorMessage = ex.Message;
                glException = ex;
                return false;
            }
        }

        private void OpenDBConnection(bool LoadTables)
        {
            try
            {
                ActiveConnection = new System.Data.SqlClient.SqlConnection(ActiveConnectionString);
                ActiveConnection.Open();
                Status = true;

                glError = false;
                glErrorMessage = "";
                glException = null;
            }
            catch (Exception ex)
            {
                glErrorMessage = ex.Message;
                glException = ex;
                glError = true;
            }
            return;
        }

        public void DBOpenMetabase(string connectionString, bool LoadTables)
        {
            if (InitDBConnection(LoadTables))
            {
                ActiveConnection = new SqlConnection(connectionString);
                Type = DBConnectionType.SQLServer_SQLAuthentication;

                OpenDBConnection(LoadTables);
            }
            return;
        }

        public void DBOpen(cFWSettings AppSettings, bool LoadTables)
        {
            if (InitDBConnection(LoadTables))
            {
                if (DBIsOpen)
                {
                    // Database is already open in this instance
                    DB_FWS = AppSettings;
                    return;
                }
                else
                {
                    DB_FWS = AppSettings;
                }

                DBServer = AppSettings.glServer;
                DBName = AppSettings.glDatabase;
                DBUserName = AppSettings.glDBUserId;    //'"FrameworkUser"
                DBPassword = AppSettings.glDBPassword; // '"halstead"
                DBType = (DBConnectionType)AppSettings.glDBEngine;
                DBTimeout = AppSettings.glDBTimeout;

                if (DBName == "" || DBUserName == "" || DBPassword == "" || DBType == 0) //' DBServer() = "" Or
                {
                    glError = true;
                    return;
                }

                if (DBTimeout == 0)
                {
                    DBTimeout = 60;
                }

                string tmpStr;

                tmpStr = "";

                if (Server != "")
                {
                    tmpStr += "Server=" + Server.Trim() + ";";
                }

                tmpStr += string.Format("Initial Catalog={0};Connect Timeout={1};User ID={2};Password={3};Max Pool Size=1000;Persist Security Info=True;" + "Application Name={4};", Name.Trim(), DBTimeout.ToString().Trim(), UserName.Trim(), Password.Trim(), GlobalVariables.DefaultApplicationInstanceName);
                ActiveConnectionString = tmpStr;
                OpenDBConnection(LoadTables);
            }
        }

        public void DBClose()
        {
            FWDbF.Clear();
            FWDbT.Clear();
            FWDbV.Clear();
            FWDb2F.Clear();
            FWDb2T.Clear();
            FWDb2V.Clear();
            FWDb3F.Clear();
            FWDb3T.Clear();
            FWDb3V.Clear();
            glDBWorkA.Dispose();
            glDBWorkB.Dispose();
            glDBWorkC.Dispose();
            glDBWorkD.Dispose();
            glDBWorkI.Dispose();
            glDBWorkL.Dispose();
            glDBWorkX.Dispose();
            glDBParamList = null;
            FWDbParamList = null;
            FWDbConditionParamList = null;
            if (DBIsOpen)
            {
                ActiveConnection.Close();
                ActiveConnection.ConnectionString = "";
                Status = false;
            }
            if (SQLCmd != null)
            {
                SQLCmd.Dispose();
                SQLCmd = null;
            }
        }

        public void RunSQL(string strSQL, DataSet rstResults, bool AppendData, string AppendTableName, bool isFWDb)
        {
            try
            {
                DBSQLCommand();

                // Clear the Dataset passed in or it will append the data!
                if (!AppendData)
                {
                    rstResults.Clear();
                }

                bool isSelect;

                if (strSQL.Trim().Substring(0, 6).ToUpper() == "SELECT")
                {
                    isSelect = true;
                }
                else
                {
                    isSelect = false;
                }

                // add parameters
                SQLCmd.Parameters.Clear();

                int x;
                string fwKey;
                object fwVal;

                if (isFWDb)
                {
                    for (x = 0; x < FWDbParamList.Count; x++)
                    {
                        fwKey = (string)FWDbParamList.GetKey(x);
                        fwVal = FWDbParamList.GetByIndex(x);
                        SQLCmd.Parameters.AddWithValue("@" + fwKey.Trim(), fwVal);
                    }
                }
                else
                {
                    for (x = 0; x < glDBParamList.Count; x++)
                    {
                        fwKey = (string)glDBParamList.GetKey(x);
                        fwVal = glDBParamList.GetByIndex(x);
                        SQLCmd.Parameters.AddWithValue("@" + fwKey.Trim(), fwVal);
                    }
                }

                for (x = 0; x < FWDbConditionParamList.Count; x++)
                {
                    fwKey = (string)FWDbConditionParamList.GetKey(x);
                    fwVal = FWDbConditionParamList.GetByIndex(x);
                    SQLCmd.Parameters.AddWithValue("@" + fwKey.Trim(), fwVal);
                }

                SqlDataAdapter Adapter = new SqlDataAdapter(SQLCmd);

                SQLCmd.CommandText = strSQL;
                SQLCmd.Connection = ActiveConnection;

                if (AppendTableName != "")
                {
                    Adapter.Fill(rstResults, AppendTableName);
                    if (isSelect)
                    {
                        glNumRowsReturned = rstResults.Tables[AppendTableName].Rows.Count;
                    }
                    else
                    {
                        glNumRowsReturned = 0;
                    }
                }
                else
                {
                    Adapter.Fill(rstResults);
                    if (isSelect)
                    {
                        glNumRowsReturned = rstResults.Tables[0].Rows.Count;
                    }
                    else
                    {
                        glNumRowsReturned = 0;
                    }
                }

                SQLCmd.Parameters.Clear();
                Adapter.Dispose();
                SQLCmd.Dispose();
                glError = false;
                glErrorMessage = "";
                glException = null;
            }
            catch (Exception ex)
            {
                glErrorMessage = ex.Message;
                glException = ex;
                glIdentity = 0;
                glNumRowsReturned = -1;
                glError = true;

            }

        }

        public void ExecuteSQL(string sql)
        {
            try
            {
                // execute SQL with no return record set
                DBSQLCommand();

                SQLCmd = new SqlCommand(sql, ActiveConnection);

                int x;
                string fwKey;
                object fwVal;
                for (x = 0; x < glDBParamList.Count; x++)
                {
                    fwKey = (string)glDBParamList.GetKey(x);
                    fwVal = glDBParamList.GetByIndex(x);
                    SQLCmd.Parameters.AddWithValue("@" + fwKey.Trim(), fwVal);
                }

                SQLCmd.ExecuteNonQuery();

                SQLCmd.Dispose();
                glError = false;
                glErrorMessage = null;
            }
            catch (Exception ex)
            {
                glError = true;
                glErrorMessage = ex.Message;
                glException = ex;

                UserInfo uinfo = new UserInfo();
                //cErrors sqlMail = new cErrors(uinfo, localFWS);
                //sqlMail.ReportSQL(sql, FWDbParamList, ex);
                //sqlMail = null;
            }
        }

        public object GetFieldValue(DataSet dset, string FieldName, int RowIdx, int TableIdx)
        {
            object tmp = null;
            DataRow drow = dset.Tables[TableIdx].Rows[RowIdx];
            tmp = drow[FieldName];

            if (tmp == null || tmp == DBNull.Value)
            {
                tmp = GetDefaultVal(GetFieldType(dset, 0, dset.Tables[TableIdx].Columns.IndexOf(FieldName), TableIdx));
            }

            return tmp;
        }

        public int GetRowCount(DataSet dset, int TableIdx)
        {
            return dset.Tables[TableIdx].Rows.Count;
        }

        public string GetColumnName(DataSet dset, int ColumnIdx, int TableIdx)
        {
            return dset.Tables[TableIdx].Columns[ColumnIdx].ColumnName;
        }

        public int GetColumnCount(DataSet dset, int TableIdx)
        {
            return dset.Tables[TableIdx].Columns.Count;
        }

        public string GetFieldType(DataSet dset, int RowIdx, int ColumnIdx, int TableIdx)
        {
            return dset.Tables[TableIdx].Columns[ColumnIdx].DataType.ToString();
        }

        public void FWDb(string DType, string DDb, string DSF1, object DSV1, string DSF2, object DSV2, string DSF3, object DSV3, string DSF4, object DSV4, string DSF5, object DSV5, string DSF6, object DSV6)
        {

            //' Access the database.
            //' DType     - R = Read
            //'             R2 = Read into second area
            //'             W = Write
            //'             A = Amend
            //'             D = Delete
            //' DDb       - The table name
            //' DSF1 - An optional field to use for selection (used by Read, Amend, and Delete)
            //' DSV1 - The value to select.
            //' DSF2,3,4,5  - Further optional fields for selection
            //' DSV2,3,4,5  - Values for DSF2, DSF3, DSF4 and DSF5
            //'   For selection, if the field name is preceded by !, then a <> test is used, otherwise an = test is used...

            int x;
            string comma;
            object CVal;
            string fieldKey;

            FWDbRec = new DataSet();

            Sql = new System.Text.StringBuilder();

            switch (DType.ToUpper())
            {
                case "A":   // Amend record
                    Sql.Append("UPDATE [");
                    Sql.Append(DDb);
                    Sql.Append("]\n");
                    Sql.Append(" SET ");
                    comma = "";
                    for (x = 0; x < FWDbF.Count; x++)
                    {
                        if (FWDbF.GetByIndex(x).ToString() != "")
                        {
                            CVal = ConvertVal(x);
                            fieldKey = ConvertToKey(FWDbF.GetByIndex(x).ToString()).ToLower();

                            Sql.Append(comma);
                            Sql.Append("\n [");
                            Sql.Append(FWDbF.GetByIndex(x));
                            Sql.Append("] = ");
                            switch (FWDbT.GetByIndex(x).ToString())
                            {
                                case "D":
                                    if (CVal != null)
                                    {
                                        Sql.Append("CONVERT(datetime,@" + fieldKey + ",120) ");
                                    }
                                    else
                                    {
                                        CVal = DBNull.Value;
                                        Sql.Append("@" + fieldKey);
                                    }
                                    break;
                                default:
                                    Sql.Append("@" + fieldKey);
                                    break;
                            }

                            if (!FWDbParamList.Contains(fieldKey))
                            {
                                FWDbParamList.Add(fieldKey, CVal);
                            }
                            else
                            {
                                FWDbParamList[fieldKey] = CVal;
                            }

                            comma = ",";
                        }
                    }
                    Sql.Append(Where(DSF1, DSV1, DSF2, DSV2, DSF3, DSV3, DSF4, DSV4, DSF5, DSV5, DSF6, DSV6));
                    RunSQL(Sql.ToString(), FWDbRec, false, "", true);
                    break;
                case "D":
                    Sql.Append("DELETE FROM [");
                    Sql.Append(DDb);
                    Sql.Append("]\n");
                    Sql.Append(Where(DSF1, DSV1, DSF2, DSV2, DSF3, DSV3, DSF4, DSV4, DSF5, DSV5, DSF6, DSV6));
                    RunSQL(Sql.ToString(), FWDbRec, false, "", true);
                    break;
                case "R":  // Read record
                    FWDbF.Clear();
                    FWDbV.Clear();
                    FWDbT.Clear();

                    Sql.Append("SELECT [");
                    Sql.Append(DDb);
                    Sql.Append("].* FROM [");
                    Sql.Append(DDb + "]");
                    Sql.Append(Where(DSF1, DSV1, DSF2, DSV2, DSF3, DSV3, DSF4, DSV4, DSF5, DSV5, DSF6, DSV6));
                    RunSQL(Sql.ToString(), FWDbRec, false, "", true);

                    if (GetRowCount(FWDbRec, 0) == 0)
                    {
                        FWDbFlag = false;
                    }
                    else
                    {
                        FWDbFlag = true;
                        for (x = 0; x < GetColumnCount(FWDbRec, 0); x++)
                        {
                            FWDbPopulateReturnFields(1, FWDbRec, x);
                        }
                    }
                    break;
                case "R2":  // Read record
                    FWDb2F.Clear();
                    FWDb2V.Clear();
                    FWDb2T.Clear();

                    Sql.Append("SELECT * FROM [");
                    Sql.Append(DDb + "]");
                    Sql.Append(Where(DSF1, DSV1, DSF2, DSV2, DSF3, DSV3, DSF4, DSV4, DSF5, DSV5, DSF6, DSV6));
                    RunSQL(Sql.ToString(), FWDbRec, false, "", true);

                    if (GetRowCount(FWDbRec, 0) == 0)
                    {
                        FWDb2Flag = false;
                    }
                    else
                    {
                        FWDb2Flag = true;
                        for (x = 0; x < GetColumnCount(FWDbRec, 0); x++)
                        {
                            FWDbPopulateReturnFields(2, FWDbRec, x);
                        }
                    }
                    break;

                case "R3":  // Read record
                    FWDb3F.Clear();
                    FWDb3V.Clear();
                    FWDb3T.Clear();

                    Sql.Append("SELECT * FROM [");
                    Sql.Append(DDb);
                    Sql.Append("]");
                    Sql.Append(Where(DSF1, DSV1, DSF2, DSV2, DSF3, DSV3, DSF4, DSV4, DSF5, DSV5, DSF6, DSV6));
                    RunSQL(Sql.ToString(), FWDbRec, false, "", true);

                    if (GetRowCount(FWDbRec, 0) == 0)
                    {
                        FWDb3Flag = false;
                    }
                    else
                    {
                        FWDb3Flag = true;
                        for (x = 0; x < GetColumnCount(FWDbRec, 0); x++)
                        {
                            FWDbPopulateReturnFields(3, FWDbRec, x);
                        }
                    }
                    break;

                case "W":
                case "WX": // Write new record
                    Sql.Append("INSERT INTO [");
                    Sql.Append(DDb);
                    Sql.Append("]\n");
                    Sql.Append("(");
                    comma = "[";
                    for (x = 0; x < FWDbF.Count; x++)
                    {
                        string tmpStr = FWDbF.GetByIndex(x).ToString();
                        if (tmpStr != "")
                        {
                            Sql.Append(comma);
                            Sql.Append(FWDbF.GetByIndex(x));
                            Sql.Append("]");
                            comma = ", [";
                        }
                    }
                    comma = "";
                    Sql.Append(")\n");
                    Sql.Append("VALUES (");
                    for (x = 0; x < FWDbF.Count; x++)
                    {
                        if (FWDbF.GetByIndex(x).ToString() != "")
                        {
                            fieldKey = ConvertToKey(FWDbF.GetByIndex(x).ToString()).ToLower();
                            CVal = ConvertVal(x);

                            Sql.Append(comma);
                            switch (FWDbT.GetByIndex(x).ToString())
                            {
                                case "D":

                                    if (CVal != null && CVal.GetType() != typeof(DateTime))
                                    {
                                        Sql.Append("CONVERT(datetime,@" + fieldKey + ",120) ");
                                    }
                                    else
                                    {
                                        Sql.Append("@" + fieldKey);
                                    }
                                    if (CVal == null)
                                    {
                                        CVal = DBNull.Value;
                                    }
                                    FWDbParamList.Add(fieldKey, CVal);
                                    break;
                                case "G":
                                    Sql.Append("CONVERT(uniqueidentifier, @" + fieldKey + ") ");
                                    FWDbParamList.Add(fieldKey, CVal.ToString());
                                    break;
                                default:
                                    Sql.Append("@" + fieldKey);
                                    FWDbParamList.Add(fieldKey, CVal);
                                    break;
                            }

                            comma = ", ";
                        }
                    }
                    Sql.Append(")");

                    if (DType.ToUpper() == "W")
                    {
                        Sql.Append(" SELECT @@Identity AS 'identity'");
                    }

                    RunSQL(Sql.ToString(), FWDbRec, false, "", true);

                    if (DType.ToUpper() == "W")
                    {
                        glIdentity = int.Parse(GetFieldValue(FWDbRec, "identity", 0, 0).ToString());
                    }
                    break;
                default:
                    break;

            }
            FWDbRec.Dispose();
        }

        private object ConvertVal(int Ptr)
        {
            // Convert the value to the correct format for SQL insert or amend
            // Note that any single quotes (') in strings are replaced with single left quotes (`)
            //DateTime CD;
            object retVal;

            switch (FWDbT.GetByIndex(Ptr).ToString())
            {
                case "D":
                case "d":   //Date
                    if (FWDbV.GetByIndex(Ptr) == DBNull.Value)
                    {
                        retVal = null; // "NULL";
                    }
                    else
                    {
                        DateTime tmpDate;
                        if (DateTime.TryParse(FWDbV.GetByIndex(Ptr).ToString(), out tmpDate))
                        {
                            retVal = tmpDate;
                        }
                        else
                        {
                            retVal = null; //"NULL";
                        }
                    }
                    break;
                case "N":
                case "n":   // Numeric
                    if (FWDbV.GetByIndex(Ptr) == DBNull.Value)
                    {
                        retVal = 0;
                    }
                    else
                    {
                        retVal = FWDbV.GetByIndex(Ptr);
                    }
                    break;
                case "S":
                case "s":   // String
                    if (FWDbV.GetByIndex(Ptr) == DBNull.Value || FWDbV.GetByIndex(Ptr) == null)
                    {
                        return "";
                    }
                    else
                    {
                        retVal = FWDbV.GetByIndex(Ptr);
                        if (retVal == null)
                        {
                            retVal = "";
                        }
                    }
                    break;
                case "B":
                case "b": // boolean
                    if (FWDbV.GetByIndex(Ptr) == DBNull.Value)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        if (FWDbV.GetByIndex(Ptr).ToString().ToLower() == "true")
                        {
                            retVal = "1";
                        }
                        else
                        {
                            retVal = "0";
                        }
                    }
                    break;
                case "#":
                    retVal = DBNull.Value;
                    break;
                case "G":
                case "g": // GUID
                    if (FWDbV.GetByIndex(Ptr) == DBNull.Value || FWDbV.GetByIndex(Ptr).ToString().Trim() == "")
                    {
                        retVal = Guid.Empty;
                    }
                    else
                    {
                        retVal = new Guid(FWDbV.GetByIndex(Ptr).ToString());
                    }
                    break;
                default:
                    retVal = "";
                    break;
            }

            return retVal;
        }

        public string FWDbFindVal(string DName, int DBase)
        {
            // Find the value for a corresponding name (assuming the record has been read!)
            // DName = name of field to find
            // DBase = 1 for main areas, 2 for secondary areas
            string fieldKey;
            string retVal = "";

            fieldKey = ConvertToKey(DName).ToLower();

            switch (DBase)
            {
                case 1:
                    if (FWDbF.ContainsKey(fieldKey))
                    {
                        retVal = FWDbV[fieldKey].ToString();
                    }
                    break;
                case 2:
                    if (FWDb2F.ContainsKey(fieldKey))
                    {
                        retVal = FWDb2V[fieldKey].ToString();
                    }
                    break;
                case 3:
                    if (FWDb3F.ContainsKey(fieldKey))
                    {
                        retVal = FWDb3V[fieldKey].ToString();
                    }
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public string Where(string DSF1, object DSV1, string DSF2, object DSV2, string DSF3, object DSV3, string DSF4, object DSV4, string DSF5, object DSV5, string DSF6, object DSV6)
        {
            // Create and return the WHERE clauses
            StringBuilder TempWhere = new StringBuilder();
            FWDbConditionParamList.Clear();

            if (DSF1 != "")
            {
                TempWhere.Append(Where2("WHERE", DSF1, DSV1));
            }
            if (DSF2 != "")
            {
                TempWhere.Append(Where2("AND", DSF2, DSV2));
            }
            if (DSF3 != "")
            {
                TempWhere.Append(Where2("AND", DSF3, DSV3));
            }
            if (DSF4 != "")
            {
                TempWhere.Append(Where2("AND", DSF4, DSV4));
            }
            if (DSF5 != "")
            {
                TempWhere.Append(Where2("AND", DSF5, DSV5));
            }
            if (DSF6 != "")
            {
                TempWhere.Append(Where2("AND", DSF6, DSV6));
            }
            return TempWhere.ToString();
        }

        public string Where2(string KeyWord, string DSF, object DSV)
        {
            // Create and return an individual WHERE clause
            StringBuilder TempWhere = new StringBuilder();

            if (DSF.StartsWith("!"))
            {
                DSF.Substring(1, DSF.Length - 1);
                TempWhere.Append("\n");
                TempWhere.Append(KeyWord);
                TempWhere.Append(" [");
                TempWhere.Append(DSF);
                TempWhere.Append("] <> ");
                TempWhere.Append("@condition_" + ConvertToKey(DSF));
                FWDbConditionParamList.Add("condition_" + ConvertToKey(DSF), DSV);
            }
            else
            {
                TempWhere.Append("\n");
                TempWhere.Append(KeyWord);
                TempWhere.Append(" [");
                TempWhere.Append(DSF);
                TempWhere.Append("] = ");
                TempWhere.Append("@condition_" + ConvertToKey(DSF));
                FWDbConditionParamList.Add("condition_" + ConvertToKey(DSF), DSV);
            }
            return TempWhere.ToString();
        }

        private void FWDbPopulateReturnFields(int FWArrayNum, DataSet FWField, int FldNum)
        {
            // Move the field from the recordset to the internal array, adjusting dates and money
            string dataType;
            string fieldKey;

            fieldKey = ConvertToKey(GetColumnName(FWField, FldNum, 0)).ToLower();

            switch (FWArrayNum)
            {
                case 1:
                    FWDbF.Add(fieldKey, GetColumnName(FWField, FldNum, 0));
                    dataType = GetFieldType(FWField, 0, FldNum, 0);

                    switch (dataType)
                    {
                        case "System.String":
                            FWDbV.Add(fieldKey, GetFieldValue(FWField, FWDbF[fieldKey].ToString(), 0, 0).ToString().Trim());
                            FWDbT.Add(fieldKey, "S");
                            break;
                        case "System.Boolean":
                            if (GetFieldValue(FWField, FWDbF[fieldKey].ToString(), 0, 0).ToString().ToLower() == "true")
                            {
                                FWDbV.Add(fieldKey, "1");
                            }
                            else
                            {
                                FWDbV.Add(fieldKey, "0");
                            }
                            FWDbT.Add(fieldKey, "N");
                            break;
                        case "System.Int32":
                            FWDbV.Add(fieldKey, GetFieldValue(FWField, FWDbF[fieldKey].ToString(), 0, 0));
                            FWDbT.Add(fieldKey, "N");
                            break;
                        default:
                            FWDbV.Add(fieldKey, GetFieldValue(FWField, FWDbF[fieldKey].ToString(), 0, 0).ToString());
                            FWDbT.Add(fieldKey, "S");
                            break;
                    }
                    break;
                case 2:
                    FWDb2F.Add(fieldKey, GetColumnName(FWField, FldNum, 0));
                    dataType = GetFieldType(FWField, 0, FldNum, 0);

                    switch (dataType)
                    {
                        case "System.String":
                            FWDb2V.Add(fieldKey, GetFieldValue(FWField, FWDb2F[fieldKey].ToString(), 0, 0).ToString().Trim());
                            FWDb2T.Add(fieldKey, "S");
                            break;
                        case "System.Boolean":
                            if (GetFieldValue(FWField, FWDb2F[fieldKey].ToString(), 0, 0).ToString().ToLower() == "true")
                            {
                                FWDb2V.Add(fieldKey, 1);
                            }
                            else
                            {
                                FWDb2V.Add(fieldKey, 0);
                            }
                            FWDb2T.Add(fieldKey, "N");
                            break;
                        case "System.Int32":
                            FWDb2V.Add(fieldKey, GetFieldValue(FWField, FWDb2F[fieldKey].ToString(), 0, 0));
                            FWDb2T.Add(fieldKey, "N");
                            break;
                        default:
                            FWDb2V.Add(fieldKey, GetFieldValue(FWField, FWDb2F[fieldKey].ToString(), 0, 0).ToString());
                            FWDb2T.Add(fieldKey, "S");
                            break;
                    }
                    break;
                case 3:
                    FWDb3F.Add(fieldKey, GetColumnName(FWField, FldNum, 0));
                    dataType = GetFieldType(FWField, 0, FldNum, 0);

                    switch (dataType)
                    {
                        case "System.String":
                            FWDb3V.Add(fieldKey, GetFieldValue(FWField, FWDb3F[fieldKey].ToString(), 0, 0).ToString().Trim());
                            FWDb3T.Add(fieldKey, "S");
                            break;
                        case "System.Boolean":
                            if (GetFieldValue(FWField, FWDb3F[fieldKey].ToString(), 0, 0).ToString().ToLower() == "true")
                            {
                                FWDb3V.Add(fieldKey, 1);
                            }
                            else
                            {
                                FWDb3V.Add(fieldKey, 0);
                            }
                            FWDb3T.Add(fieldKey, "N");
                            break;
                        case "System.Int32":
                            FWDb3V.Add(fieldKey, GetFieldValue(FWField, FWDb3F[fieldKey].ToString(), 0, 0));
                            FWDb3T.Add(fieldKey, "N");
                            break;
                        default:
                            FWDb3V.Add(fieldKey, GetFieldValue(FWField, FWDb3F[fieldKey].ToString(), 0, 0).ToString());
                            FWDb3T.Add(fieldKey, "S");
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetFieldValue(string DbField, object DbVal, string DType, bool FWDbClear)
        {
            // This routine moves a literal value to the FWDb arrays
            // DbField   - The database field
            // DbVal     - the value for the field
            // DType     - The data type: S = String, N = Numeric, D = Date, G = GUID, # = NULL
            // FWDbClear  - If true, then clear the arrays before starting

            bool FMatch;
            string fieldKey;

            fieldKey = ConvertToKey(DbField).ToLower();

            if (FWDbClear)
            {
                FWDbF.Clear();
                FWDbV.Clear();
                FWDbT.Clear();
                FWDbParamList.Clear();
            }

            // First check if the field already exists FWDbV(x)
            FMatch = FWDbF.ContainsKey(fieldKey);
            if (!FMatch)
            {
                // Set up name and type
                if (DType == "D")    // check for blank date field, set to NULL if is blank
                {
                    DateTime tmpDate;

                    if (!DateTime.TryParse(DbVal.ToString(), out tmpDate))
                    {
                        DbVal = DBNull.Value;
                    }
                    else
                    {
                        DbVal = tmpDate;
                    }
                }

                FWDbF.Add(fieldKey, DbField);
                FWDbT.Add(fieldKey, DType);

                // Set up value
                FWDbV.Add(fieldKey, DbVal);
            }
        }

        private object GetDefaultVal(object VarType)
        {
            object retVal;
            switch (VarType.ToString())
            {
                case "System.String":
                    retVal = string.Empty;
                    break;
                case "System.Int32":
                    retVal = (int)0;
                    break;
                case "System.Float":
                    retVal = (float)0;
                    break;
                case "System.Double":
                    retVal = (double)0;
                    break;
                case "System.Decimal":
                    retVal = (decimal)0;
                    break;
                case "System.Boolean":
                    retVal = false;
                    break;
                case "System.DateTime":
                    retVal = "";
                    break;
                default:
                    retVal = "";
                    break;
            }

            return retVal;
        }

        public void AddDBParam(string paramName, object ParamVal, bool ClearParams)
        {
            string fieldKey;

            if (ClearParams)
            {
                glDBParamList.Clear();
            }

            fieldKey = ConvertToKey(paramName);

            if (!glDBParamList.ContainsKey(fieldKey))
            {
                glDBParamList.Add(fieldKey, ParamVal);
            }
        }

        private string ConvertToKey(string fieldName)
        {
            string tmpKey;

            tmpKey = fieldName.Replace(" ", "_");
            return tmpKey.Replace("-", "_");
        }

        public SqlDataReader GetReader(string strSQL)
        {
            DBSQLCommand();

            int x;
            string fwKey;
            object fwVal;

            if (glDBParamList.Count > 0)
            {
                for (x = 0; x < glDBParamList.Count; x++)
                {
                    fwKey = glDBParamList.GetKey(x).ToString();
                    fwVal = glDBParamList.GetByIndex(x);
                    SQLCmd.Parameters.Add(new SqlParameter("@" + fwKey.Trim(), fwVal));
                }
            }

            SQLCmd.CommandText = strSQL;
            SQLCmd.CommandType = CommandType.Text;
            SQLCmd.Connection = ActiveConnection;
            return SQLCmd.ExecuteReader();
        }

        public int GetCount(string strSQL)
        {
            int retCount = 0;
            DBSQLCommand();

            SQLCmd.Connection = ActiveConnection;
            SQLCmd.CommandText = strSQL;
            SQLCmd.CommandType = System.Data.CommandType.Text;

            if (glDBParamList.Count > 0)
            {
                int x;
                string fwKey;
                object fwVal;

                for (x = 0; x < glDBParamList.Count; x++)
                {
                    fwKey = glDBParamList.GetKey(x).ToString();
                    fwVal = glDBParamList.GetByIndex(x);
                    SQLCmd.Parameters.Add(new SqlParameter("@" + fwKey.Trim(), fwVal));
                }
            }

            retCount = (int)SQLCmd.ExecuteScalar();
            return retCount;
        }

    }
}
