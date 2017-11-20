using System;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using Spend_Management.shared.webServices;

namespace Spend_Management
{
    using System.Data;

    using SpendManagementLibrary.Definitions;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Logic_Classes.Fields;

    /// <summary>
    /// Summary description for cReports.
    /// </summary>
    public class cReports
    {
        DBConnection expdata;
        string _strsql = "";
        readonly int _accountid;
        readonly int _subaccountid;

        #region Properties

        public int AccountID
        {
            get { return _accountid; }
        }

        public int SubAccountID
        {
            get { return _subaccountid; }
        }

        #endregion

        public cReports()
        {
        }
        public cReports(int nAccountid, int subAccountId)
        {
            _accountid = nAccountid;
            _subaccountid = subAccountId;
        }

        public cReport getReportById(Guid id)
        {
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            return clsreports.getReportById(AccountID, id);
        }

        public int getListValueFromText(cField field, string value)
        {
            expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            cField srcField = field;
            cTable srcTable = field.GetParentTable();

            if (field.FieldType == "R")
            {
                srcField = field.GetRelatedTable().GetKeyField();
                srcTable = field.GetRelatedTable();
            }
            _strsql = "select [" + srcTable.TableName + "].[" + srcTable.GetPrimaryKey().FieldName + "] from [" + srcTable.TableName + "] where [" + srcTable.TableName + "].[" + srcField.FieldName + "] = @value";
            expdata.sqlexecute.Parameters.AddWithValue("@value", value);
            int val = expdata.getcount(_strsql);
            expdata.sqlexecute.Parameters.Clear();
            return val;
        }

        public string getReportListValues(cReportCriterion criteria)
        {
            expdata = new DBConnection(cAccounts.getConnectionString(_accountid));
            System.Data.SqlClient.SqlDataReader reader;
            string values = "";
            cField field = criteria.field;
            cTable table = criteria.field.GetParentTable();

            if (field.FieldType == "R")
            {
                table = field.GetRelatedTable();
                field = field.GetRelatedTable().GetKeyField();
            }

            _strsql = "select [" + table.TableName + "].[" + field.FieldName + "] from [" + field.GetParentTable().TableName + "]";

            if (criteria.value1[0] != null && criteria.value1[0] != "")
            {
                _strsql += " where [" + table.TableName + "].[" + table.GetPrimaryKey().FieldName + "] in (" + criteria.value1[0] + ")";
            }

            using (reader = expdata.GetReader(_strsql))
            {
                while (reader.Read())
                {
                    values += reader.GetString(0) + ", ";
                }
                reader.Close();
            }
            if (values.Length != 0)
            {
                values = values.Remove(values.Length - 2, 2);
            }

            return values;
        }

