using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Timers;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;

namespace Expenses_Scheduler
{
    using System.ComponentModel;

    using BusinessLogic.Modules;

    using ConsoleBootstrap;

    using Container = SimpleInjector.Container;

    /// <summary>
    /// Expenses Scheduler
    /// </summary>
    public partial class Expenses_Scheduler : ServiceBase
    {
        /// <summary>
        /// Timer interval
        /// </summary>
        double _nTimerInterval;
        /// <summary>
        /// Timer process
        /// </summary>
        readonly System.Timers.Timer _tmr = new System.Timers.Timer();

        /// <summary>
        /// 
        /// </summary>
        readonly System.Timers.Timer _tmrEmailSender = new System.Timers.Timer();
        /// <summary>
        /// Number of threads
        /// </summary>
        private static int _nNumberOfThreads;
        /// <summary>
        /// Thread polling interval
        /// </summary>
        private static int _nThreadPollInterval;
        /// <summary>
        /// Collection of scheduled reports
        /// </summary>
        private static readonly List<cScheduleRequest> lstScheduleRequests = new List<cScheduleRequest>();
        /// <summary>
        /// Collection of report threads
        /// </summary>
        private static readonly SortedList<string, Thread> lstReportThreads = new SortedList<string, Thread>();
        /// <summary>
        /// Schedules timer
        /// </summary>
        private System.Threading.Timer _tmrSchedules;
        /// <summary>
        /// 
        /// </summary>
        private readonly TimerCallback _timeCb = ProcessSchedules;
        private static bool _bListInUse;

        private static bool _enableLogging;

        //private System.Threading.Timer tmrEmails;

        public Expenses_Scheduler()
        {
            InitializeComponent();
        }

