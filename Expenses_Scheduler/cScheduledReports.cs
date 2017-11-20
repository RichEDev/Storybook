using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;
using SpendManagementLibrary;
using Spend_Management;

namespace Expenses_Scheduler
{
    using System.Diagnostics;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;

    public class cScheduledReports
    {
        private readonly int _nAccountid;
        
        private static SortedList accountlist = new SortedList();
        SortedList<int, cScheduledReport> _list;
        
        private static int _nThreadPollInterval;

        private Timer tmr;
        private TimerCallback timeCB = new TimerCallback(ProcessReports);

        private SortedList<int, cScheduledReport> ScheduledReportsList
        {
            get
            {
                if (_list == null)
                {
                    InitialiseData();
                }

                return _list;
            }
        }

        public cScheduledReports(int accountid)
        {
            _nAccountid = accountid;
            
            _nThreadPollInterval = ConfigurationManager.AppSettings["ThreadPollInterval"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["ThreadPollInterval"]) : 1000;
            tmr = new Timer(timeCB, null, 0, ThreadPollInterval);
        }

        #region properties
        public int Accountid
        {
            get { return _nAccountid; }
        }
        public int ThreadPollInterval
        {
            get { return _nThreadPollInterval; }
        }
        #endregion

        #region data initialisation
        private void InitialiseData()
        {
            _list = CacheList();
        }