        /// <summary>
        /// Returns whether the name for a report has already been used
        /// </summary>
        /// <param name="name">The name of the report to be checked</param>
        /// <param name="subAccountID">The sub account ID to check</param>
        /// <returns></returns>
        public bool nameUsed(string name, int? subAccountID)
        {
            DBConnection dbcon = new DBConnection(cAccounts.getConnectionString(AccountID));
            string sql = "select count(*) from reports where reportname = @reportname and (subaccountid is null or (subaccountid is not null and subaccountid = @subaccountID))";
            dbcon.sqlexecute.Parameters.Add("@reportname", SqlDbType.NVarChar, 150);
            dbcon.sqlexecute.Parameters["@reportname"].Value = name;
            if (subAccountID.HasValue && subAccountID > 0)
            {
                dbcon.sqlexecute.Parameters.AddWithValue("@subaccountID", subAccountID);
            }
            else
            {
                dbcon.sqlexecute.Parameters.AddWithValue("@subaccountID", DBNull.Value);
            }
            int count = dbcon.getcount(sql);
            dbcon.sqlexecute.Parameters.Clear();

            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public Guid addReport(cReport rpt)
        {
            Guid reportid;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                if (nameUsed(rpt.reportname, rpt.SubAccountID.Value) == true)
                {
                    return Guid.Empty;
                }

                reportid = Guid.NewGuid();
                string strsql =
                    "insert into reports (reportid, employeeid, reportname, description, basetable, folderid, readonly, forclaimants, limit, subAccountID, useJoinVia, showChart, module) values (@reportid, @employeeid,@reportname,@description,@basetable, @folderid, @readonly, @forclaimants, @limit, @subaccountid, @useJoinVia, @showChart, @module);";

                connection.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", rpt.employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@reportname", rpt.reportname);
                if (rpt.description.Length > 2000)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@description", rpt.description.Substring(0, 1999));
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@description", rpt.description);
                }
                connection.sqlexecute.Parameters.AddWithValue("@basetable", rpt.basetable.TableID);
                connection.sqlexecute.Parameters.AddWithValue("@limit", rpt.Limit);
                if (rpt.FolderID != Guid.Empty)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@folderid", rpt.FolderID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@folderid", DBNull.Value);
                }
                connection.sqlexecute.Parameters.AddWithValue("@readonly", Convert.ToByte(rpt.readonlyrpt));
                connection.sqlexecute.Parameters.AddWithValue("@forclaimants", Convert.ToByte(rpt.claimantreport));

                if (rpt.SubAccountID == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@subaccountid", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@subaccountid", (int)rpt.SubAccountID);
                }

                connection.sqlexecute.Parameters.AddWithValue("@useJoinVia", rpt.UseJoinVia);
                connection.sqlexecute.Parameters.AddWithValue("@showChart", rpt.ShowChart);
                connection.sqlexecute.Parameters.AddWithValue("@module", rpt.reportArea);

                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();

                rpt.setReportID(reportid);
                rpt.exportoptions = new cExportOptions(rpt.employeeid.Value, reportid, false, false, false, new SortedList<Guid, int>(), null, Guid.Empty, FinancialApplication.CustomReport, ",", false, true, false);
                for (int i = 0; i < rpt.columns.Count; i++)
                {
                    addColumn(reportid, (cReportColumn)rpt.columns[i]);
                }

                RenumberGroups(rpt.criteria);
                for (int i = 0; i < rpt.criteria.Count; i++)
                {
                    this.AddCriteria(reportid, (cReportCriterion)rpt.criteria[i]);
                }

                strsql = "update reports set modifiedon = getDate() where reportid = @reportid";
                connection.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();
            }

            return reportid;

        }

        private bool alreadyExists(Guid reportid, string reportname, int action, int subAccountID)
        {
            var dbcon = new DBConnection(cAccounts.getConnectionString(AccountID));
            if (subAccountID == 0)
            {
                subAccountID = cMisc.GetCurrentUser().CurrentSubAccountId;
            }

            string sql = "select count(*) from reports where reportname = @reportname and reportid <> @reportid and subaccountid = @subaccountID";

            dbcon.sqlexecute.Parameters.Add("@reportname", SqlDbType.NVarChar, 150);
            dbcon.sqlexecute.Parameters["@reportname"].Value = reportname;
            dbcon.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
            dbcon.sqlexecute.Parameters.AddWithValue("@subaccountID", subAccountID);
            int count = dbcon.getcount(sql);
            dbcon.sqlexecute.Parameters.Clear();
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// Update an existing report
        /// </summary>
        /// <param name="report">An instance of <see cref="cReport"/>to update </param>
        /// <returns></returns>
        public int updateReport(cReport report)
        {
            string strsql;
            var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID));
            cReport oldreport = getReportById(report.reportid);
            if (alreadyExists(report.reportid, report.reportname, 2, oldreport != null && oldreport.SubAccountID.HasValue ? oldreport.SubAccountID.Value : 0))
            {
                return 1;
            }

            try
            {
                using (System.Transactions.TransactionScope transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0)))
                {
                    strsql =
                        "update reports set reportname = @reportname, description = @description, basetable = @basetable, folderid = @folderid, readonly = @readonly, forclaimants = @forclaimants, modifiedon = getDate(), limit = @limit, subaccountID = @subaccountid, showChart = @showChart where reportid = @reportid";

                    expdata.sqlexecute.Parameters.AddWithValue("@reportname", report.reportname);
                    expdata.sqlexecute.Parameters.AddWithValue("@description", report.description);
                    expdata.sqlexecute.Parameters.AddWithValue("@basetable", report.basetable.TableID);
                    expdata.sqlexecute.Parameters.AddWithValue("@readonly", Convert.ToByte(report.readonlyrpt));
                    if (report.FolderID != Guid.Empty)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@folderid", report.FolderID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@folderid", DBNull.Value);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@forclaimants", Convert.ToByte(report.claimantreport));
                    expdata.sqlexecute.Parameters.AddWithValue("@reportid", report.reportid);
                    expdata.sqlexecute.Parameters.AddWithValue("@limit", report.Limit);
                    if (report.SubAccountID == null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", (int) report.SubAccountID);
                    }

                    expdata.sqlexecute.Parameters.AddWithValue("@showChart", report.ShowChart);

                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    this.deleteCriteria(report.reportid);

                    cReportColumn reportcol;
                    for (int i = 0; i < oldreport.columns.Count; i++)
                    {
                        reportcol = (cReportColumn) oldreport.columns[i];
                        if (report.getReportColumnById(reportcol.reportcolumnid) == null)
                        {
                            deleteReportColumn(reportcol);
                        }
                    }

                    foreach (var column in report.columns)
                    {
                        this.addColumn(report.reportid, (cReportColumn) column);
                    }

                    foreach (var criteria in report.criteria)
                    {
                        this.AddCriteria(report.reportid, (cReportCriterion) criteria);
                    }

                    strsql = "update reports set modifiedon = getDate() where reportid = @reportid";
                    expdata.sqlexecute.Parameters.AddWithValue("@reportid", report.reportid);
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {

                LogError(ex, "updateReport");
                return 2;
            }

            return 0;
        }

        private void deleteCriteria(Guid reportid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            string strsql = "delete from reportcriteria where reportid = @reportid";
            expdata.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();


        }
        private void deleteReportColumn(cReportColumn reportcol)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            string strsql;
            strsql = "delete from reportcolumns where reportcolumnid = @reportcolumnid";
            expdata.sqlexecute.Parameters.AddWithValue("@reportcolumnid", reportcol.reportcolumnid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public void deleteReport(Guid reportid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            expdata.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
            strsql = "delete from ReportCharts where reportid = @reportid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from reportcolumns where reportid = @reportid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from reportcriteria where reportid = @reportid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from reports where reportid = @reportid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from scheduled_reports where reportid = @reportid";
            expdata.ExecuteSQL(strsql);

            expdata.sqlexecute.Parameters.Clear();


        }

        public ArrayList getValueList()
        {
            DBConnection dbcon = new DBConnection(cAccounts.getConnectionString(AccountID));
            ArrayList list = new ArrayList();
            using (SqlDataReader reader = dbcon.GetReader("select reportid, reportname from reportsview"))
            {
                while (reader.Read())
                {
                    list.Add(new object[] { reader.GetGuid(0), reader.GetString(1) });
                }
                reader.Close();
            }
            return list;

        }
        public SortedList<string, string> getFinancialExportableList()
        {
            var financialExportableReports = new SortedList<string, string>();

            using (var dbcon = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                using (IDataReader reader = dbcon.GetReader("select reportid, reportname from dbo.reportsview where globalReport <> 1"))
                {
                    while (reader.Read())
                    {
                        if (financialExportableReports.ContainsKey(reader.GetString(1)) == false)
                        {
                            financialExportableReports.Add(reader.GetString(1), reader.GetGuid(0).ToString());
                        }
                    }
                    reader.Close();
                }
            }

            return financialExportableReports;
        }

        public List<ListItem> getFooterReports(Guid basetable, Guid reportid)
        {
            DBConnection dbcon = new DBConnection(cAccounts.getConnectionString(AccountID));
            List<ListItem> list = new List<ListItem>();
            dbcon.sqlexecute.Parameters.AddWithValue("@basetable", basetable);
            dbcon.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
            using (SqlDataReader reader = dbcon.GetReader("select reportid, reportname from reportsview where basetable = @basetable and reportid <> @reportid order by reportname"))
            {
                while (reader.Read())
                {
                    list.Add(new ListItem(reader.GetString(1), reader.GetGuid(0).ToString()));
                }
                reader.Close();

            }
            dbcon.sqlexecute.Parameters.Clear();

            return list;
        }
        
        private static void LogError(Exception ex, string methodName, string methodMessage = "")
        {
            string message = ex.Message;
            string methodInfo = methodMessage == string.Empty ? "Error" : methodMessage;
            if (ex.InnerException != null)
            {
                message = string.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException.Message);
            }

            cEventlog.LogEntry(string.Format("ReportEngine : cReportsSvc : {0} : {1} : {2}{3}{4}{5}", methodName, methodInfo, Environment.NewLine, message, Environment.NewLine, ex.StackTrace));
        }

        /// <summary>
        /// Save a report as a copy to a new location/report.
        /// </summary>
        /// <param name="reportId">The ID of the report to copy</param>
        /// <param name="employeeId">The current EmployeeID</param>
        /// <param name="reportName">The name of the copy report</param>
        /// <param name="folderId">The Folder GUID to store the report in</param>
        /// <param name="currentUser">An instance of <see cref="CurrentUser"/></param>
        /// <returns>The <see cref="Guid"/>ID of the new report or Guid.Empty</returns>
        public Guid SaveReportAs(Guid reportId, int? employeeId, string reportName, Guid? folderId, CurrentUser currentUser)
        {
            cReport originalReport = this.getReportById(reportId);

            if (originalReport != null)
            {
                
                var newReport = (cReport)originalReport.Clone();
                if (!originalReport.SubAccountID.HasValue)
                {
                    newReport.SubAccountID = currentUser.CurrentSubAccountId;
                }

                newReport.FolderID = folderId;

                newReport.reportname = reportName;
                newReport.employeeid = employeeId.HasValue ? employeeId : null;

                for (int i = 0; i < newReport.columns.Count; i++)
                {
                    ((cReportColumn)newReport.columns[i]).reportcolumnid = Guid.Empty;
                }

                for (int i = 0; i < newReport.criteria.Count; i++)
                {
                    ((cReportCriterion)newReport.criteria[i]).criteriaid = Guid.Empty;
                }

                return this.addReport(newReport);
            }
            else
            {
                return Guid.Empty;
            }
        }

        private static void RenumberGroups(IList criteria)
        {
            int count = 0;
            int groupnum = -1;
            for (int i = 0; i < criteria.Count; i++)
            {
                var criterion = (cReportCriterion)criteria[i];
                if (groupnum == -1)
                {
                    groupnum = criterion.groupnumber;
                }

                if (criterion.groupnumber != groupnum)
                {
                    count++;
                    groupnum = criterion.groupnumber;
                }

                criterion.groupnumber = count;
            }
        }

        /// <summary>
        /// Adds a new column to the given report
        /// </summary>
        /// <param name="reportid">The ID of the report to add the column to</param>
        /// <param name="column">The column to add</param>
        private void addColumn(Guid reportid, cReportColumn column)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            Guid reportColumnID = column.reportcolumnid;
            string strsql = "";

            cStandardColumn standard;
            cStaticColumn staticcol;
            cCalculatedColumn calculatedcol;
            switch (column.columntype)
            {
                case ReportColumnType.Standard:
                    standard = (cStandardColumn)column;
                    if (column.reportcolumnid != Guid.Empty)
                    {
                        strsql =
                            "update reportcolumns set fieldid = @fieldid, groupby = @groupby, sort = @sort, [order] = @order, funcsum = @funcsum, funcmax = @funcmax, funcmin = @funcmin, funcavg = @funcavg, funccount = @funccount, hidden = @hidden, joinViaID = @joinViaID, format = @format, [system] = @system  where reportcolumnid = @reportcolumnid";
                        expdata.sqlexecute.Parameters.AddWithValue("@reportcolumnid", column.reportcolumnid);
                    }
                    else
                    {

                        strsql =
                            "insert into reportcolumns (reportcolumnid, reportid, columntype, fieldid, groupby, sort, [order], funcsum, funcmax, funcmin, funcavg, funccount, hidden, joinViaID, format, [system]) values (@reportColumnID, @reportid,@columntype,@fieldid,@groupby,@sort,@order,@funcsum,@funcmax,@funcmin,@funcavg,@funccount,@hidden, @joinViaID, @format, @system)";
                        expdata.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                        expdata.sqlexecute.Parameters.AddWithValue("@columntype", 1);
                        reportColumnID = Guid.NewGuid();
                        expdata.sqlexecute.Parameters.AddWithValue("@reportColumnID", reportColumnID);
                    }

                    expdata.sqlexecute.Parameters.AddWithValue("@fieldid", standard.field.FieldID);

                    expdata.sqlexecute.Parameters.AddWithValue("@groupby", Convert.ToByte(standard.groupby));
                    expdata.sqlexecute.Parameters.AddWithValue("@sort", standard.sort);
                    expdata.sqlexecute.Parameters.AddWithValue("@order", standard.order);
                    expdata.sqlexecute.Parameters.AddWithValue("@funcsum", Convert.ToByte(standard.funcsum));
                    expdata.sqlexecute.Parameters.AddWithValue("@funcmax", Convert.ToByte(standard.funcmax));
                    expdata.sqlexecute.Parameters.AddWithValue("@funcmin", Convert.ToByte(standard.funcmin));
                    expdata.sqlexecute.Parameters.AddWithValue("@funcavg", Convert.ToByte(standard.funcavg));
                    expdata.sqlexecute.Parameters.AddWithValue("@funccount", Convert.ToByte(standard.funccount));
                    expdata.sqlexecute.Parameters.AddWithValue("@hidden", Convert.ToByte(standard.hidden));
                    expdata.sqlexecute.Parameters.AddWithValue("@format", standard.DisplayName);
                    expdata.sqlexecute.Parameters.AddWithValue("@system", standard.system);
                    if (column.JoinVia != null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", column.JoinVia.JoinViaID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
                    }

                    break;
                case ReportColumnType.Static:
                    staticcol = (cStaticColumn)column;
                    if (column.reportcolumnid != Guid.Empty)
                    {
                        strsql = "update reportcolumns set [order] = @order, literalname = @literalname, literalvalue = @literalvalue, runtime = @runtime " + "where reportcolumnid = @reportcolumnid";
                        expdata.sqlexecute.Parameters.AddWithValue("@reportcolumnid", column.reportcolumnid);
                    }
                    else
                    {
                        strsql = "insert into reportcolumns (reportcolumnid, reportid, columntype, [order], literalname, literalvalue, runtime) "
                                 + "values (@reportColumnID, @reportid,@columntype,@order,@literalname, @literalvalue, @runtime);";
                        reportColumnID = Guid.NewGuid();
                        expdata.sqlexecute.Parameters.AddWithValue("@reportColumnID", reportColumnID);
                        expdata.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                        expdata.sqlexecute.Parameters.AddWithValue("@columntype", 2);
                    }

                    expdata.sqlexecute.Parameters.AddWithValue("@literalname", staticcol.literalname);
                    if (staticcol.literalvalue == null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@literalvalue", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@literalvalue", staticcol.literalvalue);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@order", staticcol.order);
                    expdata.sqlexecute.Parameters.AddWithValue("@runtime", staticcol.runtime);
                    break;
                case ReportColumnType.Calculated:
                    calculatedcol = (cCalculatedColumn)column;
                    if (column.reportcolumnid != Guid.Empty)
                    {
                        strsql = "update reportcolumns set [order] = @order, literalname = @literalname, literalvalue = @literalvalue " + "where reportcolumnid = @reportcolumnid";
                        expdata.sqlexecute.Parameters.AddWithValue("@reportcolumnid", column.reportcolumnid);
                    }
                    else
                    {
                        strsql =
                            "insert into reportcolumns (reportcolumnid, reportid, columntype, [order], literalname, literalvalue) values (@reportColumnID, @reportid,@columntype,@order,@literalname, @literalvalue);";
                        expdata.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                        expdata.sqlexecute.Parameters.AddWithValue("@columntype", 3);
                        reportColumnID = Guid.NewGuid();
                        expdata.sqlexecute.Parameters.AddWithValue("@reportColumnID", reportColumnID);
                    }

                    expdata.sqlexecute.Parameters.AddWithValue("@literalname", calculatedcol.columnname);
                    expdata.sqlexecute.Parameters.AddWithValue("@literalvalue", calculatedcol.formattedFormula);
                    expdata.sqlexecute.Parameters.AddWithValue("@order", calculatedcol.order);
                    break;
            }

            if (column.reportcolumnid == Guid.Empty)
            {

            }
            expdata.ExecuteSQL(strsql);

            column.reportcolumnid = reportColumnID;
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The add a single criteria to the database.
        /// </summary>
        /// <param name="reportid">
        /// The report ID.
        /// </param>
        /// <param name="criteria">
        /// The criteria to add.
        /// </param>
        private void AddCriteria(Guid reportid, cReportCriterion criteria)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                var criteriaid = Guid.NewGuid();
                const string Strsql = "insert into reportcriteria (criteriaid, reportid, fieldid, condition, value1, value2, [order], runtime, andor, groupnumber, joinViaID) "
                                      + "values (@criteriaid, @reportid,@fieldid,@condition,@value1,@value2,@order, @runtime, @andor, @groupnumber, @joinViaID);";

                connection.sqlexecute.Parameters.AddWithValue("@criteriaid", criteriaid);
                connection.sqlexecute.Parameters.AddWithValue("@reportid", reportid);
                connection.sqlexecute.Parameters.AddWithValue("@fieldid", criteria.field == null ? Guid.Empty : criteria.field.FieldID);
                connection.sqlexecute.Parameters.AddWithValue("@condition", (byte)criteria.condition);
                connection.sqlexecute.Parameters.AddWithValue("@order", criteria.order);
                connection.sqlexecute.Parameters.AddWithValue("@runtime", criteria.runtime);
                connection.sqlexecute.Parameters.AddWithValue("@andor", (byte)criteria.joiner);
                if (criteria.field != null && criteria.field.GenList == false)
                {
                    if (criteria.value1 == null)
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@value1", DBNull.Value);
                    }
                    else
                    {
                        if (criteria.value1[0] == null)
                        {
                            connection.sqlexecute.Parameters.AddWithValue("@value1", DBNull.Value);
                        }
                        else
                        {
                            connection.sqlexecute.Parameters.AddWithValue("@value1", criteria.value1[0]);
                        }
                    }
                    if (criteria.condition == ConditionType.Between && !criteria.runtime)
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@value2", criteria.value2[0]);
                    }
                    else
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@value2", DBNull.Value);
                    }
                }
                else
                {
                    if (criteria.value1 == null)
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@value1", DBNull.Value);
                    }
                    else
                    {
                        if (criteria.value1[0] == null)
                        {
                            connection.sqlexecute.Parameters.AddWithValue("@value1", DBNull.Value);
                        }
                        else
                        {
                            connection.sqlexecute.Parameters.AddWithValue("@value1", criteria.value1[0]);
                        }
                    }
                    connection.sqlexecute.Parameters.AddWithValue("@value2", DBNull.Value);
                }
                if (criteria.groupnumber == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@groupnumber", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@groupnumber", criteria.groupnumber);
                }

                if (criteria.JoinVia == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@joinViaID", criteria.JoinVia.JoinViaID);
                }

                connection.ExecuteSQL(Strsql);
                criteria.criteriaid = criteriaid;
                connection.sqlexecute.Parameters.Clear();
            }
        }

        public void getAvailableListItems(cField field, int reportid, ref Infragistics.WebUI.UltraWebNavigator.UltraWebTree list)
        {
            expdata = new DBConnection(cAccounts.getConnectionString(_accountid));
            System.Data.SqlClient.SqlDataReader reader;
            cField primarykey;
            cField srcField;

            if (field.FieldSource != cField.FieldSourceType.Metabase && field.FieldType == "N" && field.GetRelatedTable() != null && field.IsForeignKey)
            {
                // catch custom entity or UDF n:1 relationship fields
                Dictionary<object, object> values = getRelationshipValues(cMisc.GetCurrentUser(), field.FieldID);

                foreach (KeyValuePair<object, object> kvp in values)
                {
                    list.Nodes.Add(kvp.Value.ToString(), kvp.Key);
                }
            }
            else
            {
                if (field.FieldType == "R")
                {
                    primarykey = field.GetRelatedTable().GetPrimaryKey();
                    srcField = field.GetRelatedTable().GetKeyField();
                }
                else
                {
                    primarykey = field.GetParentTable().GetPrimaryKey(); // clsfields.getFieldById(field.table.primarykey);
                    srcField = field;
                }

                _strsql = "select " + primarykey.GetParentTable().TableName + "." + primarykey.FieldName + " as table_value, " + srcField.GetParentTable().TableName + "." + srcField.FieldName + " as table_description from " + srcField.GetParentTable().TableName;

                if (primarykey.GetParentTable().GetSubAccountIDField() != null && primarykey.GetParentTable().SubAccountIDFieldID != Guid.Empty)
                {
                    // need to filter by subaccount
                    CurrentUser user = cMisc.GetCurrentUser();
                    cField subaccField = primarykey.GetParentTable().GetSubAccountIDField();
                    _strsql += " where " + subaccField.FieldName + " = @subAccId";
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccId", user.CurrentSubAccountId);
                }

                _strsql += " order by " + srcField.FieldName;

                using (reader = expdata.GetReader(_strsql))
                {
                    while (reader.Read())
                    {
                        list.Nodes.Add(reader.GetString(1), reader.GetInt32(0));
                    }
                    reader.Close();
                }
            }
        }

         /// <summary>
        /// Check to see if the report has any static entries that are "enter at run time"  
        /// </summary>
        /// <param name="reportId">The report Id to check</param>
        /// <returns>True if any of the current report is used in a schedule</returns>
        public bool ReportIsScheduled(Guid reportId)
        {
        var result = false;
        var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountID));
            connection.sqlexecute.Parameters.AddWithValue("@reportid", reportId);
            using (var reader = connection.GetReader("select reportid from scheduled_reports WHERE reportid = @reportid"))
            {
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        result = true;
                    }
                }
            }

        return result;
        }


        public static Dictionary<object, object> getRelationshipValues(ICurrentUser currentUser, Guid fieldId)
        {
            Dictionary<object, object> retList = new Dictionary<object, object>();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));

            cFields clsFields = new cFields(currentUser.AccountID);
            cField field = clsFields.GetFieldByID(fieldId);
            Guid displayFieldId = Guid.Empty;

            switch (field.FieldSource)
            {
                case cField.FieldSourceType.CustomEntity:
                    cCustomEntities entities = new cCustomEntities(currentUser);
                    cCustomEntity entity = entities.getEntityByTableId(field.TableID);
                    cAttribute att = null;

                    if (entity.IsSystemView)
                    {
                        entity = entities.getEntityById(entity.SystemView_DerivedEntityId.Value);
                        att = entity.getAttributeByName(field.FieldName);
                    }
                    else
                    {
                        att = entities.getAttributeByFieldId(fieldId);
                    }

                    if (att.GetType() == typeof(cManyToOneRelationship))
                    {
                        displayFieldId = ((cManyToOneRelationship)att).AutoCompleteDisplayField;
                    }
                    break;
                case cField.FieldSourceType.Userdefined:
                    cUserdefinedFields ufields = new cUserdefinedFields(currentUser.AccountID);
                    cUserDefinedField uf = ufields.GetUserdefinedFieldByFieldID(fieldId);
                    if (uf.fieldtype == FieldType.Relationship)
                    {
                        displayFieldId = ((cManyToOneRelationship)uf.attribute).AutoCompleteDisplayField;
                    }
                    break;
                default:
                    return retList;
                    break;
            }

            // if display field is not set, use related table's KeyField (i.e. for employees, this is username)
            if (displayFieldId == Guid.Empty || displayFieldId == null)
            {
                cTables clsTables = new cTables(currentUser.AccountID);
                cTable relatedTable = clsTables.GetTableByID(field.RelatedTableID);
                displayFieldId = relatedTable.GetKeyField().FieldID;
            }

            Caching cache = new Caching();
            if (cache.Cache.Contains(currentUser.AccountID.ToString() + "_" + fieldId.ToString()))
            {
                retList = (Dictionary<object, object>)cache.Cache[currentUser.AccountID.ToString() + "_" + fieldId.ToString()];
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@relatedTable", field.RelatedTableID);
                db.sqlexecute.Parameters.AddWithValue("@displayField", displayFieldId);
                db.sqlexecute.Parameters.AddWithValue("@subAccountId", currentUser.CurrentSubAccountId);

                using (SqlDataReader reader = db.GetStoredProcReader("getRelationshipValues"))
                {
                    while (reader.Read())
                    {
                        object id = reader.GetValue(0);
                        object value = reader.GetValue(1);

                        retList.Add(id, value);
                    }
                    reader.Close();
                }

                cache.Add(currentUser.AccountID.ToString() + "_" + fieldId.ToString(), retList, string.Empty, Caching.CacheTimeSpans.UltraShort, Caching.CacheDatabaseType.Customer, currentUser.AccountID);
            }

            return retList;
        }

        public static ValueList getRelationshipValuesAsValueList(ICurrentUser currentUser, Guid fieldId)
        {
            Dictionary<object, object> values = getRelationshipValues(currentUser, fieldId);

            ValueList vlist = new ValueList();
            vlist.ValueListItems.Add(DBNull.Value, "");
            vlist.ValueListItems.Add("0", "");

            foreach (KeyValuePair<object, object> kvp in values)
            {
                vlist.ValueListItems.Add(new ValueListItem(Convert.ToString(kvp.Value), kvp.Key));
            }

            return vlist;
        }

        public static string getRelationshipValueText(ICurrentUser currentUser, Guid fieldId, object valueId)
        {
            Dictionary<object, object> values = getRelationshipValues(currentUser, fieldId);

            return values.ContainsKey(valueId) ? values[valueId].ToString() : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ReportRequestInformation> GetReportRequestInformation()
        {
            var requests = new List<ReportRequestInformation>();

            try
            {
                IReports reportService = Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem") as IReports;
                requests = reportService == null ? requests : reportService.GetCurrentRequests();
            }
            catch (Exception e)
            {
            }

            return requests;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ReportThreadInformation> GetReportThreadInformation()
        {
            var threads = new List<ReportThreadInformation>();

            try
            {
                IReports reportService = Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem") as IReports;
                threads = reportService == null ? threads : reportService.GetCurrentThreads();
            }
            catch (Exception e)
            {
            }

            return threads;
        }

        public ArrayList CreateDrillDownList(Guid basetableid, int employeeid, int subAccountID)
        {

            ArrayList lst = new ArrayList();
            object[] values;

            DBConnection dbcon = new DBConnection(cAccounts.getConnectionString(AccountID));
            dbcon.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            using (SqlDataReader reader = dbcon.GetReader("select reportid, reportname from reportsview where reporttype = 1 and (folderid is null or (folderid is not null and  (personalFolder = 0 or (personalFolder = 1 and employeeid = @employeeID)))) order by reportname"))
            {
                while (reader.Read())
                {
                    lst.Add(new object[] { reader.GetGuid(0), reader.GetString(1) });
                }
                reader.Close();
            }


            return lst;
        }

        /// <summary>
        /// Generate HTML for grid showing available reports.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="claimants">If true, only show reports that are for claimaints</param>
        /// <returns></returns>
        public string[] generateGrid(CurrentUser user, bool claimants)
        {
            const string sql = "select reportID, globalReport, null as export, category, reportname, owner, description, employeeid, reportarea, useJoinVia from dbo.reportsview";
            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridreports", sql)
            {
                KeyField = "reportID",
                enableupdating = true,
                enabledeleting = true,
                editlink = "javascript:editReport('{reportID}', '{usejoinvia}');",
                deletelink = "javascript:deleteReport('{reportID}');"
            };
            grid.getColumnByName("reportID").hidden = true;
            grid.getColumnByName("employeeid").hidden = true;
            grid.getColumnByName("reportarea").hidden = true;
            grid.getColumnByName("useJoinVia").hidden = true;
            grid.getColumnByName("globalReport").HeaderText = "<img alt='Standard Report' src='../images/icons/16/Plain/earth2.png' border='0' height='16' width='16' />";

            grid.addEventColumn("export", "../images/icons/16/Plain/export1.png", "javascript:showExportOptions('{reportid}')", "Export", "Export");
            grid.WhereClause = "(folderid is null or (folderid is not null and  (personalFolder = 0 or (personalFolder = 1 and employeeid = @employeeID)))) and (subaccountid is null or subaccountid = @subaccountID)";

            var clsfields = new cFields(user.AccountID);
            var reportview = new cTables(user.AccountID).GetTableByName("reportsview");
            grid.addFilter(clsfields.GetBy(reportview.TableID, "employeeid"), "@employeeID", user.EmployeeID);
            grid.addFilter(clsfields.GetBy(reportview.TableID, "subAccountId"), "@subaccountID", user.CurrentSubAccountId);
            grid.WhereClause += string.Format(" and (reportarea is null or reportarea = {0} or reportarea = 0)", (int)user.CurrentActiveModule);
            if (claimants)
            {
                grid.WhereClause += " and forclaimants = 1";
            }

            // Initialise row method implementation
            var gridInfo = new SerializableDictionary<string, object> { { "employeeid", user.EmployeeID }, { "claimants", Convert.ToInt32(claimants) } };

            grid.InitialiseRowGridInfo = gridInfo;
            grid.InitialiseRow += new cGridNew.InitialiseRowEvent(this.grid_InitialiseRow);
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cReports";
            grid.ServiceClassMethodForInitialiseRowEvent = "grid_InitialiseRow";
            string[] gridData = grid.generateGrid();
            return gridData;
        }
        private void grid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            int employeeID = Convert.ToInt32(gridInfo["employeeid"]);
            int claimants = Convert.ToInt32(gridInfo["claimants"]);
            if (row.getCellByID("employeeid").Value == DBNull.Value || (row.getCellByID("employeeid").Value != DBNull.Value && (int)row.getCellByID("employeeid").Value != employeeID))
            {
                row.enabledeleting = false;
                row.enableupdating = false;
            }
            if ((int)row.getCellByID("globalReport").Value == 1)
            {
                row.getCellByID("globalReport").Value = "<img alt=\"Standard Report\" src=\"../images/icons/16/Plain/earth2.png\" border=\"0\" height=\"16\" width=\"16\" />";
            }
            else
            {
                row.getCellByID("globalReport").Value = "&nbsp;";
            }

            byte reportType = 1;
            if (row.getCellByID("reportType") != null)
            {
                Byte.TryParse(row.getCellByID("reportType").Text, out reportType);
            }

            if (reportType != 0)
            {
                row.getCellByID("reportname").Value = "<a href=\"javascript:runReport('" + ((string)row.getCellByID("reportname").Value).Replace("'", "\\'") + "','" + row.getCellByID("reportID").Value + "'," + claimants + ", " + row.getCellByID("reportArea") + ");\">" + row.getCellByID("reportname").Value.ToString().Replace(" ", "&nbsp;") + "</a>";    
            }
            
            row.getCellByID("export").Value = "<a href=\"javascript:showExportOptions('" + row.getCellByID("reportID").Value + "', 'lnk" + row.getCellByID("reportID").Value.ToString().Replace("-", "") + "');\" id=\"lnk" + row.getCellByID("reportID").Value.ToString().Replace("-", "") + "\"><img src=\"../images/icons/16/Plain/export1.png\" border=\"0\" alt=\"Export\" title=\"Export\" height=\"16\" width=\"16\" /></a>";
            row.getCellByID("description").Value = row.getCellByID("description").Value.ToString().Replace(" ", "&nbsp");
            row.getCellByID("owner").Value = row.getCellByID("owner").Value.ToString().Replace(" ", "&nbsp");
            row.getCellByID("category").Value = row.getCellByID("category").Value.ToString().Replace(" ", "&nbsp");
        }

        /// <summary>
        /// Add any columns required by the system, view reports exports etc.
        /// </summary>
        /// <param name="columns">
        ///     The columns.
        /// </param>
        /// <param name="fields">
        ///     The fields.
        /// </param>
        /// <param name="reportGuid">
        ///     The report GUID.
        /// </param>
        /// <param name="joinVias">
        ///     The join VIAS.
        /// </param>
        /// <param name="reportOn">The guid (as a string) of the report base table</param>
        public static void AddSystemColumns(ArrayList columns, IFields fields, Guid reportGuid, JoinVias joinVias, cTable reportOn)
        {
            if (reportGuid == Guid.Empty)
            {
                return;
            }

            var reportType = cReport.getReportType(columns);
            if ((reportOn.TableID == new Guid(ReportTable.Claims) || reportOn.TableID == new Guid(ReportTable.SavedExpenses)) && reportType == ReportType.Item)
            {
                // add claimid for view claim link
                var col = new cStandardColumn(new Guid(), reportGuid, ReportColumnType.Standard, ColumnSort.None, columns.Count + 1, fields.GetFieldByID(new Guid(ReportKeyFields.ClaimsClaimId)), false, false, false, false, false, false, true, systemColumn: true);
                if (reportOn.TableID != new Guid(ReportTable.Claims))
                {
                    var joinViaList = new SortedList<int, JoinViaPart>();
                    var joinPart = new JoinViaPart(new Guid(ReportKeyFields.SavedexpensesClaimId), JoinViaPart.IDType.RelatedTable);
                    joinViaList.Add(0, joinPart);
                    var joinVia = new JoinVia(0, "ClaimID", Guid.Empty, joinViaList);
                    var joinViaId = joinVias.SaveJoinVia(joinVia);
                    col.JoinVia = joinVias.GetJoinViaByID(joinViaId);
                }

                columns.Add(col);

                if (reportOn.TableID == new Guid(ReportTable.SavedExpenses))
                {
                    col = new cStandardColumn(new Guid(), reportGuid, ReportColumnType.Standard, ColumnSort.None, columns.Count + 1, fields.GetFieldByID(new Guid(ReportKeyFields.SavedexpensesExpenseId)), false, false, false, false, false, false, true, systemColumn: true);
                    columns.Add(col);
                }
            }

            if ((reportOn.TableID == new Guid(ReportTable.ContractDetails) || reportOn.TableID == new Guid(ReportTable.ContractProductDetails)) && reportType == ReportType.Item)
            {
                // add contractid for view contract link
                columns.Add(new cStandardColumn(new Guid(), reportGuid, ReportColumnType.Standard, ColumnSort.None, columns.Count + 1, fields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId)), false, false, false, false, false, false, true, systemColumn: true));

                foreach (cReportColumn repCol in columns)
                {
                    if (repCol.columntype == ReportColumnType.Standard)
                    {
                        var tmpCol = (cStandardColumn)repCol;
                        if (tmpCol.field.TableID.ToString() == ReportTable.ContractProductDetails)
                        {
                            columns.Add(new cStandardColumn(new Guid(), reportGuid, ReportColumnType.Standard, ColumnSort.None, columns.Count + 1, fields.GetFieldByID(new Guid(ReportKeyFields.ContractProducts_ConProdId)), false, false, false, false, false, false, true, systemColumn: true));
                            break;
                        }
                    }
                }
            }

            if ((reportOn.TableID == new Guid(ReportTable.SupplierDetails) || reportOn.TableID == new Guid(ReportTable.SupplierContacts)) && reportType == ReportType.Item)
            {
                var joinVia = CreatejoinVia(ReportFields.SupplierContactsSupplierId, JoinViaPart.IDType.Field, "Supplier : SupplierDetails", joinVias);
                var col = new cStandardColumn(new Guid(), reportGuid, ReportColumnType.Standard, ColumnSort.None, columns.Count + 1, fields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)), false, false, false, false, false, false, true, systemColumn: true);
                col.JoinVia = joinVia;
                columns.Add(col);
            }

            if (reportOn.TableID == new Guid(ReportTable.Tasks) && reportType == ReportType.Item)
            {
                columns.Add(new cStandardColumn(new Guid(), reportGuid, ReportColumnType.Standard, ColumnSort.None, columns.Count + 1, fields.GetFieldByID(new Guid(ReportKeyFields.Tasks_TaskId)), false, false, false, false, false, false, true, systemColumn: true));
            }

        }

        /// <summary>
        /// Create a new join via object and then save, if already exist, return the exisint one, else return new one.
        /// </summary>
        /// <param name="viaId">The ID of the single step</param>
        /// <param name="idType">The join type</param>
        /// <param name="joinName">The name of the join</param>
        /// <param name="joinVias">the JOINVIAS class</param>
        /// <returns>The new or exisiting JOINVIA.</returns>
        public static JoinVia CreatejoinVia(string viaId, JoinViaPart.IDType idType, string joinName, JoinVias joinVias)
        {
            var joinViaList = new SortedList<int, JoinViaPart>();
            var joinPart = new JoinViaPart(new Guid(viaId), idType);
            joinViaList.Add(0, joinPart);
            var joinVia = new JoinVia(0, joinName, Guid.Empty, joinViaList);
            var joinViaId = joinVias.SaveJoinVia(joinVia);
            joinVia = joinVias.GetJoinViaByID(joinViaId);
            return joinVia;
        }

        /// <summary>
        /// Get a list of <see cref="ReportBasic"/> which have no runtime criteria or static text
        /// </summary>
        /// <param name="accountId">The account ID to get reports for</param>
        /// <returns>The list of <see cref="ReportBasic"/></returns>
        public static List<ReportBasic> GetReportIdWithNoRuntimeItems(int accountId)
        {
            var result = new List<ReportBasic>();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                using (var reader = expdata.GetReader("select distinct rep.reportid, rep.reportname  from reportsview as rep where rep.reportID not in (select reportid from reportcolumnsview where runtime = 1 union select reportID from reportcriteriaview where runtime = 1) "))
                {
                    while (reader.Read())
                    {
                        result.Add( new ReportBasic(reader.GetString(1), reader.GetGuid(0)));
                    }
                }
            }

            return result;
        }
    }
}