        #region properties
        public static int NumberOfThreads
        {
            get { return _nNumberOfThreads; }
        }
        public int ThreadPollInterval
        {
            get { return _nThreadPollInterval; }
        }
        public static List<cScheduleRequest> ScheduleRequests
        {
            get { return lstScheduleRequests; }
        }
        public static SortedList<string, Thread> ReportThreads
        {
            get { return lstReportThreads; }
        }
        public static bool ListInUse
        {
            get { return _bListInUse; }
            set { _bListInUse = value; }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            var clsEmails = new cEmails("127.0.0.1");
            clsEmails.SendOnStopEmail();

            _tmr.Dispose();
            _tmrSchedules.Dispose();
            _tmrEmailSender.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            GlobalVariables.DefaultModule = (Modules)int.Parse(ConfigurationManager.AppSettings["defaultModule"]);
            GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ToString();

            //Bootstrap scheduler as it use SM and SMLib
            Container container = Bootstrapper.Bootstrap(null);

            //Assign container to funky injector
            FunkyInjector.Container = container;

            try
            {
                _enableLogging = Convert.ToBoolean(ConfigurationManager.AppSettings["enableLogging"]);
            }
            catch (Exception e)
            {
                _enableLogging = false;
            }

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                // changed to using an encrypted password in the web/app/exe config
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString =
                    new SqlConnectionStringBuilder(GlobalVariables.MetabaseConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
                SqlDependency.Start(sConnectionString.ToString());
            }

            int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            ChannelServices.RegisterChannel(new TcpChannel(port), false);
            WellKnownServiceTypeEntry wkste = new WellKnownServiceTypeEntry(
                typeof(cSchedulerSvc),
                "scheduler.rem",
                WellKnownObjectMode.SingleCall);
            RemotingConfiguration.ApplicationName = "ExpensesScheduler";
            RemotingConfiguration.RegisterWellKnownServiceType(wkste);

            _nTimerInterval = Convert.ToDouble(ConfigurationManager.AppSettings["TimerSetting"]);
            _tmr.Interval = _nTimerInterval;
            _tmr.Elapsed += TmrElapsed;
            _tmr.Enabled = true;

            _tmrEmailSender.Enabled = false;

            _nNumberOfThreads = ConfigurationManager.AppSettings["NumberOfThreads"] != null
                                    ? Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"])
                                    : 5;

            _nThreadPollInterval = ConfigurationManager.AppSettings["ThreadPollInterval"] != null
                                       ? Convert.ToInt32(ConfigurationManager.AppSettings["ThreadPollInterval"])
                                       : 1000;

            _tmrSchedules = new System.Threading.Timer(_timeCb, null, _nThreadPollInterval, _nThreadPollInterval);

            bool accountsCached = new cAccounts().CacheList(false);

            if (!accountsCached)
            {
                throw new InvalidOperationException("The Accounts could not be cached correctly.");
            }

            HostManager.SetHostInformation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TmrElapsed(object sender, ElapsedEventArgs e)
        {
            //Bootstrap scheduler as it use SM and SMLib
            Container container = Bootstrapper.Bootstrap(null);

            //Assign container to funky injector
            FunkyInjector.Container = container;

            //check for potential schedules

            DateTime startDate = new DateTime(1900, 01, 01, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            double intervalMinutes = (_nTimerInterval / 1000) / 60;
            DateTime endDate = startDate.AddMinutes(intervalMinutes);
            DateTime runDate = DateTime.Now.AddMinutes(0 - intervalMinutes);

            DiagLog(string.Format("Scheduler : TmrElapsed called.\n@startdate = {0}, @endDate = {1}, @rundate = {2}", startDate.ToString(), endDate.ToString(), runDate.ToString()));

            string[] validHostnameIds = ConfigurationManager.AppSettings["ValidHostnameIDS"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (cAccount account in cAccounts.CachedAccounts.Values)
            {
                if (account.archived)
                {
                    continue;
                }

                var scheduleUser = cMisc.GetCurrentUser(account.accountid + ",0", true);

                DiagLog(string.Format("Scheduler : TmrElapsed : Checking schedules for {0} ({1})", account.companyid, account.accountid));

                bool allowToContinue = validHostnameIds.Any(t => Convert.ToInt32(t) == account.GetHostnameID(GlobalVariables.DefaultModule));

                if (!allowToContinue)
                {
                    DiagLog(string.Format("Scheduler : TmrElapsed : AccountID {0} on hostnameID {1} is not in the valid host name list. Skipping account", account.accountid, account.GetHostnameID(GlobalVariables.DefaultModule)));
                    continue;
                }

                //Bootstrap scheduler as it use SM and SMLib
                container = Bootstrapper.Bootstrap(account.accountid);

                //Assign container to funky injector
                FunkyInjector.Container = container;

                #region scheduled reports

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSchedules"]))
                {
                    try
                    {
                        // RECORD STARTING CHECK ON X DATABASE
                        //cAccounts.LogEvent("Started Checking " + account.companyid + " Database");

                        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(account.accountid));

                        string strsql = "select scheduleid from scheduled_reports where ((getDate() between startdate and enddate) or (getdate() >= startdate and enddate is null)) and (starttime between @startdate and @enddate) and scheduleid not in (select scheduleid from scheduled_reports_log where (datestamp between @startdate and @enddate))";
                        expdata.sqlexecute.Parameters.AddWithValue("@startdate", startDate);
                        expdata.sqlexecute.Parameters.AddWithValue("@enddate", endDate);

                        int scheduleid;
                        SortedList schedules = new SortedList();

                        using (SqlDataReader reader = expdata.GetReader(strsql))
                        {
                            expdata.sqlexecute.Parameters.Clear();

                            while (reader.Read())
                            {
                                scheduleid = reader.GetInt32(0);
                                schedules.Add(scheduleid, account.accountid);
                            }
                            reader.Close();
                        }

                        DiagLog(string.Format("Scheduler : TmrElapsed : Schedules to process for {0} : {1}", account.companyid, schedules.Count));

                        for (int i = 0; i < schedules.Count; i++)
                        {
                            scheduleid = (int)schedules.GetKey(i);

                            DiagLog(string.Format("Scheduler : TmrElapsed : Found schedule with ID {0} for company {1} ({2})", scheduleid, account.companyid, account.accountid));

                            cScheduledReports clsreports = new cScheduledReports(account.accountid);
                            if (!clsreports.IsReadyForProcess(scheduleid))
                            {
                                DiagLog(string.Format("Scheduler : TmrElapsed : Schedule {0} not ready for processing. Skipping...", scheduleid));
                                continue;
                            }

                            strsql = "SELECT count(scheduleid) FROM scheduled_reports_log WHERE scheduleid = @scheduleid AND datestamp > @rundate";
                            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
                            expdata.sqlexecute.Parameters.AddWithValue("@rundate", runDate);
                            int reccount = expdata.getcount(strsql);

                            if (reccount == 0)
                            {
                                cScheduledReport rpt = clsreports.GetScheduledReportById(scheduleid);

                                DiagLog(string.Format("Scheduler : TmrElapsed : Adding schedule ID {0} for company {1} ({2}) to report queue.", scheduleid, account.companyid, account.accountid));

                                lock (((ICollection)Expenses_Scheduler.ScheduleRequests).SyncRoot)
                                {
                                    ListInUse = true;
                                    try
                                    {
                                        lstScheduleRequests.Add(new cScheduleRequest(account.accountid, scheduleid, rpt.employeeid, FunkyInjector.Container));
                                    }
                                    catch (Exception ex)
                                    {
                                        cEventlog.LogEntry(string.Format("Scheduler : TmrElapsed : Error adding schedule to queue : {0}\n{1}", ex.Message, ex.StackTrace));
                                    }
                                    finally
                                    {
                                        ListInUse = false;
                                    }
                                }

                                //clsreports.runSchedule(scheduleid);
                            }
                            else
                            {
                                DiagLog(string.Format("Scheduler : TmrElapsed : Schedule {0} ran within time frame already (found in schedule_reports_log), skipping...", scheduleid));
                            }
                            expdata.sqlexecute.Parameters.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("Scheduler : TmrElapsed : Error checking schedules for company " + account.companyid);
                        cEventlog.LogEntry("Scheduler : TmrElapsed : Error message is " + ex.Message + " " + ex.StackTrace);
                    }
                }

                #endregion scheduled reports

                #region automated checks

             
                    try
                    {
                        if (account.IsNHSCustomer)
                        {
                           DiagLog(string.Format("Scheduler : TmrElapsed : Starting automated ESR checks for account {0} ({1})", account.companyid, account.accountid));
                           DoAutomatedChecks(account);
                        }
                    }
                    catch
                    {
                        cEventlog.LogEntry(string.Format("Scheduler : TmrElapsed : Failed doAutomatedChecks for {0} ({1})", account.companyid, account.accountid));
                    }
                

                #endregion

                #region password keys

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnablePasswordKeys"]))
                {
                    try
                    {
                        DBConnection smData = new DBConnection(cAccounts.getConnectionString(account.accountid));

                        smData.sqlexecute.Parameters.AddWithValue("@today", DateTime.Now);
                        const string strSQL = "SELECT employeeID FROM employeePasswordKeys WHERE sendOnDate = @today";
                        List<int> employeeIds = new List<int>();

                        using (SqlDataReader reader = smData.GetReader(strSQL))
                        {
                            cEmployees clsEmployees = new cEmployees(account.accountid);
                            cAccountSubAccounts subaccs = new cAccountSubAccounts(account.accountid);
                            cAccountProperties properties = subaccs.getSubAccountById(subaccs.getFirstSubAccount().SubAccountID).SubAccountProperties;
   
                            while (reader.Read())
                            {
                                int employeeID = reader.GetInt32(0);

                                DiagLog(string.Format("Scheduler : TmrElapsed : Calling SendPasswordKey and SendWelcomeEmail methods for employeeID {0}", employeeID));
                            
                                clsEmployees.SendPasswordKey(employeeID, cEmployees.PasswordKeyType.Imported, null, Modules.Expenses);
                                clsEmployees.SendWelcomeEmail(properties.MainAdministrator, employeeID, scheduleUser); 
                                employeeIds.Add(employeeID);
                            }

                            reader.Close();
                        }

                        DiagLog("Scheduler : TmrElapsed : Updating employeePasswordKeys.sendOnDate = NULL for employees");

                        foreach (int employeeId in employeeIds)
                        {
                            smData.sqlexecute.Parameters.Clear();
                            smData.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                            smData.ExecuteSQL("UPDATE employeePasswordKeys SET sendOnDate = NULL WHERE employeeID = @employeeId");
                        }
                    }
                    catch
                    {
                        cEventlog.LogEntry(string.Format("Scheduler : TmrElapsed : Failed checking password keys on {0} ({1})", account.companyid, account.accountid));
                    }
                }

                #endregion password keys

                #region tasks

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableTaskNotifications"]))
                {
                    DiagLog(string.Format("Scheduler : TmrElapsed : Checking task notifications for {0} ({1})", account.companyid, account.accountid));

                    cSchedulerTasks sTasks = new cSchedulerTasks(account.accountid);
                    sTasks.DoTasks();
                }

                #endregion

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        static void ProcessSchedules(object state)
        {
            try
            {
                if (ListInUse)
                {
                    return;
                }

                FreeSchedulesAndThreads();

                if (ScheduleRequests.Count == 0 || ReportThreads.Count >= NumberOfThreads)
                {
                    ListInUse = false;
                    return;
                }

                int availablethreads = NumberOfThreads - ReportThreads.Count;
                DiagLog(string.Format("Scheduler : ProcessSchedules : Available threads for running scheduled reports = {0}", availablethreads));
                DiagLog(string.Format("Scheduler : ProcessSchedules : Currently {0} entries in lstScheduledRequests and current ReportThreads count = {1}", lstScheduleRequests.Count, ReportThreads.Count));

                lock (((ICollection)ScheduleRequests).SyncRoot)
                {
                    try
                    {
                        ListInUse = true;

                        int newThreads = 0;
                        foreach (cScheduleRequest sched in lstScheduleRequests)
                        {
                            if (newThreads >= availablethreads)
                            {
                                DiagLog(string.Format("Scheduler : ProcessSchedules : No available threads. Current Threads = {0}, Max Threads = {1}. Waiting...", newThreads, availablethreads));
                                break;
                            }

                            if (sched.Status != ReportRequestStatus.Queued)
                            {
                                DiagLog(string.Format("Scheduler : ProcessSchedules : AccountId {0} : Schedule {1} Status is {2}", sched.AccountID, sched.ScheduleID, sched.Status.ToString()));
                                continue;
                            }

                            sched.Status = ReportRequestStatus.BeingProcessed;

                            DiagLog(string.Format("Scheduler : ProcessSchedules : Running schedule in thread for AccountId {0} : Schedule {1} : EmployeeId {2}", sched.AccountID, sched.ScheduleID, sched.EmployeeID));

                            Thread thread = new Thread(StartScheduleThread) { IsBackground = true, Name = GetThreadName(sched) };
                            ReportThreads.Add(thread.Name, thread);
                            thread.Start(sched);
                            newThreads++;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry(string.Format("Scheduler : ProcessSchedules : Error processing a schedule : {0}\n{1}", ex.Message, ex.StackTrace));
                    }
                    finally
                    {
                        ListInUse = false;
                    }
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(string.Format("Scheduler : ProcessSchedules : Error: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Removes completed or failed reports from the ReportTheads and Scheduled Reports Queue collections
        /// </summary>
        private static void FreeSchedulesAndThreads()
        {
            try
            {
                if (ListInUse)
                {
                    return;
                }

                lock (((ICollection)ScheduleRequests).SyncRoot)
                {
                    ListInUse = true;

                    List<string> completedThreadNames = (from x in lstScheduleRequests where x.Status == ReportRequestStatus.Complete || x.Status == ReportRequestStatus.Failed select GetThreadName(x)).ToList();

                    if (completedThreadNames.Count > 0)
                    {
                        DiagLog(string.Format("Scheduler : FreeSchedulesAndThreads : Number of threads released = {0}", completedThreadNames.Count));

                        foreach (var threadName in completedThreadNames)
                        {
                            if (ReportThreads.ContainsKey(threadName))
                            {
                                ReportThreads.Remove(threadName);
                            }
                        }
                    }

                    lstScheduleRequests.RemoveAll(x => x.Status == ReportRequestStatus.Complete || x.Status == ReportRequestStatus.Failed);
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(string.Format("Scheduler : FreeSchedulesAndThreads : Error {0}\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                ListInUse = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void StartScheduleThread(object data)
        {
            cScheduleRequest request = (cScheduleRequest)data;

            FunkyInjector.Container = request.Container;

            try
            {
                cScheduledReports clsscheduled = new cScheduledReports(request.AccountID);

                clsscheduled.RunSchedule(ref request);
            }
            catch (Exception ex)
            {
                DiagLog(string.Format("Scheduler : StartScheduleThread : Error occurred running schedule {0} : Error = {1}\n{2}", request.ScheduleID, ex.Message, ex.StackTrace));
                cEventlog.LogEntry(string.Format("Scheduler : StartScheduleThread : Error occurred running schedule {0} : Error = {1}", request.ScheduleID, ex.Message), true, EventLogEntryType.Error, cEventlog.ErrorCode.DebugInformation);
                request.Status = ReportRequestStatus.Failed;
            }
            finally
            {
                DiagLog(string.Format("Scheduler : StartScheduleThread : AccountId {0} : Schedule {1} : Schedule Thread finished", request.AccountID, request.ScheduleID));

                FreeSchedulesAndThreads();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleRequest"></param>
        /// <returns></returns>
        private static string GetThreadName(cScheduleRequest scheduleRequest)
        {
            return scheduleRequest.AccountID + "_" + scheduleRequest.ScheduleID + "_" + scheduleRequest.EmployeeID;
        }

        /// <summary>
        /// Do checks for the ESR automated imports
        /// </summary>
        /// <param name="account"></param>
        private static void DoAutomatedChecks(cAccount account)
        {
            var esrChecks = new cESRRoutines(account);
            bool notifyAboutActivation = false;
            bool notifyAboutArchive = false;
            bool notifyAboutAssignments = false;
            bool notifyAboutCarActivation = false;
               
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableESRPersonChecks"]))
            {
                try
                {
                    DiagLog(string.Format("Calling ESR ActivationCheck for accountId {0}", account.accountid));
                    notifyAboutActivation = esrChecks.ActivationCheck();
                }
                catch (Exception exActivate)
                {
                    cEventlog.LogEntry("esrChecks.ActivationCheck() failed.\n\nMessage: " + exActivate.Message + "\n\nStackTrace: " + exActivate.StackTrace);
                }

                try
                {
                    DiagLog(string.Format("Calling ESR ArchiveCheck for accountId {0}", account.accountid));
                    notifyAboutArchive = esrChecks.ArchiveCheck();
                }
                catch (Exception exArchive)
                {
                    cEventlog.LogEntry("esrChecks.ArchiveCheck() failed.\n\nMessage: " + exArchive.Message + "\n\nStackTrace: " + exArchive.StackTrace);
                }
            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableESRAssignmentChecks"]))
            {
                try
                {
                    DiagLog(string.Format("Calling ESR AssignmentCheck for accountId {0}", account.accountid));
                    notifyAboutAssignments = esrChecks.AssignmentCheck();
                }
                catch (Exception exAssignment)
                {
                    cEventlog.LogEntry("esrChecks.AssignmentCheck() failed.\n\nMessage: " + exAssignment.Message + "\n\nStackTrace: " + exAssignment.StackTrace);
                }

            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableESRVehicleChecks"]))
            {
                try
                {
                    DiagLog(string.Format("Calling ESR CarActivationCheck for accountId {0}.", account.accountid));
                    notifyAboutCarActivation = esrChecks.CarActivationCheck();
                }
                catch (Exception exCarActivate)
                {
                    cEventlog.LogEntry("esrChecks.CarActivationCheck() failed.\n\nMessage: " + exCarActivate.Message + "\n\nStackTrace: " + exCarActivate.StackTrace);
                }
            }

            try
            {
                if (notifyAboutActivation || notifyAboutArchive || notifyAboutAssignments || notifyAboutCarActivation)
                {
                    var notifications = new Notifications(account.accountid);
                    var templates = new NotificationTemplates(account.accountid, 0, string.Empty, 0, GlobalVariables.DefaultModule);

                    notifications.SendNotifications(EmailNotificationType.ESRSummaryNotification, "", templates);
                }
            }
                catch (Exception exNotification)
                {
                    cEventlog.LogEntry("notifications.SendNotifications() failed.\n\nMessage: " + exNotification.Message + "\n\nStackTrace: " + exNotification.StackTrace);
                }
        }

        /// <summary>
        /// Outputs a message to the EventLog but only if enableLogging is set to "true"
        /// </summary>
        /// <param name="msg"></param>
        public static void DiagLog(string msg)
        {
            if (!_enableLogging) return;

            cEventlog.LogEntry(msg);
        }
    }

    #region cScheduleRequest
    public class cScheduleRequest
    {
        private readonly int _nAccountID;
        private readonly int _nScheduleID;
        private readonly int _nEmployeeID;
        private ReportRequestStatus _eStatus;
        private cReportRequest _clsReportRequest;
        private byte[] _oReportData;

        public Container Container { get; set; }

        public cScheduleRequest(int accountid, int scheduleid, int employeeid, Container container)
        {
            _nAccountID = accountid;
            _nScheduleID = scheduleid;
            _nEmployeeID = employeeid;
            this.Container = container;
            _eStatus = ReportRequestStatus.Queued;
        }

        #region properties
        public int AccountID
        {
            get { return _nAccountID; }
        }
        public int ScheduleID
        {
            get { return _nScheduleID; }
        }
        public ReportRequestStatus Status
        {
            get { return _eStatus; }
            set { _eStatus = value; }
        }
        public cReportRequest ReportRequest
        {
            get { return _clsReportRequest; }
            set
            {
                _clsReportRequest = value;
            }
        }
        public byte[] ReportData
        {
            get { return _oReportData; }
            set { _oReportData = value; }
        }
        public int EmployeeID
        {
            get { return _nEmployeeID; }
        }
        #endregion
    }
    #endregion
}
