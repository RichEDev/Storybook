using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SpendManagementLibrary;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cScheduledReport
    {
        protected int nAccountid;
        protected int nScheduleid;
        protected Guid gReportid;
        protected int nFinancialExportid;
        protected int nEmployeeid;
        protected ScheduleType stScheduletype;
        protected DateTime dtStartdate;
        protected DateTime dtEnddate;
        protected DateTime dtStarttime;
        protected ExportType etOutputtype;
        protected DeliveryType dtDeliverymethod;
        protected SortedList lstColumns;
        protected SortedList lstCriteria;
        protected string sEmailaddresses;
        protected string sFTPServer;
        protected string sFTPUsername;
        protected string sFTPPassword;
        protected string sEmailBody;
        protected bool bFTPUseSSL;

        public cScheduledReport()
        {

        }

        public cScheduledReport(int accountid, int scheduleid, Guid reportid, int financialexportid, int employeeid, ScheduleType scheduletype, ExportType exporttype, DeliveryType deliverymethod, DateTime startdate, DateTime enddate, DateTime startime, SortedList columns, SortedList criteria, string emailaddresses, string ftpserver, string ftpusername, string ftppassword, string emailbody, bool ftpusessl)
        {
            nAccountid = accountid;
            nScheduleid = scheduleid;
            gReportid = reportid;
            nFinancialExportid = financialexportid;
            nEmployeeid = employeeid;
            stScheduletype = scheduletype;
            dtStartdate = startdate;
            dtEnddate = enddate;
            dtStarttime = startime;
            etOutputtype = exporttype;
            dtDeliverymethod = deliverymethod;
            lstColumns = columns;
            lstCriteria = criteria;
            sEmailaddresses = emailaddresses;
            sFTPServer = ftpserver;
            sFTPUsername = ftpusername;
            sFTPPassword = ftppassword;
            sEmailBody = emailbody;
            bFTPUseSSL = ftpusessl;
        }
        public void setScheduleid(int id)
        {
            nScheduleid = id;
        }
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public int scheduleid
        {
            get { return nScheduleid; }
        }
        public Guid reportid
        {
            get { return gReportid; }
        }
        public int financialexportid
        {
            get { return nFinancialExportid; }
        }
        public int employeeid
        {
            get { return nEmployeeid; }
        }
        public ScheduleType scheduletype
        {
            get { return stScheduletype; }
        }
        public DateTime startdate
        {
            get { return dtStartdate; }
        }
        public DateTime enddate
        {
            get { return dtEnddate; }
        }
        public DateTime starttime
        {
            get { return dtStarttime; }
        }
        public ExportType outputtype
        {
            get { return etOutputtype; }
        }
        public DeliveryType deliverymethod
        {
            get { return dtDeliverymethod; }
        }
        public SortedList columns
        {
            get { return lstColumns; }
        }
        public SortedList criteria
        {
            get { return lstCriteria; }
        }
        public string emailaddresses
        {
            get { return sEmailaddresses; }
        }
        public string ftpserver
        {
            get { return sFTPServer; }
        }
        public string ftpusername
        {
            get { return sFTPUsername; }
        }
        public bool ftpusessl
        {
            get { return bFTPUseSSL; }
        }
        public string ftppassword
        {
            get { return sFTPPassword; }
        }
        public string emailbody
        {
            get { return sEmailBody; }
        }
        #endregion
    }

    [Serializable()]
    public class cDailyScheduledReport : cScheduledReport
    {
        private ArrayList arrDaysofweek;
        private byte bRepeatFrequency;


        public cDailyScheduledReport(int accountid, int scheduleid, Guid reportid, int financialexportid, int employeeid, ScheduleType scheduletype, ExportType exporttype, DeliveryType deliverymethod, DateTime startdate, DateTime enddate, DateTime startime, ArrayList daysofweek, byte repeatfrequency, SortedList columns, SortedList criteria, string emailaddresses, string ftpserver, string ftpusername, string ftppassword, string emailbody, bool ftpusessl)
        {
            nAccountid = accountid;
            nScheduleid = scheduleid;
            gReportid = reportid;
            nFinancialExportid = financialexportid;
            nEmployeeid = employeeid;
            stScheduletype = scheduletype;
            arrDaysofweek = daysofweek;
            bRepeatFrequency = repeatfrequency;
            dtStartdate = startdate;
            dtEnddate = enddate;
            dtStarttime = startime;
            etOutputtype = exporttype;
            dtDeliverymethod = deliverymethod;
            lstColumns = columns;
            lstCriteria = criteria;
            sEmailaddresses = emailaddresses;
            sFTPServer = ftpserver;
            sFTPUsername = ftpusername;
            sFTPPassword = ftppassword;
            sEmailBody = emailbody;
            bFTPUseSSL = ftpusessl;
        }
        #region properties
        public ArrayList daysofweek
        {
            get { return arrDaysofweek; }
        }
        public byte repeatfrequency
        {
            get { return bRepeatFrequency; }
        }
        #endregion
    }

    [Serializable()]
    public class cWeeklyScheduledReport : cScheduledReport
    {
        private ArrayList arrDaysofweek;
        private byte bRepeatFrequency;

        public cWeeklyScheduledReport(int accountid, int scheduleid, Guid reportid, int financialexportid, int employeeid, ScheduleType scheduletype, ExportType exporttype, DeliveryType deliverymethod, DateTime startdate, DateTime enddate, DateTime startime, ArrayList daysofweek, byte repeatfrequency, SortedList columns, SortedList criteria, string emailaddresses, string ftpserver, string ftpusername, string ftppassword, string emailbody, bool ftpusessl)
        {
            nAccountid = accountid;
            nScheduleid = scheduleid;
            gReportid = reportid;
            nFinancialExportid = financialexportid;
            nEmployeeid = employeeid;
            stScheduletype = scheduletype;
            arrDaysofweek = daysofweek;
            bRepeatFrequency = repeatfrequency;
            dtStartdate = startdate;
            dtEnddate = enddate;
            dtStarttime = startime;
            etOutputtype = exporttype;
            dtDeliverymethod = deliverymethod;
            lstColumns = columns;
            lstCriteria = criteria;
            sEmailaddresses = emailaddresses;
            sFTPServer = ftpserver;
            sFTPUsername = ftpusername;
            sFTPPassword = ftppassword;
            sEmailBody = emailbody;
            bFTPUseSSL = ftpusessl;
        }
        #region properties
        public ArrayList daysofweek
        {
            get { return arrDaysofweek; }
        }
        public byte repeatfrequency
        {
            get { return bRepeatFrequency; }
        }
        #endregion
    }

    [Serializable()]
    public class cMonthlyScheduledReport : cScheduledReport
    {
        private ArrayList arrMonths;
        private byte bWeek;
        private string sCalendardays;
        private ArrayList arrDaysofweek;
        private ArrayList arrCalendardays;

        public cMonthlyScheduledReport(int accountid, int scheduleid, Guid reportid, int financialexportid, int employeeid, ScheduleType scheduletype, ExportType exporttype, DeliveryType deliverymethod, DateTime startdate, DateTime enddate, DateTime starttime, ArrayList months, byte week, ArrayList daysofweek, string calendardays, SortedList columns, SortedList criteria, string emailaddresses, string ftpserver, string ftpusername, string ftppassword, string emailbody, bool ftpusessl)
        {
            nAccountid = accountid;
            nScheduleid = scheduleid;
            gReportid = reportid;
            nFinancialExportid = financialexportid;
            nEmployeeid = employeeid;
            stScheduletype = scheduletype;
            arrMonths = months;
            bWeek = week;
            arrDaysofweek = daysofweek;
            sCalendardays = calendardays;
            dtStartdate = startdate;
            dtEnddate = enddate;
            dtStarttime = starttime;
            etOutputtype = exporttype;
            dtDeliverymethod = deliverymethod;
            lstColumns = columns;
            lstCriteria = criteria;
            sEmailaddresses = emailaddresses;
            sFTPServer = ftpserver;
            sFTPUsername = ftpusername;
            sFTPPassword = ftppassword;
            sEmailBody = emailbody;
            bFTPUseSSL = ftpusessl;
        }

        #region properties
        public ArrayList months
        {
            get { return arrMonths; }
        }
        public byte week
        {
            get { return bWeek; }
        }
        public ArrayList daysofweek
        {
            get { return arrDaysofweek; }
        }

        public string calendardays
        {
            get { return sCalendardays; }
        }
        public ArrayList aCalendardays
        {
            get { return arrCalendardays; }
        }

        #endregion
    }





    public enum ScheduleType
    {
        Day = 1,
        Week,
        Month,
        Once
    }

    
}