        private SortedList<int, cScheduledReport> CacheList()
        {
            cAccounts clsAccount = new cAccounts();
            cAccount account = clsAccount.GetAccountByID(Accountid);

            string[] validHostnameIDS = ConfigurationManager.AppSettings["ValidHostnameIDS"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            bool allowToContinue = false;
            for (var i = 0; i < validHostnameIDS.Length; i++)
            {
                if (Convert.ToInt32(validHostnameIDS[i]) != account.GetHostnameID(GlobalVariables.DefaultModule)) continue;

                allowToContinue = true;
                break;
            }

            if (allowToContinue == false)
            {
                return new SortedList<int, cScheduledReport>();
            }

            DBConnection schedexpdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            cScheduledReport scheduledrep = null;
            SqlDataReader reader;

            var sList = new SortedList<int, cScheduledReport>();

            const string strsql = "SELECT scheduleid, reportid, employeeid, scheduletype, date, repeat_frequency, week, calendar_days, startdate, enddate, starttime, outputtype, deliverymethod, emailaddresses, ftpserver, ftpusername, ftppassword, ftpusessl, financialexportid, emailbody FROM dbo.scheduled_reports";
            schedexpdata.sqlexecute.CommandText = strsql;

			using (reader = schedexpdata.GetReader(strsql))
			{
				while (reader.Read())
				{
					int scheduleid = reader.GetInt32(reader.GetOrdinal("scheduleid"));
					Guid reportid = reader.GetGuid(reader.GetOrdinal("reportid"));

				    int financialexportid = reader.IsDBNull(reader.GetOrdinal("financialexportid")) ? 0 : reader.GetInt32(reader.GetOrdinal("financialexportid"));
					int employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
					ScheduleType scheduletype = (ScheduleType)reader.GetByte(reader.GetOrdinal("scheduletype"));
					ExportType outputtype = (ExportType)reader.GetByte(reader.GetOrdinal("outputtype"));
					DeliveryType deliverymethod = (DeliveryType)reader.GetByte(reader.GetOrdinal("deliverymethod"));
					DateTime startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
					DateTime starttime = reader.GetDateTime(reader.GetOrdinal("starttime"));
				    DateTime enddate = reader.IsDBNull(reader.GetOrdinal("enddate")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("enddate"));
				    byte repeatfrequency = reader.IsDBNull(reader.GetOrdinal("repeat_frequency")) ? (byte)0 : reader.GetByte(reader.GetOrdinal("repeat_frequency"));
				    string emailaddresses = reader.IsDBNull(reader.GetOrdinal("emailaddresses")) ? "" : reader.GetString(reader.GetOrdinal("emailaddresses"));
				    string ftpserver = reader.IsDBNull(reader.GetOrdinal("ftpserver")) ? "" : reader.GetString(reader.GetOrdinal("ftpserver"));
				    string ftpusername = reader.IsDBNull(reader.GetOrdinal("ftpusername")) ? "" : reader.GetString(reader.GetOrdinal("ftpusername"));
				    string ftppassword = reader.IsDBNull(reader.GetOrdinal("ftppassword")) ? "" : reader.GetString(reader.GetOrdinal("ftppassword"));
                    bool ftpusessl = reader.GetBoolean(reader.GetOrdinal("ftpusessl"));
				    string emailbody = reader.IsDBNull(reader.GetOrdinal("emailbody")) ? "" : reader.GetString(reader.GetOrdinal("emailbody"));

					switch (scheduletype)
					{
						case ScheduleType.Day:
                            scheduledrep = new cDailyScheduledReport(Accountid, scheduleid, reportid, financialexportid, employeeid, ScheduleType.Day, outputtype, deliverymethod, startdate, enddate, starttime, GetDaysOfWeek(scheduleid), repeatfrequency, GetColumns(scheduleid), GetCriteria(scheduleid), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
							break;
						case ScheduleType.Week:
							scheduledrep = new cWeeklyScheduledReport(Accountid,scheduleid, reportid, financialexportid, employeeid, ScheduleType.Week, outputtype, deliverymethod, startdate, enddate, starttime, GetDaysOfWeek(scheduleid), repeatfrequency, GetColumns(scheduleid), GetCriteria(scheduleid), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
							break;
						case ScheduleType.Month:
							byte week = reader.IsDBNull(reader.GetOrdinal("week")) ? (byte)0 : reader.GetByte(reader.GetOrdinal("week"));
					        string calendardays = reader.IsDBNull(reader.GetOrdinal("calendar_days")) ? "" : reader.GetString(reader.GetOrdinal("calendar_days"));
                            scheduledrep = new cMonthlyScheduledReport(Accountid, scheduleid, reportid, financialexportid, employeeid, ScheduleType.Month, outputtype, deliverymethod, startdate, enddate, starttime, GetMonthsOfYear(scheduleid), week, GetDaysOfWeek(scheduleid), calendardays, GetColumns(scheduleid), GetCriteria(scheduleid), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
							break;
						case ScheduleType.Once:
                            scheduledrep = new cScheduledReport(Accountid, scheduleid, reportid, financialexportid, employeeid, ScheduleType.Once, outputtype, deliverymethod, startdate, enddate, starttime, GetColumns(scheduleid), GetCriteria(scheduleid), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
							break;
					}
				    if(scheduledrep != null) sList.Add(scheduleid, scheduledrep);
				}
				reader.Close();
			}

            return sList;
        }

        private static ArrayList GetCalendarDays(string days)
        {
            string[] arrdays = days.Split(',');

            ArrayList calendardays = new ArrayList();

            for (int i = 0; i < arrdays.GetLength(0); i++)
            {
                if (arrdays[i].Contains("-")) //range
                {
                    int dashpos = arrdays[i].IndexOf("-");
                    byte startday = byte.Parse(arrdays[i].Substring(0, dashpos).Trim());
                    byte endday = byte.Parse(arrdays[i].Substring(dashpos+1, arrdays[i].Length - dashpos - 1).Trim());
                    if (startday < endday)
                    {
                        while (startday <= endday)
                        {
                            calendardays.Add(startday);
                            startday++;
                        }
                    }
                    else
                    {
                        while (startday >= endday)
                        {
                            calendardays.Add(startday);
                            startday--;
                        }
                    }
                }
                else
                {
                    calendardays.Add(byte.Parse(arrdays[i].Trim()));
                }
            }

            return calendardays;
        }

        private ArrayList GetDaysOfWeek(int scheduleid)
        {
            ArrayList days = new ArrayList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            SqlDataReader reader;
            const string strsql = "select * from scheduled_days where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
			using (reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();
				while (reader.Read())
				{
					days.Add((DayOfWeek)reader.GetByte(reader.GetOrdinal("day")));
				}
				reader.Close();
			}
            return days;
        }

        private ArrayList GetMonthsOfYear(int scheduleid)
        {
            ArrayList months = new ArrayList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            SqlDataReader reader;
            const string strsql = "select * from scheduled_months where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
			using (reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();
				while (reader.Read())
				{
					months.Add(reader.GetByte(reader.GetOrdinal("month")));
				}
				reader.Close();
			}
            return months;
        }

        private SortedList GetColumns(int scheduleid)
        {
            SortedList columns = new SortedList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            SqlDataReader reader;
            
            const string strsql = "select * from scheduled_reports_columns where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
			using (reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();

				while (reader.Read())
				{
					Guid reportcolumnid = reader.GetGuid(reader.GetOrdinal("reportcolumnid"));
					string literalvalue = reader.GetString(reader.GetOrdinal("literalvalue"));
					columns.Add(reportcolumnid, literalvalue);
				}
				reader.Close();
			}
            return columns;
        }

        public SortedList GetCriteria(int scheduleid)
        {
            SortedList criteria = new SortedList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            SqlDataReader reader;
            const string strsql = "select criteriaid, value1, value2 from scheduled_reports_criteria where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
			using (reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();
				while (reader.Read())
				{
					Guid criteriaid = reader.GetGuid(reader.GetOrdinal("criteriaid"));
					object[] value1 = new object[1];
					value1[0] = reader.GetString(reader.GetOrdinal("value1"));
				    object[] value2 = null;
				    if (reader.IsDBNull(reader.GetOrdinal("value2")) == false)
					{
						value2 = new object[1];
						value2[0] = reader.GetString(reader.GetOrdinal("value2"));
					}
					object[] values = new object[2];
					values[0] = value1;
					values[1] = value2;
					criteria.Add(criteriaid, values);
				}
				reader.Close();
			}
            return criteria;
        }
        #endregion

        public int AddSchedule(cScheduledReport report)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            int scheduleid;
            cDailyScheduledReport daily;
            cWeeklyScheduledReport weekly;
            cMonthlyScheduledReport monthly;

            const string strsql = "insert into scheduled_reports (reportid, financialexportid, employeeid, scheduletype, [date], repeat_frequency, week, calendar_days, startdate, enddate, starttime, outputtype, deliverymethod, emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl) " +
                                  "values (@reportid, @financialexportid, @employeeid, @scheduletype, @date, @repeatfrequency, @week, @calendardays, @startdate, @enddate, @starttime, @outputtype, @deliverymethod, @emailaddresses, @ftpserver, @ftpusername, @ftppassword,@emailbody,@ftpusessl);select @identity = @@identity";
            
            expdata.sqlexecute.Parameters.AddWithValue("@reportid", report.reportid);

            if (report.financialexportid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", report.financialexportid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", report.employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@scheduletype", (byte)report.scheduletype);
            expdata.sqlexecute.Parameters.AddWithValue("@startdate", report.startdate);
            if (report.enddate == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", report.enddate);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@starttime", report.starttime);
            expdata.sqlexecute.Parameters.AddWithValue("@outputtype", (byte)report.outputtype);
            expdata.sqlexecute.Parameters.AddWithValue("@deliverymethod", (byte)report.deliverymethod);
            expdata.sqlexecute.Parameters.AddWithValue("@emailaddresses", report.emailaddresses);
            expdata.sqlexecute.Parameters.AddWithValue("@ftpserver", report.ftpserver);
            expdata.sqlexecute.Parameters.AddWithValue("@ftpusername", report.ftpusername);
            expdata.sqlexecute.Parameters.AddWithValue("@ftppassword", report.ftppassword);
            expdata.sqlexecute.Parameters.AddWithValue("@emailbody", report.emailbody);
            expdata.sqlexecute.Parameters.AddWithValue("@ftpusessl", report.ftpusessl);
            switch (report.scheduletype)
            {
                case ScheduleType.Day:
                    daily = (cDailyScheduledReport)report;
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", daily.repeatfrequency);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", DBNull.Value);

                    break;
                case ScheduleType.Week:
                    weekly = (cWeeklyScheduledReport)report;
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", weekly.repeatfrequency);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", DBNull.Value);
                    break;
                case ScheduleType.Month:
                    monthly = (cMonthlyScheduledReport)report;
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", monthly.week);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", monthly.calendardays);
                    break;
                case ScheduleType.Once:
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", DBNull.Value);
                    break;
            }

            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;

            try
            {
                expdata.ExecuteSQL(strsql);
                scheduleid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();

                report.setScheduleid(scheduleid);
                switch (report.scheduletype)
                {
                    case ScheduleType.Day:
                        daily = (cDailyScheduledReport)report;
                        InsertDays(scheduleid, daily.daysofweek);
                        break;
                    case ScheduleType.Week:
                        weekly = (cWeeklyScheduledReport)report;
                        InsertDays(scheduleid, weekly.daysofweek);
                        break;
                    case ScheduleType.Month:
                        monthly = (cMonthlyScheduledReport)report;
                        InsertMonths(scheduleid, monthly.months);
                        InsertDays(scheduleid, monthly.daysofweek);
                        break;
                }

                AddColumns(scheduleid, report.columns);
                AddCriteria(scheduleid, report.criteria);
            }
            catch (Exception ex)
            {
                cScheduleRequest tmpRequest = new cScheduleRequest(report.accountid, report.scheduleid, report.employeeid);
                SendError(tmpRequest, errMsg: ex.Message, errStackDump: ex.StackTrace, msgBody: "An error occurred attempting to add a scheduled report");
                scheduleid = 0;
            }

            // list if nullified due to dependency onChange() event
            //list.Add(scheduleid, report);

            return scheduleid;
        }

        public int UpdateSchedule(cScheduledReport report)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            cDailyScheduledReport daily;
            cWeeklyScheduledReport weekly;
            cMonthlyScheduledReport monthly;

            const string strsql = "update scheduled_reports set scheduletype = @scheduletype, [date] = @date, repeat_frequency = @repeatfrequency, week = @week, calendar_days = @calendardays, startdate = @startdate, enddate = @enddate, starttime = @starttime, outputtype = @outputtype, deliverymethod = @deliverymethod, emailaddresses = @emailaddresses, ftpserver = @ftpserver, ftpusername = @ftpusername, ftppassword = @ftppassword, emailbody = @emailbody, ftpusessl = @ftpusessl where scheduleid = @scheduleid";

            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", report.scheduleid);
            expdata.sqlexecute.Parameters.AddWithValue("@scheduletype", (byte)report.scheduletype);
            expdata.sqlexecute.Parameters.AddWithValue("@startdate", report.startdate);
            if (report.enddate == new DateTime(1900, 01, 01))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", report.enddate);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@starttime", report.starttime);
            expdata.sqlexecute.Parameters.AddWithValue("@outputtype", (byte)report.outputtype);
            expdata.sqlexecute.Parameters.AddWithValue("@deliverymethod", (byte)report.deliverymethod);
            expdata.sqlexecute.Parameters.AddWithValue("@emailaddresses", report.emailaddresses);
            expdata.sqlexecute.Parameters.AddWithValue("@ftpserver", report.ftpserver);
            expdata.sqlexecute.Parameters.AddWithValue("@ftpusername", report.ftpusername);
            expdata.sqlexecute.Parameters.AddWithValue("@ftppassword", report.ftppassword);
            expdata.sqlexecute.Parameters.AddWithValue("@emailbody", report.emailbody);
            expdata.sqlexecute.Parameters.AddWithValue("@ftpusessl", report.ftpusessl);
            switch (report.scheduletype)
            {
                case ScheduleType.Day:
                    daily = (cDailyScheduledReport)report;
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", daily.repeatfrequency);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", DBNull.Value);

                    break;
                case ScheduleType.Week:
                    weekly = (cWeeklyScheduledReport)report;
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", weekly.repeatfrequency);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", DBNull.Value);
                    break;
                case ScheduleType.Month:
                    monthly = (cMonthlyScheduledReport)report;
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", monthly.week);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", monthly.calendardays);
                    break;
                case ScheduleType.Once:
                    expdata.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@repeatfrequency", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@week", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@calendardays", DBNull.Value);
                    break;
            }

            try
            {
                expdata.ExecuteSQL(strsql);

                expdata.sqlexecute.Parameters.Clear();

                ClearDays(report.scheduleid);
                ClearMonths(report.scheduleid);
                ClearColumns(report.scheduleid);
                ClearCriteria(report.scheduleid);

                switch (report.scheduletype)
                {
                    case ScheduleType.Day:
                        daily = (cDailyScheduledReport)report;
                        InsertDays(report.scheduleid, daily.daysofweek);
                        break;
                    case ScheduleType.Week:
                        weekly = (cWeeklyScheduledReport)report;
                        InsertDays(report.scheduleid, weekly.daysofweek);
                        break;
                    case ScheduleType.Month:
                        monthly = (cMonthlyScheduledReport)report;
                        InsertMonths(report.scheduleid, monthly.months);
                        InsertDays(report.scheduleid, monthly.daysofweek);
                        break;
                }

                AddColumns(report.scheduleid, report.columns);
                AddCriteria(report.scheduleid, report.criteria);
            }
            catch (Exception ex)
            {
                cScheduleRequest tmpRequest = new cScheduleRequest(report.accountid, report.scheduleid , report.employeeid);
                SendError(tmpRequest, errMsg: ex.Message, errStackDump: ex.StackTrace, msgBody: "An error occurred attempting to update a scheduled report");
            }
            // List variable is nullified due to dependency onChange() event call
            //if (list[report.scheduleid] != null)
            //{
            //    list[report.scheduleid] = report;
            //}

            return report.scheduleid;
        }

        public void DeleteSchedule(int scheduleid)
        {
            try
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
                const string strsql = "delete from scheduled_reports where scheduleid = @scheduleid";
                expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
                expdata.ExecuteSQL(strsql);
                ScheduledReportsList.Remove(scheduleid);
            }
            catch (Exception ex)
            {
                cScheduleRequest tmpRequest = new cScheduleRequest(Accountid, scheduleid, 0);

                SendError(tmpRequest, errMsg: ex.Message, errStackDump: ex.StackTrace, msgBody: "A problem occurred attempting to delete a schedule");
            }
        }

        private void InsertDays(int scheduleid, ArrayList days)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            foreach(object t in days)
            {
                const string strsql = "insert into scheduled_days (scheduleid, [day]) " +
                                      "values (@scheduleid, @day)";
                expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
                expdata.sqlexecute.Parameters.AddWithValue("@day", (byte)(DayOfWeek)t);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        private void ClearDays(int scheduleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            const string strsql = "delete from scheduled_days where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private void ClearMonths(int scheduleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            const string strsql = "delete from scheduled_months where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private void ClearColumns(int scheduleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            const string strsql = "delete from scheduled_reports_columns where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private void ClearCriteria(int scheduleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            const string strsql = "delete from scheduled_reports_criteria where scheduleid = @scheduleid";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private void InsertMonths(int scheduleid, ArrayList months)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            foreach(object t in months)
            {
                const string strsql = "insert into scheduled_months (scheduleid, [month]) " +
                                      "values (@scheduleid, @month)";
                expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
                expdata.sqlexecute.Parameters.AddWithValue("@month", (byte)t);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        private void AddColumns(int scheduleid, SortedList columns)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
           
            foreach (DictionaryEntry col in columns)
            {
                const string strsql = "insert into scheduled_reports_columns (scheduleid, reportcolumnid, literalvalue) " +
                                      "values (@scheduleid, @columnid, @literalvalue)";
                expdata.sqlexecute.Parameters.AddWithValue("@scheduleid",scheduleid);
                expdata.sqlexecute.Parameters.AddWithValue("@columnid",col.Key);
                expdata.sqlexecute.Parameters.AddWithValue("@literalvalue",col.Value);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        public void AddCriteria(int scheduleid, SortedList criteria)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));

            for (int i = 0; i < criteria.Count; i++)
            {
                Guid criteriaid = (Guid)criteria.GetKey(i);
                object[] values = (object[])criteria.GetByIndex(i);
                object[] value1 = (object[])values[0];
                object[] value2 = (object[])values[1];
                const string strsql = "insert into scheduled_reports_criteria (scheduleid, criteriaid, value1, value2) " +
                                      "values (@scheduleid, @criteriaid, @value1, @value2)";
                expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
                expdata.sqlexecute.Parameters.AddWithValue("@criteriaid", criteriaid);
                expdata.sqlexecute.Parameters.AddWithValue("@value1", value1[0]);
                expdata.sqlexecute.Parameters.AddWithValue("@value2", value2.Length == 0 ? DBNull.Value : value2[0]);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        public cScheduledReport GetScheduledReportById(int reportid)
        {
            return (cScheduledReport)ScheduledReportsList[reportid];
        }

        public bool IsReadyForProcess(int scheduleid)
        {
            cScheduledReport rpt = GetScheduledReportById(scheduleid);
            switch (rpt.scheduletype)
            {
                case ScheduleType.Day:
                    cDailyScheduledReport dailyrpt = (cDailyScheduledReport)rpt;
                    if (dailyrpt.daysofweek.Count > 0)
                    {
                        return ContainsToday(dailyrpt.daysofweek);
                    }
                    else
                    {
                        return IsRepeatDay(dailyrpt.startdate, dailyrpt.repeatfrequency);
                    }
                case ScheduleType.Week:
                    cWeeklyScheduledReport weeklyrpt = (cWeeklyScheduledReport)rpt;
                    if (IsRepeatWeek(weeklyrpt.startdate, weeklyrpt.repeatfrequency) && ContainsToday(weeklyrpt.daysofweek))
                    {
                        return true;
                    }
                    break;
                case ScheduleType.Month:
                    cMonthlyScheduledReport monthlyrpt = (cMonthlyScheduledReport)rpt;
                    if (ContainsMonth(monthlyrpt.months))
                    {
                        if (monthlyrpt.calendardays != "")
                        {
                            return ContainsCalendarDay(monthlyrpt.calendardays);
                        }
                        
                        if (ContainsToday(monthlyrpt.daysofweek))
                        {
                            return ContainsWeek(monthlyrpt.week);
                        }
                        return false;
                    }
                    break;
                case ScheduleType.Once:
                    return true;
            }
            return false;
        }

        private static bool ContainsToday(ArrayList days)
        {
            DayOfWeek today = DateTime.Today.DayOfWeek;
            return days.Contains(today);
        }

        public bool ContainsWeek(int weeknum)
        {
            int todayweeknum = DateTime.Today.Day / 7;
            int count = 0;
            int totaldays = 0;
            int daysinmonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            DateTime startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            
            while (startdate.DayOfWeek != DateTime.Today.DayOfWeek)
            {
                startdate = startdate.AddDays(1);
            }

            DateTime date = startdate;
            while (date <= DateTime.Today)
            {
                date = date.AddDays(7);
                count++;
            }

            if(weeknum != 5)
            {
                return weeknum == count;
            }

            date = startdate;
            DateTime enddate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, daysinmonth);
            while(date <= enddate)
            {
                totaldays++;
                date = date.AddDays(7);
            }
            return totaldays == count;
        }

        private static bool ContainsMonth(ArrayList months)
        {
            return months.Contains((byte)DateTime.Today.Month);
        }

        private static bool ContainsCalendarDay(string calendardays)
        {
            ArrayList days = GetCalendarDays(calendardays);
            return days.Contains((byte)DateTime.Today.Day);
        }
        
        private static bool IsRepeatWeek(DateTime startdate, byte repeatfrequency)
        {
            if (repeatfrequency <= 1)
            {
                return true;
            }
            TimeSpan ts = DateTime.Today.Subtract(startdate);

            double weeks = ts.Days / 7;

            weeks = Math.Truncate(weeks);
            if (weeks == 0 || Math.IEEERemainder(weeks, repeatfrequency) == 0)
            {
                return true;
            }
            return false;
        }

        private static bool IsRepeatDay(DateTime startdate, byte repeatfrequency)
        {
           
            TimeSpan ts = DateTime.Today.Subtract(startdate);
            if (ts.Days == 0)
            {
                return true;
            }
 
            if (Math.IEEERemainder(ts.Days, repeatfrequency) == 0)
            {
                return true;
            }
            return false;
        }

        public void RunSchedule(ref cScheduleRequest scheduleRequest)
        {
            int scheduleid = scheduleRequest.ScheduleID;
            cScheduledReport scheduledReport = GetScheduledReportById(scheduleid);

            if (scheduledReport == null)
            {
                return;
            }

            try
            {
                IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
                cExportOptions exportOptions = clsreports.getExportOptions(Accountid, scheduledReport.employeeid, scheduledReport.reportid);

                if (scheduledReport.financialexportid > 0)
                {
                    Expenses_Scheduler.DiagLog(string.Format("Running scheduled report for financialexportid {0}", scheduledReport.financialexportid));

                    byte application;
                    byte exportType;
                    int nhsTrustId;
                    bool preventNegativePayments;
                    bool expeditePaymentReport;
                    int exportNumber;
                    using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.Accountid)))
                    {
                        //                              0            1          2
                        string strsql = "SELECT applicationtype, exporttype, NHSTrustID, PreventNegativeAmountPayable,ExpeditePaymentReport FROM financial_exports WHERE financialexportid = @financialexportid";
                        application = 1;
                        exportType = 1;
                        nhsTrustId = 0;
                        expdata.sqlexecute.Parameters.AddWithValue("@financialexportid", scheduledReport.financialexportid);
                        preventNegativePayments = false;
                        expeditePaymentReport = false;
                        using (var reader = expdata.GetReader(strsql))
                        {
                            while (reader.Read())
                            {
                                application = reader.GetByte(0);

                                exportType = reader.IsDBNull(1) ? (byte)0 : reader.GetByte(1);
                                nhsTrustId = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                preventNegativePayments = !reader.IsDBNull(3) && reader.GetBoolean(3);
                                expeditePaymentReport = !reader.IsDBNull(4) && reader.GetBoolean(4);
                            }

                            reader.Close();
                        }

                        strsql = "select count(financialexportid) from exporthistory where financialexportid = @financialexportid";
                        exportNumber = expdata.ExecuteScalar<int>(strsql);
                        expdata.sqlexecute.Parameters.Clear();
                    }

                    exportOptions.financialexport = new cFinancialExport(scheduledReport.financialexportid, this.Accountid, (FinancialApplication)application, scheduledReport.reportid, false, 0, new DateTime(1900, 01, 01), exportNumber, new DateTime(2008, 12, 16, 14, 24, 51), exportType, nhsTrustId, preventNegativePayments, expeditePaymentReport);
                    exportOptions.isfinancialexport = true;
                    exportOptions.PreventNegativePayment = preventNegativePayments;
                    exportOptions.application = (FinancialApplication)application;
                }

                cReport reqreport = clsreports.getReportById(Accountid, scheduledReport.reportid);

                if (reqreport == null)
                {
                    // report doesn't exist, schedule is an orphan
                    Expenses_Scheduler.DiagLog(string.Format("Scheduler : RunSchedule : Report ID [{0}] not found for schedule ID {1}. Deleting schedule.", scheduledReport.reportid.ToString(), scheduleid));
                    this.DeleteSchedule(scheduleid);
                    AddAuditLogEntry(scheduledReport.accountid, scheduledReport.employeeid, string.Format("The report linked to the schedule with ID {0} could not be found. The schedule has been deleted.", scheduleid));
                    scheduleRequest.Status = ReportRequestStatus.Failed;
                    return;
                }

                // get access role level for the user running the report
                var subaccs = new cAccountSubAccounts(Accountid);
                CurrentUser scheduleUser = cMisc.GetCurrentUser(this.Accountid + "," + scheduledReport.employeeid, true);
                scheduleUser.CurrentSubAccountId = (reqreport.SubAccountID.HasValue ? reqreport.SubAccountID.Value : subaccs.getFirstSubAccount().SubAccountID);
                AccessRoleLevel roleLevel = scheduleUser.HighestAccessLevel;
                if (!reqreport.SubAccountID.HasValue) // requested report may be global, so inherits the schedule subaccount id
                {
                    reqreport.SubAccountID = scheduleUser.CurrentSubAccountId;
                }
                reqreport.exportoptions = exportOptions;
                var reportRequest = new cReportRequest(Accountid, reqreport.SubAccountID.Value, 1, reqreport, scheduledReport.outputtype, exportOptions, false, scheduledReport.employeeid, roleLevel);
                reportRequest.SchedulerRequestID = Guid.NewGuid(); // give report unique ID to identify it from potential of same report also scheduled again.
                scheduleRequest.ReportRequest = reportRequest; // store in case need to handle error as it is referenced

                if (roleLevel == AccessRoleLevel.SelectedRoles && cReport.canFilterByRole(reportRequest.report.basetable.TableID))
                {
                    // get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
                    var roles = new cAccessRoles(Accountid, cAccounts.getConnectionString(Accountid), true);
                    List<int> reportRoles = new List<int>();
                    // TODO - Need to double check this to see if it has the employee object populated.
                    List<int> lstAccessRoles = scheduleUser.Employee.GetAccessRoles().GetBy(reqreport.SubAccountID.Value);

                    foreach (int empArId in lstAccessRoles)
                    {
                        cAccessRole empRole = roles.GetAccessRoleByID(empArId);
                        foreach (int arId in empRole.AccessRoleLinks)
                        {
                            if (!reportRoles.Contains(arId))
                            {
                                reportRoles.Add(arId);
                            }
                        }
                    }
                    reportRequest.AccessLevelRoles = reportRoles.ToArray();
                }

                reportRequest.report.exportoptions = reportRequest.report.exportoptions;

                if (reqreport.hasRuntimeCriteria())
                {
                    foreach(object t in reqreport.criteria)
                    {
                        cReportCriterion criteria = (cReportCriterion)t;
                        if(!criteria.runtime) continue;

                        object[] values = (object[])scheduledReport.criteria[criteria.criteriaid];
                        if (values == null)
                        {
                            reportRequest.Status = ReportRequestStatus.Failed;
                            reportRequest.Exception = new Exception("No runtime criteria specified");
                            AddAuditLogEntry(scheduledReport.accountid, scheduledReport.employeeid, string.Format("Runtime criteria have not been defined for the schedule with ID {0} for the report {1}.", scheduleid, reqreport.reportname));
                            scheduleRequest.Status = ReportRequestStatus.Failed;
                            return;
                        }
                        criteria.updateValues((object[])values[0], (object[])values[1]);
                    }

                    SortedList<int, string> lstStaticColsToUpdate = new SortedList<int, string>();
                    //cReportColumn tmpReportColumn;
                    foreach (DictionaryEntry ent in scheduledReport.columns)
                    {
                        int j = 0;
                        foreach (cReportColumn col in reqreport.columns)
                        {
                            if (col.reportcolumnid == (Guid)(ent.Key))
                            {
                                lstStaticColsToUpdate.Add(j, (string)ent.Value);
                            }
                            j++;
                        }
                    }

                    foreach (KeyValuePair<int, string> kvp in lstStaticColsToUpdate)
                    {
                        ((cStaticColumn)reqreport.columns[kvp.Key]).setValue(kvp.Value);
                    }
                }

                cEventlog.LogEntry(
                    string.Format("Scheduler: cScheduledReports : RunSchedule : Calling create report for scheduleID {2}, reportid {0} with uniqueID {1}",
                        reportRequest.report.reportid.ToString(),
                        reportRequest.SchedulerRequestID.ToString(),
                        scheduleid));

                if (!clsreports.createReport(reportRequest))
                {
                    // returns false if report request already exists
                    scheduleRequest.Status = ReportRequestStatus.Failed;
                }
                scheduleRequest.ReportRequest = reportRequest;
            }
            catch (Exception ex)
            {
                scheduleRequest.Status = ReportRequestStatus.Failed;
                Expenses_Scheduler.DiagLog(string.Format("ScheduledReports : RunSchedule : Schedule - {0} : error - {1} : trace - " + ex.StackTrace, scheduledReport.scheduleid.ToString(), ex.Message, ex.StackTrace));
                cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : RunSchedule : Schedule - {0} : error - {1} : trace - " + ex.StackTrace, scheduledReport.scheduleid.ToString(), ex.Message, ex.StackTrace), false, EventLogEntryType.Error, cEventlog.ErrorCode.DebugInformation);
                SendError(scheduleRequest, errMsg: ex.Message, errStackDump: ex.StackTrace);
            }
        }

        public ArrayList GetEmailAddresses(string emailaddresses)
        {
            ArrayList addresses = new ArrayList();

            if (emailaddresses.IndexOf(";") != -1)
            {
                string[] arremails = emailaddresses.Split(';');
                foreach(string t in arremails)
                {
                    addresses.Add(t.Trim());
                }
            }
            else
            {
                addresses.Add(emailaddresses);
            }
            return addresses;
        }

        public string FtpReport(cScheduledReport rpt, byte[] data, string filename)
        {
            cAccounts clsAccount = new cAccounts();
            cAccount account = clsAccount.GetAccountByID(rpt.accountid);
            cSecureData clssecure = new cSecureData();
            string password = clssecure.Decrypt(rpt.ftppassword);
            cFTPClient ftpClient = new cFTPClient(rpt.ftpserver, rpt.ftpusername, password, useSSL: rpt.ftpusessl);

            string msg = "Schedule with Schedule ID " + rpt.scheduleid.ToString() + " (Filename: " + filename + ") **STATUS UNKNOWN** to the FTP site " + rpt.ftpserver + " for company " + account.companyid;

            if (ftpClient.UploadFile(filename, data) == FTP_Status.Success)
            {
                msg = msg.Replace("**STATUS UNKNOWN**", "was successfully sent");
            }
            else
            {
                msg = msg.Replace("**STATUS UNKNOWN**", "failed to transfer");
            }

            cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : FtpReport : {0}", msg));
            cAuditLog alog = new cAuditLog(Accountid, rpt.employeeid);
            alog.addRecord(SpendManagementElement.ReportsExport, msg, rpt.scheduleid);

            return msg;
        }

        public delegate object DelProcesReport(cReportRequest request);

        static void ProcessReports(object state)
        {
            try
            {
                if (Expenses_Scheduler.ListInUse)
                {
                    return;
                }

                lock (((ICollection)Expenses_Scheduler.ScheduleRequests).SyncRoot)
                {
                    Expenses_Scheduler.ListInUse = true;
                    IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

                    foreach (cScheduleRequest request in Expenses_Scheduler.ScheduleRequests)
                    {
                        if (request.Status == ReportRequestStatus.Failed)
                        {
                            Expenses_Scheduler.DiagLog(string.Format("cScheduledReports : ProcessReports : Report failed for ScheduleID: {0}", request.ScheduleID));
                            AddAuditLogEntry(request.AccountID, request.EmployeeID, string.Format("The schedule with ID {0} failed due to an error in the report.", request.ScheduleID));
                            SendError(request);
                        }
                        else
                        {
                            if (request.ReportRequest != null && request.Status == ReportRequestStatus.BeingProcessed)
                            {
                                object[] reportData = clsreports.getReportProgress(request.ReportRequest);
                                if (reportData != null)
                                {
                                    if (reportData[0].ToString() == "Complete")
                                    {
                                        Expenses_Scheduler.DiagLog(string.Format("cScheduledReports : ProcessReports : Report successful for ScheduleID: {0}", request.ScheduleID));
                                        request.Status = ReportRequestStatus.Complete;
                                        request.ReportData = (byte[])clsreports.getReportData(request.ReportRequest);
                                        SendSchedule(request);
                                        clsreports.cancelRequest(request.ReportRequest);
                     
                                    }
                                    else if (reportData[0].ToString() == "Failed")
                                    {
                                        Expenses_Scheduler.DiagLog(string.Format("cScheduledReports : ProcessReports : Report failed for ScheduleID: {0}", request.ScheduleID));
                                        AddAuditLogEntry(request.AccountID, request.EmployeeID, string.Format("The schedule with ID {0} failed due to an error in the report.", request.ScheduleID));
                                        SendError(request);
                                        request.Status = ReportRequestStatus.Failed;

                                        clsreports.cancelRequest(request.ReportRequest);
                                    }
                                }
                                else
                                {
                                    // no report progress found, so abort scheduled request
                                    cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : ProcessReports : Report Progress could not be obtained for schedule ID {0}. Schedule Failed.", request.ScheduleID));
                                    AddAuditLogEntry(request.AccountID, request.EmployeeID, string.Format("Could not obtain progress for the report {0}. The schedule with ID {1} failed.", request.ReportRequest.report.reportname, request.ScheduleID));
                                    request.Status = ReportRequestStatus.Failed;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Scheduler : cScheduledReports : ProcessReports : Error {0}\n{1}", ex.Message, ex.StackTrace);
                Expenses_Scheduler.DiagLog(msg);
                cEventlog.LogEntry(msg);
            }
            finally
            {
                Expenses_Scheduler.ListInUse = false;
            }
        }

        private static void SendError(cScheduleRequest request, string errMsg = "", string errStackDump = "", string msgBody = "")
        {
            try
            {
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(request.AccountID);
                cAccountProperties reqProperties;

                if (request.ReportRequest != null && request.ReportRequest.SubAccountId > -1)
                {
                    reqProperties = clsSubAccounts.getSubAccountById(request.ReportRequest.SubAccountId).SubAccountProperties;
                }
                else
                {
                    reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
                }
                string sSmtpServer = reqProperties.EmailServerAddress;
                cEmails clsEmails = new cEmails(sSmtpServer);

                string fromAddress = "admin@sel-expenses.com";
                string toAddress = "support@selenity.com";
                if (reqProperties.ErrorEmailFromAddress != "")
                {
                    fromAddress = reqProperties.ErrorEmailFromAddress;
                }
                if (reqProperties.ErrorEmailAddress != "")
                {
                    toAddress = reqProperties.ErrorEmailAddress;
                }

                clsEmails.SendErrorMail(fromAddress, toAddress, request, true, errMsg: errMsg, errStackDump: errStackDump, msgBody: msgBody);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : SendError : Error sending the error! Message: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        private static void SendSchedule(cScheduleRequest request)
        {
            try
            {
                cScheduledReports clsschedules = new cScheduledReports(request.AccountID);
                cScheduledReport rpt = clsschedules.GetScheduledReportById(request.ScheduleID);
                if (request.ReportData == null)
                {
                    if (rpt.scheduletype == ScheduleType.Once) //delete it
                    {
                        clsschedules.DeleteSchedule(request.ScheduleID);
                    }
                    return;
                }

                byte[] file = request.ReportData;
                int scheduleid = request.ScheduleID;
                int accountid = request.AccountID;
                cExportOptions clsoptions = request.ReportRequest.report.exportoptions;
                //IESR clsESR = (IESR)Activator.GetObject(typeof(IESR), ConfigurationManager.AppSettings["ESRService"] + "/ESR.rem");

                cAccounts clsAccount = new cAccounts();
                cAccount account = clsAccount.GetAccountByID(accountid);

                if (file.Length == 0)
                {
                    cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : SendSchedule : Schedule with Schedule ID {0} for company {1} has no records and has cancelled", scheduleid, account.companyid));
                    AddAuditLogEntry(request.AccountID, request.EmployeeID, string.Format("The schedule with ID {0} for report {1} has no records and has been cancelled.", scheduleid, request.ReportRequest.report.reportname));
                    if (rpt.scheduletype == ScheduleType.Once) //delete it
                    {
                        clsschedules.DeleteSchedule(scheduleid);
                    }
                    return;
                }
                string filename = "";

                if (clsoptions.application == FinancialApplication.ESR)
                {
                    try
                    {
                        // BEN BEN filename = clsESR.getESRInboundFilename(accountid, true);
                    }
                    catch
                    {
                        cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : sendSchedule : Error obtaining ESR Inbound Filename from IOffline for schedule id = {0}", scheduleid));
                    }
                }
                else
                {
                    filename = request.ReportRequest.report.reportname;
                }


                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
                cAccountProperties reqProperties;
                if (request.ReportRequest.SubAccountId > -1)
                {
                    reqProperties = clsSubAccounts.getSubAccountById(request.ReportRequest.SubAccountId).SubAccountProperties;
                }
                else
                {
                    reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
                }
                string sSmtpServer = reqProperties.EmailServerAddress;

                ContentType ct = new ContentType(MediaTypeNames.Application.Octet);
                switch (rpt.outputtype)
                {
                    case ExportType.Excel:
                    case ExportType.Pivot:
                        if (clsoptions.application == FinancialApplication.CustomReport)
                        {
                            filename += ".xls";
                        }
                        break;
                    case ExportType.CSV:
                        if (clsoptions.application == FinancialApplication.CustomReport)
                        {
                            filename += ".csv";
                        }
                        break;
                    case ExportType.FlatFile:
                        if (clsoptions.application == FinancialApplication.CustomReport)
                        {
                            filename += ".txt";
                        }
                        break;
                }

                //Stream tostream = File.Create(filename);

                MemoryStream tostream = new MemoryStream(file);
                Attachment attachment;
                ContentDisposition disposition;
                cEmails clsemails;
                StringBuilder body = new StringBuilder();
                string fromAddress = "admin@sel-expenses.com";
                cEmployees emps = new cEmployees(request.AccountID);

                if (reqProperties.SourceAddress == 1 && reqProperties.EmailAdministrator != "")
                {
                    fromAddress = reqProperties.EmailAdministrator;
                }

                if (reqProperties.SourceAddress == 0)
                {
                    Employee senderEmployee = emps.GetEmployeeById(request.EmployeeID);
                    if (senderEmployee.EmailAddress != "")
                    {
                        fromAddress = senderEmployee.EmailAddress;
                    }
                }

                string subject = "Scheduled Report: " + request.ReportRequest.report.reportname;
                System.Diagnostics.Debug.WriteLine("Sending report for reportid " + request.ReportRequest.report.reportid.ToString() + " with uniqueID " + request.ReportRequest.SchedulerRequestID.ToString());

                switch (rpt.deliverymethod)
                {
                    case DeliveryType.email:
                        clsemails = new cEmails(sSmtpServer);

                        //create the attachment
                        attachment = new Attachment(tostream, ct);
                        disposition = attachment.ContentDisposition;
                        disposition.FileName = filename;
                        body.Append("The attached report has been scheduled for execution and you were nominated as a recipient.\n\n");
                        body.Append(rpt.emailbody);
                        body.Append("\n\n\n");
                        body.Append("This email was automatically generated. ** Please do not reply **");
                        SendEmail(clsemails, fromAddress, clsschedules.GetUserEmail(rpt.employeeid), subject, body.ToString(), request, attachment);
                        break;
                    case DeliveryType.multipleemail:
                        clsemails = new cEmails(sSmtpServer);
                        ArrayList addresses = clsschedules.GetEmailAddresses(rpt.emailaddresses);
                        attachment = new Attachment(tostream, ct);
                        disposition = attachment.ContentDisposition;
                        disposition.FileName = filename;

                        body.Append("The attached report has been scheduled for execution and you were nominated as a recipient.\n\n");
                        body.Append(rpt.emailbody);
                        body.Append("\n\n\n");
                        body.Append("This email was automatically generated. ** Please do not reply **");
                        foreach (object t in addresses)
                        {
                            SendEmail(clsemails, fromAddress, (string)t, subject, body.ToString(), request, attachment);
                        }
                        break;
                    case DeliveryType.FTP:
                        string msg = clsschedules.FtpReport(rpt, file, filename);

                        clsemails = new cEmails(sSmtpServer);
                        body.Append(msg);
                        body.Append("\n\n\n");
                        body.Append("This email was automatically generated. ** Please do not reply **");

                        cEmployees clsemployees = new cEmployees(accountid);
                        Employee emp = clsemployees.GetEmployeeById(rpt.employeeid);

                        if (emp.EmailAddress != string.Empty)
                        {
                            SendEmail(clsemails, fromAddress, emp.EmailAddress, subject, body.ToString(), request, null);
                        }
                        // also inform administrator as long as it's not the same person as requested the ftp transfer
                        if (reqProperties.MainAdministrator > 0 && reqProperties.MainAdministrator != rpt.employeeid)
                        {
                            emp = clsemployees.GetEmployeeById(reqProperties.MainAdministrator);
                            if (emp.EmailAddress != string.Empty)
                            {
                                SendEmail(clsemails, fromAddress, emp.EmailAddress, subject, body.ToString(), request, null);
                            }
                        }
                        break;
                }

                clsschedules.AddLogEntry(scheduleid);

                if (rpt.scheduletype == ScheduleType.Once) //delete it
                {
                    clsschedules.DeleteSchedule(scheduleid);
                }

                cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : SendSchedule : Schedule with Schedule ID {0} ran successfully for company {1}", scheduleid, account.companyid));
                AddAuditLogEntry(request.AccountID, request.EmployeeID, string.Format("The schedule with ID {0} for report {1} ran successfully.", scheduleid, request.ReportRequest.report.reportname));
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(string.Format("Scheduler : cScheduledReports : SendSchedule : Error : {0}\n{1}", ex.Message, ex.StackTrace));
                AddAuditLogEntry(request.AccountID, request.EmployeeID, string.Format("An error has occurred whilst running the schedule with ID {0} for report {1}.", request.ScheduleID, request.ReportRequest.report.reportname));
            }
            }


        /// <summary>
        /// Method used to send emails and write in the audit log
        /// </summary>
        /// <param name="emails">Object that contains the email methods</param>
        /// <param name="fromAddress">The from address</param>
        /// <param name="toAddress">The to address</param>
        /// <param name="subject">The email subject</param>
        /// <param name="body">The email body</param>
        /// <param name="request">The scheduler request</param>
        /// <param name="attachment">The email attachment</param>
        private static void SendEmail(cEmails emails, string fromAddress, string toAddress, string subject, string body, cScheduleRequest request, Attachment attachment)
        {
            bool emailSent = emails.SendMail(fromAddress, toAddress, subject, body.ToString(), attachment);
            AddAuditLogEntry(request.AccountID, request.EmployeeID,
                emailSent
                    ? string.Format("Schedule with ID {0} for report {1} successfully sent email to {2}.",
                        request.ScheduleID, request.ReportRequest.report.reportname, toAddress)
                    : string.Format("Schedule with ID {0} for report {1} failed to send email to {2}.", request.ScheduleID,
                        request.ReportRequest.report.reportname, toAddress));
        }
        
        
        //private string getPath()
        //{
        //    Assembly ass = Assembly.GetAssembly(typeof(cScheduledReports));
        //    string location = ass.Location.Replace("expenses_scheduler.exe", "");
        //    return location;
        //}

        private string GetUserEmail(int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            SqlDataReader reader;
            string email = "";
            const string strsql = "select email from employees where employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			using (reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();
				while (reader.Read())
				{
					email = reader.GetString(0);
				}
				reader.Close();
			}
            return email;
        }

        private void AddLogEntry(int scheduleid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(Accountid));
            const string strsql = "insert into scheduled_reports_log (scheduleid) values (@scheduleid)";
            expdata.sqlexecute.Parameters.AddWithValue("@scheduleid", scheduleid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Add entry to the audit log
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="employeeId">The employee Id</param>
        /// <param name="message">The message to add in the audit log</param>
        internal static void AddAuditLogEntry(int accountId, int employeeId, string message)
        {
            var auditLog = new cAuditLog(accountId, employeeId);
            auditLog.addRecord(SpendManagementElement.Scheduler, message, 0, fromReportsOrScheduler:true);
        }

        public System.Data.DataTable GetGrid(int employeeid)
        {
            var tbl = new DataTable();
            tbl.Columns.Add("scheduleid", typeof(System.Int32));
            tbl.Columns.Add("reportid", typeof(System.Guid));
            tbl.Columns.Add("financialexportid", typeof(System.Int32));
            tbl.Columns.Add("startdate", typeof(System.DateTime));
            tbl.Columns.Add("enddate", typeof(System.DateTime));
            tbl.Columns.Add("scheduletype", typeof(System.String));
            tbl.Columns.Add("outputtype", typeof(System.String));
            tbl.Columns.Add("deliverymethod", typeof(System.String));


            foreach (var scheduledReport in ScheduledReportsList.Values)
            {
                if (scheduledReport.employeeid != employeeid) continue;

                object[] values = new object[8];
                values[0] = scheduledReport.scheduleid;
                values[1] = scheduledReport.reportid;
                values[2] = scheduledReport.financialexportid;
                values[3] = scheduledReport.startdate;
                values[4] = scheduledReport.enddate;
                values[5] = scheduledReport.scheduletype.ToString();
                values[6] = scheduledReport.outputtype.ToString();
                values[7] = scheduledReport.deliverymethod.ToString();
                tbl.Rows.Add(values);    
            }

            return tbl;
        }
    }
}

public class FtpState
{
    private ManualResetEvent wait;
    private FtpWebRequest request;
    private string fileName;
    private Exception operationException = null;
    private MemoryStream stream;
    string status;

    public FtpState()
    {
        wait = new ManualResetEvent(false);
    }

    public ManualResetEvent OperationComplete
    {
        get { return wait; }
    }

    public FtpWebRequest Request
    {
        get { return request; }
        set { request = value; }
    }

    public string FileName
    {
        get { return fileName; }
        set { fileName = value; }
    }

    public MemoryStream data
    {
        get { return stream; }
        set { stream = value; }
    }
    public Exception OperationException
    {
        get { return operationException; }
        set { operationException = value; }
    }
    public string StatusDescription
    {
        get { return status; }
        set { status = value; }
    }
}
