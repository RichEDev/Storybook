namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Caching;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    public class cFinancialExports
    {
        protected int nAccountid = 0;

        protected string strsql;



        protected SortedList<int, cFinancialExport> list;

        public Cache Cache = HttpRuntime.Cache;

        public cFinancialExports(int accountid)
        {
            nAccountid = accountid;

            InitialiseData();
        }

        private void InitialiseData()
        {
            list = (SortedList<int, cFinancialExport>)Cache["financialexports" + accountid];
            if (list == null)
            {
                list = CacheList();
            }
        }

        /// <summary>
        /// Cache the list of financial exports
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cFinancialExport> CacheList()
        {
            SortedList<int, cFinancialExport> lst;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                lst = new SortedList<int, cFinancialExport>();
                int financialexportid, createdby, curexportnum;
                DateTime createdon, lastexportdate;
                byte exporttype;
                FinancialApplication app;
                Guid reportid;
                int NHSTrustID;
                bool automated;


                this.strsql = "SELECT financialexportid, applicationtype, reportid, automated, CreatedBy, CreatedOn, (select count(*) from exporthistory where exporthistory.financialexportid = financial_exports.financialexportid) + 1 as curexportnum, lastexportdate, exporttype, NHSTrustID, PreventNegativeAmountPayable , ExpeditePaymentReport FROM dbo.financial_exports";
                using (var reader = expdata.GetReader(this.strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        financialexportid = reader.GetInt32(reader.GetOrdinal("financialexportid"));
                        app = (FinancialApplication)reader.GetByte(reader.GetOrdinal("applicationtype"));
                        switch (app)
                        {
                            case FinancialApplication.CustomReport:
                            case FinancialApplication.ESR:
                                if (reader.IsDBNull(reader.GetOrdinal("reportid")) == true)
                                {
                                    reportid = Guid.Empty;
                                }
                                else
                                {
                                    reportid = reader.GetGuid(reader.GetOrdinal("reportid"));
                                }
                                break;
                            default:
                                reportid = Guid.Empty;
                                break;
                        }

                        automated = reader.GetBoolean(reader.GetOrdinal("automated"));
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                        curexportnum = reader.GetInt32(reader.GetOrdinal("curexportnum"));
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                        if (reader.IsDBNull(reader.GetOrdinal("lastexportdate")) == true)
                        {
                            lastexportdate = new DateTime(1900, 01, 01);
                        }
                        else
                        {
                            lastexportdate = reader.GetDateTime(reader.GetOrdinal("lastexportdate"));
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("exporttype")) == true)
                        {
                            exporttype = 0;
                        }
                        else
                        {
                            exporttype = reader.GetByte(reader.GetOrdinal("exporttype"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("NHSTrustID")) == true)
                        {
                            NHSTrustID = 0;
                        }
                        else
                        {
                            NHSTrustID = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                        }

                        var preventNegativePayments = !reader.IsDBNull(reader.GetOrdinal("PreventNegativeAmountPayable")) && reader.GetBoolean(reader.GetOrdinal("PreventNegativeAmountPayable"));
                        var expeditePaymentReport = !reader.IsDBNull(reader.GetOrdinal("ExpeditePaymentReport")) && reader.GetBoolean(reader.GetOrdinal("ExpeditePaymentReport"));
                        lst.Add(financialexportid, new cFinancialExport(financialexportid, this.accountid, app, reportid, automated, createdby, createdon, curexportnum, lastexportdate, exporttype, NHSTrustID, preventNegativePayments, expeditePaymentReport));
                    }

                    reader.Close();
                }
            }

            return lst;
        }

        /// <summary>
        /// Force an update of the cache in this object
        /// </summary>
        public void ResetCache()
        {
            Cache.Remove("financialexports" + accountid);
            list = null;
            InitialiseData();
        }

        public cFinancialExport getExportById(int id)
        {
            cFinancialExport export;
            this.list.TryGetValue(id, out export);
            return export;
        }

        public int addFinancialExport(cFinancialExport export)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@applicationtype", (byte)export.application);

            expdata.sqlexecute.Parameters.AddWithValue("@reportid", export.reportid);

            expdata.sqlexecute.Parameters.AddWithValue("@automated", Convert.ToBoolean(export.automated));
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", export.createdby);
            expdata.sqlexecute.Parameters.AddWithValue("@expeditePaymentReport", export.ExpeditePaymentReport);
            if (export.application == FinancialApplication.CustomReport)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@exporttype", export.exporttype);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@exporttype", DBNull.Value);
            }

            if (export.NHSTrustID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", export.NHSTrustID);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("addFinancialExport");

            int financialexportid = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            expdata.sqlexecute.Parameters.Clear();

            export.setFinancialExportID(financialexportid);

            ResetCache();

            return financialexportid;
        }

        /// <summary>
        /// Update the status of a financial export. This is used for ESR exports only
        /// </summary>
        /// <param name="exportHistoryID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateExportHistoryStatus(int exportHistoryID, FinancialExportStatus status)
        {
            cEventlog.LogEntry("UpdateExportHistoryStatus(" + exportHistoryID + "," + status + ") called");
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@exporthistoryid", exportHistoryID);
            expdata.sqlexecute.Parameters.AddWithValue("@exportStatus", (byte)status);

            if (currentUser != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.ExecuteProc("changeExportHistoryStatus");
            expdata.sqlexecute.Parameters.Clear();
            return 0;
        }

        /// <summary>
        /// Update the financial export information
        /// </summary>
        /// <param name="export"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int updateFinancialExport(cFinancialExport export, int userid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", export.financialexportid);
            expdata.sqlexecute.Parameters.AddWithValue("@applicationtype", (byte)export.application);

            expdata.sqlexecute.Parameters.AddWithValue("@reportid", export.reportid);

            expdata.sqlexecute.Parameters.AddWithValue("@automated", Convert.ToBoolean(export.automated));
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", export.createdby);
            expdata.sqlexecute.Parameters.AddWithValue("@preventNegativePayment", export.PreventNegativeAmountPayable);
            expdata.sqlexecute.Parameters.AddWithValue("@expeditePaymentReport", export.ExpeditePaymentReport);

            if (export.application == FinancialApplication.CustomReport)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@exporttype", export.exporttype);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@exporttype", DBNull.Value);
            }

            if (export.NHSTrustID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", export.NHSTrustID);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.ExecuteProc("updateFinancialExport");


            expdata.sqlexecute.Parameters.Clear();

            ResetCache();

            return 0;
        }
        public DataTable getGrid()
        {
            DataTable tbl = new DataTable();
            System.Data.DataTable global = new System.Data.DataTable();
            object[] values;
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            cReport rpt;

            cReportFolders folders = new cReportFolders(nAccountid);
            cReportFolder folder;
            tbl.Columns.Add("financialexportid", typeof(System.Int32));
            tbl.Columns.Add("applicationtype", typeof(System.Byte));
            tbl.Columns.Add("employeeid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("reportid", typeof(System.Guid));
            tbl.Columns.Add("reportname", System.Type.GetType("System.String"));
            tbl.Columns.Add("category", typeof(System.String));
            tbl.Columns.Add("description", System.Type.GetType("System.String"));
            tbl.Columns.Add("reporttype", System.Type.GetType("System.String"));
            tbl.Columns.Add("lastexportdate", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("curexportnum", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("exporttype", typeof(System.Byte));
            tbl.Columns.Add("automated", typeof(System.Boolean));

            foreach (cFinancialExport export in list.Values)
            {
                rpt = clsreports.getReportById(accountid, export.reportid);
                values = new object[12];
                values[0] = export.financialexportid;
                values[1] = export.application;
                values[2] = export.createdby;

                values[3] = export.reportid;

                if (rpt != null)
                {
                    values[4] = rpt.reportname;
                    if (rpt.FolderID.HasValue && rpt.FolderID != Guid.Empty)
                    {
                        folder = folders.getFolderById((Guid)rpt.FolderID);
                        if (folder != null)
                        {
                            values[5] = folder.folder;
                        }
                    }
                    values[6] = rpt.description;
                    values[7] = rpt.getReportType();
                }
                values[8] = export.lastexportdate;
                values[9] = export.curexportnum;
                values[10] = export.exporttype;




                values[11] = export.automated;
                tbl.Rows.Add(values);

            }
            return tbl;
        }

        public cFinancialExport getESRExport(int NHSTrustID)
        {
            foreach (cFinancialExport export in list.Values)
            {
                if (export.application == FinancialApplication.ESR && export.NHSTrustID == NHSTrustID)
                {
                    return export;
                }
            }
            return null;
        }

        /// <summary>
        /// Delete financial export from the database
        /// </summary>
        /// <param name="financialexportid"></param>
        public void deleteFinancialExport(int financialexportid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "deleteFinancialExport";
            expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", financialexportid);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            expdata.ExecuteProc(strsql);
            expdata.sqlexecute.Parameters.Clear();
            ResetCache();
        }



        public System.Data.DataSet getHistoryGrid(int financialexportid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet rcdsthistory;

            strsql = "select exporthistoryid, exportnum, dateexported, employees.surname + ', ' + employees.title + ' ' + employees.firstname as employee from exporthistory inner join employees on employees.employeeid = exporthistory.employeeid where financialexportid = @financialexportid order by dateexported desc";
            expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", financialexportid);
            rcdsthistory = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return rcdsthistory;
        }
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion

        /// <summary>
        /// Get the history of the specified financial export
        /// </summary>
        /// <param name="exportHistoryID"></param>
        /// <param name="financialExportID"></param>
        /// <returns></returns>
        public cReportRequest getExportHistoryData(int exportHistoryID, int financialExportID)
        {
            cReportRequest exportrequest = null;
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            cFinancialExport export = getExportById(financialExportID);

            if (export != null)
            {
                cExportOptions clsOptions = null;
                switch (export.application)
                {
                    case FinancialApplication.ESR:
                        clsOptions = new cExportOptions(0, export.reportid, false, false, false, new SortedList<Guid, int>(), null, Guid.Empty, export.application, ",", false, true, export.PreventNegativeAmountPayable);

                        clsOptions.isfinancialexport = true;
                        clsOptions.financialexport = export;
                        clsOptions.exporthistoryid = exportHistoryID;

                        cESRTrusts clsTrusts = new cESRTrusts(accountid);
                        string filename = clsTrusts.CreateESRInboundFilename(clsOptions.financialexport.NHSTrustID);
                        clsOptions.ExportFileName = filename;

                        break;
                }

                cReport rpt = clsreports.getReportById(accountid, export.reportid);
                rpt.exportoptions = clsOptions;

                if (rpt.SubAccountID != null)
                {
                    exportrequest = new cReportRequest(this.accountid, rpt.SubAccountID.Value, 0, rpt, ExportType.CSV, clsOptions, false, 0, AccessRoleLevel.AllData);
                    clsreports.createReport(exportrequest);
                }
            }

            return exportrequest;
        }

        /// <summary>
        /// Get the information for the employees who have no assignment number associated to the expense item they have
        /// claimed. This will only happen for ESR financial exports.
        /// </summary>
        /// <returns></returns>
        public string CheckFinancialExportESRAssignments()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            StringBuilder sb = new StringBuilder();
            bool areValues = false;

            sb.Append("########The following employee expense items do not have an ESR Assignment number associated########<br /><br />");

            using (SqlDataReader reader = expdata.GetStoredProcReader("CheckFinancialExportESRAssignments"))
            {
                while (reader.Read())
                {
                    areValues = true;
                    sb.Append("Employee ");
                    sb.Append(reader.GetString(0) + " ");
                    sb.Append(reader.GetString(1) + " ");
                    sb.Append("with Username ");
                    sb.Append(reader.GetString(2) + " ");
                    sb.Append("on claim ");
                    sb.Append(reader.GetString(3));
                    sb.Append(" on expense item with the date ");
                    sb.Append(reader.GetDateTime(4).ToShortDateString());
                    sb.Append(", total ");
                    sb.Append(reader.GetDecimal(5).ToString());
                    sb.Append(" and type ");
                    sb.Append(reader.GetString(6));
                    sb.Append("<br />");
                }

                reader.Close();
            }

            if (areValues)
            {
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        public static List<string> GetAccountsWithFinancialExports()
        {
            var result = new List<string>();

            var accounts = cAccounts.CachedAccounts.Where(x => !x.Value.archived).ToList();
            foreach (KeyValuePair<int, cAccount> account in accounts)
            {
                var financialExports = new cFinancialExports(account.Value.accountid);
                if (financialExports.list.Count > 0)
                {
                    result.Add(
                        string.Format(
                            "<div id='fe{0}' class='informationBlock'><span class='title'>{1}</span><img id='imgShowReportsEvents' src='/static/icons/16/plain/refresh.png' onclick='SEL.SystemHealth.TestFinancialExports({0})' class='btn healthpage-eventloader' alt='Test all the financial export reports'><img class='showHideButton' alt='Show or hide information' src='/static/icons/16/plain/navigate_open.png'><div id='fedetail{2}' class='healthpage-events'></div></div>",
                            account.Value.accountid,
                            account.Value.companyname,
                        account.Value.accountid));    
                }
            }

            return result;
        }

        public static List<string> TestFinancialExports(int accountId)
        {
            var result = new List<string>();
            
            var accounts = cAccounts.CachedAccounts.Where(x => x.Value.accountid == accountId).ToList();
            var reportsPath = ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem";
            var reports = (IReports)Activator.GetObject(typeof(IReports), reportsPath);
            var currentUser = cMisc.GetCurrentUser();
            
            foreach (KeyValuePair<int, cAccount> account in accounts)
            {
                var employees = new cEmployees(account.Value.accountid);
                var adminUser = employees.GetEmployeeById(employees.getEmployeeidByUsername(account.Value.accountid,"Admin13"));
                var subAccount = adminUser.DefaultSubAccount;
                result.Add(string.Format("Checking account '{0}' for any financial exports", account.Value.companyname));
                var financialExports = new cFinancialExports(account.Value.accountid);
                foreach (cFinancialExport financialExport in financialExports.list.Values)
                {
                    string currentReportName = string.Empty;
                    try
                    {
                        cReport report = reports.getReportById(account.Value.accountid, financialExport.reportid);
                        currentReportName = report.reportname;
                        var reportRequest = new cReportRequest(
                            account.Value.accountid,
                            subAccount,
                            1,
                            report,
                            ExportType.Viewer,
                            null,
                            false,
                            currentUser.EmployeeID,
                            AccessRoleLevel.AllData);
                        using (DataSet dataSet = reports.createSynchronousReport(reportRequest))
                        {
                        }

                        result.Add(string.Format("Creating report from financial export {0} {1} - OK", financialExport.financialexportid, currentReportName));
                    }
                    catch (Exception e)
                    {
                        result.Add(
                            string.Format("Creating report from financial export {0} {1} - Error found: {2}", financialExport.financialexportid, currentReportName, e.Message));
                    }
                }    
            }

            return result;
        }
    }
}
