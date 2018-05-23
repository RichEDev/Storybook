namespace Expenses_Reports
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;

    using ConsoleBootstrap;

    using Infragistics.WebUI.UltraWebCalcManager;
    using Infragistics.WebUI.UltraWebGrid;

    using SimpleInjector;

    using Spend_Management;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Logic_Classes.Fields;
    using SpendManagementLibrary.Definitions.JoinVia;
    #endregion

    /// <summary>
    /// The c reports svc.
    /// </summary>
    public class cReportsSvc : MarshalByRefObject, IReports
    {
        #region Static Fields

        /// <summary>
        /// The connection strings.
        /// </summary>
        public static ConcurrentDictionary<int, string> ConnectionStrings = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// The list of report requests.
        /// </summary>
        private static readonly List<cReportRequest> lstReportRequests = new List<cReportRequest>();

        /// <summary>
        /// The list of report threads.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Thread> lstReportThreads =
            new ConcurrentDictionary<string, Thread>();

        /// <summary>
        /// The  number of threads.
        /// </summary>
        private static int nNumberOfThreads;

        /// <summary>
        /// The  thread poll interval.
        /// </summary>
        private static int nThreadPollInterval;

        #endregion

        #region Fields

        private readonly bool bTimersStarted;

        private readonly TimerCallback timeCB = processReports;

        private readonly TimerCallback timeCleanupCB = removeReports;

        private readonly Timer tmr;

        private readonly Timer tmrCleanup;

        static int ReportCleanupTimeout;

        #endregion

        #region Constructors and Destructors

        static cReportsSvc()
        {
            try
            {
                ReportCleanupTimeout = int.Parse(ConfigurationManager.AppSettings["ReportCleanupTimeout"]);
            }
            catch (Exception)
            {

                ReportCleanupTimeout = 60;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="cReportsSvc"/> class.
        /// </summary>
        public cReportsSvc()
        {
            string numThreads = ConfigurationManager.AppSettings["NumberOfThreads"];
            string pollInterval = ConfigurationManager.AppSettings["ThreadPollInterval"];

            int tmpNumThreads = numThreads == null ? 5 : Convert.ToInt32(numThreads);
            int tmpPollInterval = pollInterval == null ? 1000 : Convert.ToInt32(pollInterval);

            // don't want to recreate the timers every time a connection is made to the report service! Only update the timers if the interval has changed
            if (this.tmr == null)
            {
                this.tmr = new Timer(this.timeCB, null, 0, tmpPollInterval);
            }

            if (this.tmrCleanup == null)
            {
                this.tmrCleanup = new Timer(this.timeCleanupCB, null, tmpPollInterval, tmpPollInterval);
            }

            string methodMessage;
            if (tmpPollInterval != this.ThreadPollInterval)
            {
                methodMessage = string.Format("Polling interval updated to {0}", tmpPollInterval);
                Service1.DiagLog("cReportsSvc", methodMessage);
                nThreadPollInterval = tmpPollInterval;

                if (this.bTimersStarted)
                {
                    Service1.DiagLog("cReportsSvc", "Updating timers with new polling interval");
                    this.tmr.Change(0, tmpPollInterval);
                    this.tmrCleanup.Change(tmpPollInterval, tmpPollInterval);
                }
            }

            if (tmpNumThreads != NumberOfThreads)
            {
                methodMessage = string.Format("Updating number of threads to {0}", tmpNumThreads);
                Service1.DiagLog("cReportsSvc", methodMessage);
                nNumberOfThreads = tmpNumThreads;
            }

            this.bTimersStarted = true;
        }

        #endregion

        #region Delegates

        public delegate object delProcesReport(cReportRequest request);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether list in use.
        /// </summary>
        public static bool ListInUse { get; set; }

        /// <summary>
        /// Gets the number of threads.
        /// </summary>
        public static int NumberOfThreads
        {
            get
            {
                return nNumberOfThreads;
            }
        }

        /// <summary>
        /// Gets the report requests.
        /// </summary>
        public static List<cReportRequest> ReportRequests
        {
            get
            {
                return lstReportRequests;
            }
        }

        /// <summary>
        /// Gets the report threads.
        /// </summary>
        public static ConcurrentDictionary<string, Thread> ReportThreads
        {
            get
            {
                return lstReportThreads;
            }
        }

        /// <summary>
        /// Gets the thread poll interval.
        /// </summary>
        public int ThreadPollInterval
        {
            get
            {
                return nThreadPollInterval;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static string GetConnectionString(int accountId)
        {
            string connectionstring;
            ConnectionStrings.TryGetValue(accountId, out connectionstring);
            if (string.IsNullOrEmpty(connectionstring))
            {
                using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
                {
                    connection.sqlexecute.Parameters.AddWithValue("@accountid", accountId);
                    using (
                        IDataReader reader =
                            connection.GetReader(
                                "select hostname, dbname, dbusername, dbpassword from dbo.registeredusers inner join dbo.databases on dbo.databases.databaseid = dbo.registeredusers.dbserver where accountid = @accountid"))
                    {
                        while (reader.Read())
                        {
                            var secure = new cSecureData();
                            string server = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            string name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            string userName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            string password = reader.IsDBNull(3) ? string.Empty : secure.Decrypt(reader.GetString(3));

                            var connectionStringBuilder = new SqlConnectionStringBuilder
                            {
                                DataSource = server,
                                InitialCatalog = name,
                                UserID = userName,
                                Password = password,
                                MaxPoolSize = 10000,
                                ApplicationName = GlobalVariables.DefaultApplicationInstanceName
                            };
                            connectionstring = connectionStringBuilder.ConnectionString;
                            ConnectionStrings.TryAdd(accountId, connectionstring);
                        }

                        reader.Close();
                        connection.sqlexecute.Parameters.Clear();
                    }
                }
            }

            return connectionstring;
        }

        public static object processReport(cReportRequest request)
        {
            //Bootstrap container and add to Funky Injector
            FunkyInjector.Container = Bootstrapper.Bootstrap(request.accountid);

            var clsreports = new cReports(request.accountid);
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(request.accountid);
            var subAccount = clsSubAccounts.getSubAccountById(request.SubAccountId);
            cAccountProperties accountProperties = subAccount != null ? subAccount.SubAccountProperties : new cAccountProperties();
            try
            {
                if (request.exporttype == ExportType.Viewer || request.exporttype == ExportType.Preview)
                {
                    using (var connection = new DatabaseConnection(GetConnectionString(request.accountid)))
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@employeeID", request.employeeid);
                        connection.sqlexecute.Parameters.AddWithValue("@reportID", request.report.reportid);
                        connection.sqlexecute.Parameters.AddWithValue("@subaccountID", request.SubAccountId);
                        connection.ExecuteProc("updateLastReportAccess");
                        connection.sqlexecute.Parameters.Clear();
                    }

                    var calcman = new UltraWebCalcManager();
                    var clstext = new cText();
                    var clsexcel = new cExcel();
                    var clsRow = new cRowFunction();
                    var clsColumn = new cColumnFunction();
                    var column = new ColFunction();
                    var clsAddress = new cAddressFunction();
                    var clsCarriage = new cCarriageReturn();
                    var clsReplaceText = new cReplaceTextFunction();

                    calcman.RegisterUserDefinedFunction(clstext);
                    calcman.RegisterUserDefinedFunction(clsexcel);
                    calcman.RegisterUserDefinedFunction(clsRow);
                    calcman.RegisterUserDefinedFunction(clsColumn);
                    calcman.RegisterUserDefinedFunction(column);
                    calcman.RegisterUserDefinedFunction(clsAddress);
                    calcman.RegisterUserDefinedFunction(clsCarriage);
                    calcman.RegisterUserDefinedFunction(clsReplaceText);
                    
                    var clsFields = new SubAccountFields(new cFields(request.accountid), new FieldRelabler(accountProperties));
                    return clsreports.createReport(request, clsFields, new cJoins(request.accountid), new UltraWebGrid(), calcman);
                }

                string threadname = GetThreadName(request);

                string methodMessage = $"exporting report : {threadname} [{request.report.reportname}]";
                Service1.DiagLog("processReport", methodMessage);
                
                var relabeler = new FieldRelabler(accountProperties);

                var clsexports = new cExports(relabeler);

                byte[] export = null;
                switch (request.report.exportoptions.application)
                {
                    case FinancialApplication.CustomReport:
                        switch (request.exporttype)
                        {
                            case ExportType.Excel:
                                methodMessage = $"calling exportExcel() for report {request.report.reportname}";
                                Service1.DiagLog("processReport", methodMessage);
                                export = clsexports.exportExcel(request);
                                break;
                            case ExportType.Pivot:
                                methodMessage = $"calling exportPivot() for report {request.report.reportname}";
                                Service1.DiagLog("processReport", methodMessage);
                                export = clsexports.exportPivot(request);
                                break;
                            case ExportType.CSV:
                                methodMessage = $"calling exportCSV() for report {request.report.reportname}";
                                Service1.DiagLog("processReport", methodMessage);
                                export = clsexports.exportCSV(request);
                                break;
                            case ExportType.FlatFile:
                                methodMessage = $"calling exportFlatfile() for report {request.report.reportname}";
                                Service1.DiagLog("processReport", methodMessage);
                                export = clsexports.exportFlatfile(request);
                                break;
                        }

                        methodMessage = $"finished export for report {request.report.reportname}";
                        Service1.DiagLog("processReport", methodMessage);
                        break;
                    case FinancialApplication.ESR:
                        if (request.report.basetable.TableID == new Guid(ReportTable.SavedExpenses))
                        {
                            var clsfields = new cFields(request.accountid);
                            var joinVias = new JoinVias(cMisc.GetCurrentUser($"{request.accountid},{request.SubAccountId}"));
                            var esrAssignId = new cStandardColumn(
                                Guid.Empty,
                                Guid.Empty,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                request.report.columns.Count + 1,
                                clsfields.GetFieldByID(new Guid(ReportKeyFields.EsrAssignId)),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false);
                            
                            var joinViaList = new SortedList<int, JoinViaPart>();
                            var joinPart = new JoinViaPart(new Guid(ReportFields.EsrAssignmentEsrAssignId), JoinViaPart.IDType.Field);
                            joinViaList.Add(0, joinPart);
                            var joinVia = new JoinVia(0, "Saved Expenses to Esr Assignment", Guid.Empty, joinViaList);
                            var joinViaId = joinVias.SaveJoinVia(joinVia);
                            esrAssignId.JoinVia = joinVias.GetJoinViaByID(joinViaId);
                            request.report.columns.Add(esrAssignId);

                            


                            var employeeId = new cStandardColumn(
                                Guid.Empty,
                                Guid.Empty,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                request.report.columns.Count + 1,
                                clsfields.GetFieldByID(new Guid(ReportKeyFields.EmployeesEmployeeId)),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false);
                            joinViaList = new SortedList<int, JoinViaPart>();
                            joinPart = new JoinViaPart(new Guid(ReportFields.SavedExpensesClaimId), JoinViaPart.IDType.Field);
                            joinViaList.Add(0, joinPart);
                            joinPart = new JoinViaPart(new Guid(ReportFields.ClaimsEmployee), JoinViaPart.IDType.Field);
                            joinViaList.Add(1, joinPart);
                            joinVia = new JoinVia(0, "Saved Expenses to Employee", Guid.Empty, joinViaList);
                            joinViaId = joinVias.SaveJoinVia(joinVia);
                            employeeId.JoinVia = joinVias.GetJoinViaByID(joinViaId);

                            request.report.columns.Add(employeeId);

                            var subcatId = new cStandardColumn(
                                Guid.Empty,
                                Guid.Empty,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                request.report.columns.Count + 1,
                                clsfields.GetFieldByID(new Guid(ReportKeyFields.SubCatsSubCatId)),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false);

                            joinViaList = new SortedList<int, JoinViaPart>();
                            joinPart = new JoinViaPart(new Guid(ReportFields.SavedExpensesSubCatId), JoinViaPart.IDType.Field);
                            joinViaList.Add(0, joinPart);
                            joinVia = new JoinVia(0, "Saved Expenses to Subcats", Guid.Empty, joinViaList);
                            joinViaId = joinVias.SaveJoinVia(joinVia);
                            subcatId.JoinVia = joinVias.GetJoinViaByID(joinViaId);

                            request.report.columns.Add(subcatId);


                            var assignmentNumber = new cStandardColumn(
                                Guid.Empty,
                                Guid.Empty,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                request.report.columns.Count + 1,
                                clsfields.GetFieldByID(new Guid(ReportFields.EsrAssignmentAssignmentNumber)),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false);
                            assignmentNumber.JoinVia = esrAssignId.JoinVia;
                            request.report.columns.Add(assignmentNumber);


                            var costCodesEsrCostCode = new cStandardColumn(
                                Guid.Empty,
                                Guid.Empty,
                                ReportColumnType.Standard,
                                ColumnSort.None,
                                request.report.columns.Count + 1,
                                clsfields.GetFieldByID(new Guid(ReportFields.CostCodesEsrCostCode)),
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false);

                            joinViaList = new SortedList<int, JoinViaPart>();
                            joinPart = new JoinViaPart(new Guid(ReportKeyFields.SavedExpensesCostCodesExpenseId), JoinViaPart.IDType.RelatedTable);
                            joinViaList.Add(0, joinPart);
                            joinPart = new JoinViaPart(new Guid(ReportKeyFields.SavedExpensesCostCodesCostCodeId), JoinViaPart.IDType.Field);
                            joinViaList.Add(1, joinPart);
                            joinVia = new JoinVia(0, "Saved Expenses to CostCodes", Guid.Empty, joinViaList);
                            joinViaId = joinVias.SaveJoinVia(joinVia);
                            costCodesEsrCostCode.JoinVia = joinVias.GetJoinViaByID(joinViaId);

                            request.report.columns.Add(costCodesEsrCostCode);


                            request.report.columns.Add(
                                new cStandardColumn(
                                    Guid.Empty,
                                    Guid.Empty,
                                    ReportColumnType.Standard,
                                    ColumnSort.None,
                                    request.report.columns.Count + 1,
                                    clsfields.GetFieldByID(new Guid(ReportFields.SavedExpensesDeriveCostcodeForEsrReport )),
                                    false,
                                    false,
                                    false,
                                    false,
                                    false,
                                    false,
                                    false));
                        }

                        methodMessage = $"calling exportESR() for report {request.report.reportname}";
                        Service1.DiagLog("processReport", methodMessage);
                        export = clsexports.exportESR(request);
                        break;
                }

                if (export != null)
                {
                    string reportFilePath = ConfigurationManager.AppSettings["ReportsOutputFilePath"];
                    if (string.IsNullOrEmpty(reportFilePath))
                    {
                        var ex = new ArgumentNullException(
                            reportFilePath, @"ReportsOutputFilePath key is null or empty in configuration file.");
                        throw ex;
                    }

                    if (!reportFilePath.EndsWith("\\"))
                    {
                        reportFilePath += "\\";
                    }

                    Directory.CreateDirectory(reportFilePath);
                    reportFilePath = string.Format("{0}{1}.sel", reportFilePath, request.ReportFileGuid);
                    File.WriteAllBytes(reportFilePath, export);
                }

                return export;
            }
            catch (Exception ex)
            {
                var errorHandler = new ErrorHandlerService();
                errorHandler.sendError(request, ex);

                string threadname = GetThreadName(request);
                string methodMessage = string.Format("errorCatch : {0} failed", threadname);
                LogError(ex, "processReport", methodMessage);
                AddAuditLogEntry(request.accountid, request.employeeid, string.Format("An error occured whilst running report {0}", request.report.reportname));

                request.Status = ReportRequestStatus.Failed;
                request.Exception = ex;
                if (ReportThreads.ContainsKey(threadname))
                {
                    Thread oldThread;
                    ReportThreads.TryRemove(threadname, out oldThread);
                    oldThread.Abort();
                }

                return new byte[] { };
            }
        }

        public static void reportComplete(IAsyncResult a)
        {
            var request = (cReportRequest)a.AsyncState;
            var result = (AsyncResult)a;
            var process = (delProcesReport)result.AsyncDelegate;

            string threadname = GetThreadName(request);
            string methodMessage = string.Format("reportComplete : {0}", threadname);
            Service1.DiagLog("reportComplete", methodMessage);

            request.CompletionTime = DateTime.Now;
            try
            {
                var data = process.EndInvoke(a);
                if (request.Status != ReportRequestStatus.Failed)
                {
                    request.ReportData = data;
                    request.Status = ReportRequestStatus.Complete;

                    methodMessage = string.Format("{0} got reportData", threadname);
                    Service1.DiagLog("reportComplete", methodMessage);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "reportComplete", string.Format("{0} failed.", threadname));
                request.Status = ReportRequestStatus.Failed;
                request.Exception = ex;
                var clsEmails = new cEmails();
                clsEmails.sendErrorMail(ex, request);
            }
            finally
            {
                if (ReportThreads.ContainsKey(threadname))
                {
                    Thread oldThread;
                    ReportThreads.TryRemove(threadname, out oldThread);
                    oldThread?.Abort();
                }
            }
        }

        public static void startReportThread(object data)
        {
            var request = (cReportRequest)data;
            delProcesReport process = processReport;
            process.BeginInvoke(request, reportComplete, request);
        }

        /// <summary>
        /// Returns information relating to the current running reports
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ReportRequestInformation> GetCurrentRequests()
        {
            return (from cReportRequest r in lstReportRequests
                    select
                        new ReportRequestInformation
                        {
                            AccountId = r.accountid,
                            CompletionTime = r.CompletionTime,
                            ExportType = r.exporttype,
                            ProcessedRows = r.ProcessedRows,
                            ReportJoinSql = r.report == null ? "Null" : r.report.JoinSQL,
                            ReportMaxRowLimit = r.report == null ? -1 : r.report.Limit,
                            ReportName = r.report == null ? "Null" : r.report.reportname,
                            ReportRunFrom = r.reportRunFrom,
                            ReportStaticSql =
                                r.report == null ? "Null" : r.report.StaticReportSQL,
                            ReportType =
                                r.report == null ? ReportType.None : r.report.getReportType(),
                            RowCount = r.report == null ? -1 : r.RowCount,
                            SchedulerRequest = r.SchedulerRequestID != Guid.Empty,
                            Status = r.Status,
                            SubAccountId = r.SubAccountId
                        }).ToList();
        }

        /// <summary>
        /// Returns information relating to the current running threads
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ReportThreadInformation> GetCurrentThreads()
        {
            return (from KeyValuePair<string, Thread> kvp in lstReportThreads
                    select
                        new ReportThreadInformation
                        {
                            ThreadKey = kvp.Key,
                            ManagedId = kvp.Value.ManagedThreadId,
                            Name = kvp.Value.Name,
                            State = kvp.Value.ThreadState
                        }).ToList();
        }

        public Exception GetReportError(cReportRequest request)
        {
            cReportRequest r = this.getReportRequest(request);

            return r.Exception;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public int ReportThreadsRunning()
        {
            int reportThreads = 0;
            try
            {
                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;
                    reportThreads = lstReportThreads.Count;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "ReportThreadsRunning");
            }
            finally
            {
                ListInUse = false;
            }

            return reportThreads;
        }

        public int ReportsInList()
        {
            int reportsInList = 0;
            try
            {
                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;
                    reportsInList = lstReportRequests.Count;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "ReportsInList");
            }
            finally
            {
                ListInUse = false;
            }

            return reportsInList;
        }

        public int ReportsRunning()
        {
            int reportsRunning = 0;

            try
            {
                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;

                    reportsRunning = lstReportRequests.Count(x => x.Status == ReportRequestStatus.BeingProcessed);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "ReportsRunning");
            }
            finally
            {
                ListInUse = false;
            }

            return reportsRunning;
        }

        public void cancelRequest(cReportRequest request)
        {
            try
            {
                string threadname = GetThreadName(request);
                if (ReportThreads.ContainsKey(threadname))
                {
                    Thread thread;
                    ReportThreads.TryRemove(threadname, out thread);
                    thread.Abort();
                }

                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;
                    for (int i = lstReportRequests.Count - 1; i >= 0; i--)
                    {
                        cReportRequest req = lstReportRequests[i];
                        if (req.requestnum == request.requestnum && req.report.reportid == request.report.reportid
                            && req.accountid == request.accountid && req.SubAccountId == request.SubAccountId
                            && req.employeeid == request.employeeid
                            && req.SchedulerRequestID == request.SchedulerRequestID)
                        {
                            Service1.DiagLog("cancelRequest", threadname);
                            lstReportRequests.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "cancelRequest");
            }
            finally
            {
                ListInUse = false;
            }
        }

        public bool createReport(cReportRequest request)
        {
            try
            {
                if (request.exporttype != ExportType.Viewer && request.exporttype != ExportType.Preview)
                {
                    if (request.report.basetable.TableID.ToString() == ReportTable.ContractDetails
                        || request.report.basetable.TableID.ToString() == ReportTable.ContractProductDetails)
                    {
                        // && request.report.getReportType() == ReportType.Item
                        var clsfields = new cFields(request.accountid);
                        cReportColumn col = new cStandardColumn(
                            Guid.NewGuid(),
                            request.report.reportid,
                            ReportColumnType.Standard,
                            ColumnSort.None,
                            request.report.columns.Count + 1,
                            clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractDetails_ContractId)),
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            true);
                        col.system = true;
                        request.report.columns.Add(col);

                        for (int x = 0; x < request.report.columns.Count; x++)
                        {
                            var repCol = (cReportColumn)request.report.columns[x];
                            if (repCol.columntype == ReportColumnType.Standard)
                            {
                                var tmpCol = (cStandardColumn)repCol;
                                if (tmpCol.field.TableID.ToString() == ReportTable.ContractProductDetails)
                                {
                                    cReportColumn reportColumn = new cStandardColumn(
                                        Guid.NewGuid(),
                                        request.report.reportid,
                                        ReportColumnType.Standard,
                                        ColumnSort.None,
                                        request.report.columns.Count + 1,
                                        clsfields.GetFieldByID(new Guid(ReportKeyFields.ContractProducts_ConProdId)),
                                        false,
                                        false,
                                        false,
                                        false,
                                        false,
                                        false,
                                        true);
                                    reportColumn.system = true;
                                    request.report.columns.Add(reportColumn);
                                    break;
                                }
                            }
                        }
                    }

                    // if supplier report, put View Supplier link column on the report
                    if (request.report.basetable.TableID.ToString() == ReportTable.SupplierDetails
                        || request.report.basetable.TableID.ToString() == ReportTable.SupplierContacts)
                    {
                        // && request.report.getReportType() == ReportType.Item
                        var clsfields = new cFields(request.accountid);
                        cReportColumn col = new cStandardColumn(
                            Guid.NewGuid(),
                            request.report.reportid,
                            ReportColumnType.Standard,
                            ColumnSort.None,
                            request.report.columns.Count + 1,
                            clsfields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)),
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            true);
                        col.system = true;
                        request.report.columns.Add(col);
                    }
                }

                if (this.requestExists(request))
                {
                    return false;
                }

                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;
                    try
                    {
                        lstReportRequests.Add(request);
                    }
                    catch (Exception repAdd)
                    {
                        LogError(repAdd, "createReport", "Error adding report request");
                    }
                    finally
                    {
                        ListInUse = false;
                    }
                }

                request.Status = ReportRequestStatus.Queued;
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex, "CreateReport");
                return false;
            }
        }

        public DataSet createSynchronousReport(cReportRequest request)
        {
            var clsreports = new cReports(request.accountid);
            return clsreports.createReport(request, new cFields(request.accountid), new cJoins(request.accountid), new UltraWebGrid(), new UltraWebCalcManager());
        }

        public byte[] exportReport(cReportRequest request)
        {
            return null;
        }

        public cExportOptions getExportOptions(int accountid, int employeeid, Guid reportid, bool isFinancialExport = false)
        {
            var clsreports = new cReports(accountid);
            var report = clsreports.getReportById(reportid);
            return clsreports.getExportOptions(employeeid, report, isFinancialExport);
        }

        public DataSet getHistoryGrid(int accountid, Guid reportid)
        {
            var clsreports = new cReports(accountid);
            return clsreports.getHistoryGrid(reportid);
        }

        public cReport getReportById(int accountid, Guid reportid)
        {
            var clsreports = new cReports(accountid);
            return clsreports.getReportById(reportid);
        }

        public int getReportCount(cReportRequest request)
        {
            var clsreports = new cReports(request.accountid);
            ICurrentUserBase user = cMisc.GetCurrentUser(string.Format("{0},{1}", request.accountid, request.report.employeeid.HasValue ? request.report.employeeid : 0));
            return clsreports.getReportCount(request, request.accountid, new cFields(request.accountid), new cJoins(request.accountid), new JoinVias(user));
        }

        public object getReportData(cReportRequest request)
        {
            cReportRequest r = this.getReportRequest(request);

            if (r != null && (r.Status != ReportRequestStatus.Complete || r.ReportData == null))
            {
                return null;
            }

            if (r == null)
            {
                return null;
            }

            object data = r.ReportData;

            return data;
        }

        /// <summary>
        /// This method get report data from the Report Service to Expedite 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Object array with export data and export history id</returns>
        public object[] getReportDataForExpedite(cReportRequest request)
        {
            cReportRequest r = this.getReportRequest(request);

            if (r != null && (r.Status != ReportRequestStatus.Complete || r.ReportData == null))
            {
                return null;
            }

            if (r == null)
            {
                return null;
            }

            return new object[]
                        {
                          r.ReportData, r.report.exportoptions.exporthistoryid
                        };
        }


        public object[] getReportProgress(cReportRequest request)
        {
            cReportRequest r = this.getReportRequest(request);

            if (r == null)
            {
                return null;
            }

            return new object[]
                       {
                           r.Status.ToString(), r.PercentageProcessed, r.report.reportname, r.RowCount,
                           r.exporttype.ToString()
                       };
        }

        public DataSet refreshReportData(cReportRequest request)
        {
            var clsreports = new cReports(request.accountid);
            return clsreports.createReport(request, new cFields(request.accountid), new cJoins(request.accountid), new UltraWebGrid(), new UltraWebCalcManager() );
        }

        public bool test()
        {
            return true;
        }

        public void updateDrillDownReport(int accountid, int employeeid, Guid reportid, Guid drilldown)
        {
            var clsreports = new cReports(accountid);
            var report = clsreports.getReportById(reportid);
            clsreports.updateDrillDownReport(employeeid, report, drilldown);
        }

        public void updateExportOptions(int accountid, cExportOptions options)
        {
            var clsreports = new cReports(accountid);
            clsreports.updateExportOptions(options);
        }

        public DataSet GetChartData(cReportRequest request, DataSet chartData)
        {
            var clsreports = new cReports(request.accountid);
            return clsreports.GenerateChart(request, chartData);
        }

        #endregion

        #region Methods

        private static string GetThreadName(cReportRequest r)
        {
            return string.Format(
                "{0}_{1}_{2}_{3}_{4}_{5}",
                r.accountid,
                r.SubAccountId,
                r.employeeid,
                r.report.reportid.ToString(),
                r.exporttype,
                r.SchedulerRequestID.ToString());
        }

        private static void LogError(Exception ex, string methodName, string methodMessage = "")
        {
            string message = ex.Message;
            string methodInfo = methodMessage == string.Empty ? "Error" : methodMessage;
            if (ex.InnerException != null)
            {
                message = string.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException.Message);
            }

            cEventlog.LogEntry(
                string.Format(
                    "ReportEngine : cReportsSvc : {0} : {1} : {2}{3}{4}{5}",
                    methodName,
                    methodInfo,
                    Environment.NewLine,
                    message,
                    Environment.NewLine,
                    ex.StackTrace));
        }

        /// <summary>
        /// Add entry to the audit log
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="employeeId">The employee Id</param>
        /// <param name="message">The message to add in the audit log</param>
        private static void AddAuditLogEntry(int accountId, int employeeId, string message)
        {
            var auditLog = new cAuditLog(accountId, employeeId);
            auditLog.addRecord(SpendManagementElement.Reports, message, 0, fromReportsOrScheduler: true);
        }

        /// <summary>
        /// This method delays the processing of an identical report (whether for different user or schedule) to prevent deadlocks on the data. It will remain queued until other one is completed.
        /// </summary>
        /// <param name="req">
        /// Report to check against current reports being processed
        /// </param>
        /// <returns>
        /// TRUE if the same report is already being processed
        /// </returns>
        private static bool ReportAlreadyBeingProcessed(cReportRequest req)
        {
            bool matchFound = false;
            lock (((ICollection)lstReportRequests).SyncRoot)
            {
                try
                {
                    ListInUse = true;

                    matchFound =
                        lstReportRequests.Any(
                            x =>
                            x.Status == ReportRequestStatus.BeingProcessed && x.report.reportid == req.report.reportid && x.accountid == req.accountid);
                }
                catch (Exception ex)
                {
                    LogError(ex, "ReportAlreadyBeingProcessed");
                }
                finally
                {
                    ListInUse = false;
                }
            }

            return matchFound;
        }

        private static void processReports(object state)
        {
            try
            {
                if (ListInUse)
                {
                    return;
                }

                if (ReportRequests.Count == 0 || ReportThreads.Count >= NumberOfThreads)
                {
                    return;
                }

                int availablethreads = NumberOfThreads - ReportThreads.Count;

                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;
                    int newThreads = 0;

                    foreach (cReportRequest req in lstReportRequests)
                    {
                        if (newThreads < availablethreads)
                        {
                            if (req.Status == ReportRequestStatus.Queued && !ReportAlreadyBeingProcessed(req))
                            {
                                //Bootstrap scheduler as it use SM and SMLib
                                Container container = Bootstrapper.Bootstrap(req.accountid);

                                //Assign container to funky injector
                                FunkyInjector.Container = container;

                                req.Status = ReportRequestStatus.BeingProcessed;
                                var thread = new Thread(startReportThread)
                                {
                                    IsBackground = true,
                                    Name = GetThreadName(req)
                                };

                                string methodMessage = string.Format("thread {0} being started", thread.Name);
                                Service1.DiagLog("processReports", methodMessage);
                                ReportThreads.TryAdd(thread.Name, thread);
                                thread.Start(req);
                                newThreads++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "processReports");
            }
            finally
            {
                ListInUse = false;
            }
        }

        private static void removeReports(object state)
        {
            try
            {
                if (ListInUse)
                {
                    return;
                }

                ListInUse = true;
                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    for (int i = lstReportRequests.Count - 1; i >= 0; i--)
                    {
                        cReportRequest req = lstReportRequests[i];

                        if ((req.Status == ReportRequestStatus.Complete || req.Status == ReportRequestStatus.Failed)
                            && req.CompletionTime < DateTime.Now.AddMinutes(-ReportCleanupTimeout))
                        {
                            lstReportRequests.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "removeReports");
                cEventlog.LogEntry(
                    string.Format(
                        "ReportEngine : cReportsSvc : removeReports : Error: {0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                ListInUse = false;
            }
        }

        private cReportRequest getReportRequest(cReportRequest request)
        {
            cReportRequest returnRequest = null;
            try
            {
                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;

                    returnRequest =
                        lstReportRequests.FirstOrDefault(
                            r =>
                            r.employeeid == request.employeeid && r.report.reportid == request.report.reportid
                            && r.accountid == request.accountid && r.SubAccountId == request.SubAccountId
                            && r.exporttype == request.exporttype && r.SchedulerRequestID == request.SchedulerRequestID);

                    if (returnRequest == null)
                    {
                        var requestInformation = string.Format("employeeid = {0}, report.reportid = {1}, accountid = {2}, SubAccountId = {3}, exporttype = {4}, SchedulerRequestId = {5}", request.employeeid, request.report.reportid, request.accountid, request.SubAccountId, request.exporttype, request.SchedulerRequestID);
                        var allRequestsInformation = lstReportRequests.Aggregate(string.Empty, (current, t) => current + string.Format("employeeid = {0}, report.reportid = {1}, accountid = {2}, SubAccountId = {3}, exporttype = {4}, SchedulerRequestId = {5}", t.employeeid, t.report.reportid, t.accountid, t.SubAccountId, t.exporttype, t.SchedulerRequestID));

                        cEventlog.LogEntry(string.Format("ReportEngine : cReportsSvc : getReportRequest : returning null\n\nCurrent Request:{0}\n\nAll Requests{1}", requestInformation, allRequestsInformation));
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "getReportRequest");
            }
            finally
            {
                ListInUse = false;
            }

            return returnRequest;
        }

        private bool requestExists(cReportRequest request)
        {
            try
            {
                lock (((ICollection)lstReportRequests).SyncRoot)
                {
                    ListInUse = true;
                    if (
                        ReportRequests.Any(
                            r =>
                            r.employeeid == request.employeeid && r.report.reportid == request.report.reportid
                            && r.accountid == request.accountid && r.SubAccountId == request.SubAccountId
                            && r.exporttype == request.exporttype && r.SchedulerRequestID == request.SchedulerRequestID
                            && r.Status != ReportRequestStatus.Complete && r.Status != ReportRequestStatus.Failed
                            && r.report.criteria.Count == request.report.criteria.Count
                            && r.report.columns.Count == request.report.columns.Count))
                    {
                        ListInUse = false;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "requestExists");
            }
            finally
            {
                ListInUse = false;
            }

            return false;
        }

        #endregion
    }
}
