using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using BusinessLogic.Modules;

using expenses;
using SpendManagementLibrary;
using Spend_Management;




public partial class reports_aeschedule : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Add / Edit Schedule";
        Master.title = "Scheduled Reports";
        Master.PageSubTitle = Title;
        Master.enablenavigation = false;

        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            ViewState["subaccountid"] = user.CurrentSubAccountId;
            Guid reportid;

            switch (user.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1154;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            int financialexportid = 0;

            if (Request.QueryString["financialexportid"] != null)
            {
                financialexportid = int.Parse(Request.QueryString["financialexportid"]);
            }

            ViewState["financialexportid"] = financialexportid;

            ViewState["returnto"] = int.Parse(Request.QueryString["returnto"]);

            IScheduler clsscheduler = (IScheduler)Activator.GetObject(typeof(IScheduler), ConfigurationManager.AppSettings["SchedulerServicePath"] + "/scheduler.rem");
            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");

            Spend_Management.Action action = Spend_Management.Action.Add;

            if (Request.QueryString["action"] != null)
            {
                action = (Spend_Management.Action)byte.Parse(Request.QueryString["action"]);
                
            }

            cFinancialExport export = null;
            if (financialexportid > 0)
            {
                cFinancialExports clsexports = new cFinancialExports((int)ViewState["accountid"]);
                export = clsexports.getExportById(financialexportid);
                reportid = export.reportid;
            }
            else
            {
                reportid = new Guid(Request.QueryString["reportid"].ToString());
            }

            ViewState["reportid"] = reportid;

            cReport reqreport = clsreports.getReportById(user.AccountID, reportid);
            ViewState["report"] = reqreport;

            ViewState["action"] = action;
            if (action == Spend_Management.Action.Edit)
            {
                int scheduleid = int.Parse(Request.QueryString["scheduleid"]);
                ViewState["scheduleid"] = scheduleid;

                cScheduledReport rpt = clsscheduler.getScheduledReportById(user.AccountID, scheduleid);
                if (rpt.columns.Count > 0)
                {
                    criteria.staticColumns = rpt.columns;
                }

                //update report criteria
                foreach (cReportCriterion criterion in reqreport.criteria)
                {
                    if (criterion.runtime)
                    {
                        object[] curRuntimeCriteria = (object[])rpt.criteria[criterion.criteriaid];
                        criterion.updateValues((object[])curRuntimeCriteria[0], (object[])curRuntimeCriteria[1]);
                    }
                }
                ViewState["report"] = reqreport;
                txtstartdate.Text = rpt.startdate.ToShortDateString();
                if (rpt.enddate != new DateTime(1900, 01, 01))
                {
                    txtenddate.Text = rpt.enddate.ToShortDateString();
                }
                if (cmboutputtype.Items.FindByValue(((byte)rpt.outputtype).ToString()) != null)
                {
                    cmboutputtype.Items.FindByValue(((byte)rpt.outputtype).ToString()).Selected = true;
                }
                if (cmbdeliverymethod.Items.FindByValue(((byte)rpt.deliverymethod).ToString()) != null)
                {
                    cmbdeliverymethod.Items.FindByValue(((byte)rpt.deliverymethod).ToString()).Selected = true;
                }
                if (lstscheduletype.Items.FindByValue(((byte)rpt.scheduletype).ToString()) != null)
                {
                    lstscheduletype.Items.FindByValue(((byte)rpt.scheduletype).ToString()).Selected = true;
                    scheduleview.ActiveViewIndex = (int)rpt.scheduletype-1;
                }

                if (rpt.deliverymethod == DeliveryType.FTP)
                {
                    reqftppassword.Enabled = true;
                    reqftpusername.Enabled = true;
                    reqftpaddress.Enabled = true;
                    
                    txtftpaddress.Text = rpt.ftpserver;
                    txtftpusername.Text = rpt.ftpusername;
                    chkusessl.Checked = rpt.ftpusessl;
                    deliveryview.ActiveViewIndex = 1;
                }
                else if (rpt.deliverymethod == DeliveryType.multipleemail)
                {
                    reqemails.Enabled = true;
                    txtmultipleemail.Text = rpt.emailaddresses;
                    deliveryview.ActiveViewIndex = 0;
                }
                txtEmailBody.Text = rpt.emailbody;
                switch (rpt.scheduletype)
                {
                    case ScheduleType.Day:
                        cDailyScheduledReport dailyrpt = (cDailyScheduledReport)rpt;
                        txtdayshour.Text = dailyrpt.starttime.Hour.ToString("##00");
                        txtdaysminutes.Text = dailyrpt.starttime.Minute.ToString("##00");
                        if (dailyrpt.daysofweek.Count == 0)
                        {
                            optdaysnumber.Checked = true;
                            txtnumdays.Text = dailyrpt.repeatfrequency.ToString();
                        }
                        else
                        {
                            if (dailyrpt.daysofweek.Contains(DayOfWeek.Monday) && dailyrpt.daysofweek.Contains(DayOfWeek.Tuesday) && dailyrpt.daysofweek.Contains(DayOfWeek.Wednesday) && dailyrpt.daysofweek.Contains(DayOfWeek.Thursday) && dailyrpt.daysofweek.Contains(DayOfWeek.Friday) && dailyrpt.daysofweek.Count == 5)
                            {
                                optweekdays.Checked = true;
                            }
                            else
                            {
                                optdays.Checked = true;
                                chkdayssunday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Sunday);
                                chkdaysmonday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Monday);
                                chkdaystuesday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Tuesday);
                                chkdayswednesday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Wednesday);
                                chkdaysthursday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Thursday);
                                chkdaysfriday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Friday);
                                chkdayssaturday.Checked = dailyrpt.daysofweek.Contains(DayOfWeek.Saturday);
                            }


                        }
                        break;
                    case ScheduleType.Week:
                        cWeeklyScheduledReport weeklyrpt = (cWeeklyScheduledReport)rpt;
                        txtweekhour.Text = weeklyrpt.starttime.Hour.ToString("##00");
                        txtweekminutes.Text = weeklyrpt.starttime.Minute.ToString("##00");
                        txtnumweeks.Text = weeklyrpt.repeatfrequency.ToString();
                        chkweeksunday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Sunday);
                        chkweekmonday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Monday);
                        chkweektuesday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Tuesday);
                        chkweekwednesday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Wednesday);
                        chkweekthursday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Thursday);
                        chkweekfriday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Friday);
                        chkweeksaturday.Checked = weeklyrpt.daysofweek.Contains(DayOfWeek.Saturday);
                        break;
                    case ScheduleType.Month:
                        cMonthlyScheduledReport monthlyrpt = (cMonthlyScheduledReport)rpt;
                        txtmonthhour.Text = monthlyrpt.starttime.Hour.ToString("##00");
                        txtmonthminutes.Text = monthlyrpt.starttime.Minute.ToString("##00");
                        chkmonthjan.Checked = monthlyrpt.months.Contains((byte)1);
                        chkmonthfeb.Checked = monthlyrpt.months.Contains((byte)2);
                        chkmonthmarch.Checked = monthlyrpt.months.Contains((byte)3);
                        chkmonthapril.Checked = monthlyrpt.months.Contains((byte)4);
                        chkmonthmay.Checked = monthlyrpt.months.Contains((byte)5);
                        chkmonthjune.Checked = monthlyrpt.months.Contains((byte)6);
                        chkmonthjuly.Checked = monthlyrpt.months.Contains((byte)7);
                        chkmonthaugust.Checked = monthlyrpt.months.Contains((byte)8);
                        chkmonthsep.Checked = monthlyrpt.months.Contains((byte)9);
                        chkmonthoct.Checked = monthlyrpt.months.Contains((byte)10);
                        chkmonthnov.Checked = monthlyrpt.months.Contains((byte)11);
                        chkmonthdec.Checked = monthlyrpt.months.Contains((byte)12);
                        if (monthlyrpt.calendardays == "")
                        {
                            cmbmonthweek.Items.FindByValue(monthlyrpt.week.ToString()).Selected = true;
                            chkmonthsunday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Sunday);
                            chkmonthmonday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Monday);
                            chkmonthtuesday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Tuesday);
                            chkmonthwednesday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Wednesday);
                            chkmonththursday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Thursday);
                            chkmonthfriday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Friday);
                            chkmonthsaturday.Checked = monthlyrpt.daysofweek.Contains(DayOfWeek.Saturday);
                        }
                        else
                        {
                            optmonthcalendar.Checked = true;
                            txtcalendardays.Text = monthlyrpt.calendardays;
                        }
                        break;
                    case ScheduleType.Once:
                        txtoncehour.Text = rpt.starttime.Hour.ToString("##00");
                        txtonceminutes.Text = rpt.starttime.Minute.ToString("##00");
                        break;
                }
            }
            else
            {
                txtstartdate.Text = DateTime.Today.ToShortDateString();
                txtdayshour.Text = DateTime.Now.Hour.ToString();
                txtweekhour.Text = DateTime.Now.Hour.ToString();
                txtmonthhour.Text = DateTime.Now.Hour.ToString();
                txtdaysminutes.Text = DateTime.Now.AddMinutes(15).Minute.ToString();
                txtweekminutes.Text = DateTime.Now.AddMinutes(15).Minute.ToString();
                txtmonthminutes.Text = DateTime.Now.AddMinutes(15).Minute.ToString();
                txtnumweeks.Text = "1";
                lstscheduletype.Items[0].Selected = true;
                scheduleview.ActiveViewIndex = 0;
            }
            
        }

        criteria.accountid = (int)ViewState["accountid"];
        criteria.report = (cReport)ViewState["report"];
    }
    protected void lstfrequency_SelectedIndexChanged(object sender, EventArgs e)
    {
        scheduleview.ActiveViewIndex = lstscheduletype.SelectedIndex;
    }
    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        cSecureData clssecure = new cSecureData();
        Guid reportid = new Guid(ViewState["reportid"].ToString());
        ExportType outputtype = (ExportType)byte.Parse(cmboutputtype.SelectedValue);
        DeliveryType deliverymethod = (DeliveryType)byte.Parse(cmbdeliverymethod.SelectedValue);

        ScheduleType scheduletype = (ScheduleType)byte.Parse(lstscheduletype.SelectedValue);

        DateTime startdate = DateTime.Parse(txtstartdate.Text);
        DateTime enddate;
        DateTime starttime;
        string emailaddresses, ftpserver, ftpusername, ftppassword, emailbody;
        bool ftpusessl;

        if (txtenddate.Text == "")
        {
            enddate = new DateTime(1900, 01, 01);
        }
        else
        {
            enddate = DateTime.Parse(txtenddate.Text);
        }
        int scheduleid = 0;
        if (ViewState["scheduleid"] != null)
        {
            scheduleid = (int)ViewState["scheduleid"];
        }
        int subaccountid = (int)ViewState["subaccountid"];
        ArrayList days = new ArrayList();
        cScheduledReport rpt = null;
        byte repeate_frequency = 0;
        cReport reqreport = (cReport)ViewState["report"];
        SortedList columns = getColumns(reqreport.columns);
        SortedList criteria = new SortedList();

        emailaddresses = txtmultipleemail.Text;
        ftpserver = txtftpaddress.Text;
        ftpusername = txtftpusername.Text;
        ftppassword = txtftppassword.Text;
        ftpusessl = chkusessl.Checked;

        ftppassword = clssecure.Encrypt(ftppassword);
        emailbody = txtEmailBody.Text;
        switch (scheduletype)
        {
            case ScheduleType.Day:
                if (optdays.Checked)
                {
                    if (chkdayssunday.Checked)
                    {
                        days.Add(DayOfWeek.Sunday);
                    }
                    if (chkdaysmonday.Checked)
                    {
                        days.Add(DayOfWeek.Monday);
                    }
                    if (chkdaystuesday.Checked)
                    {
                        days.Add(DayOfWeek.Tuesday);
                    }
                    if (chkdayswednesday.Checked)
                    {
                        days.Add(DayOfWeek.Wednesday);
                    }
                    if (chkdaysthursday.Checked)
                    {
                        days.Add(DayOfWeek.Thursday);
                    }
                    if (chkdaysfriday.Checked)
                    {
                        days.Add(DayOfWeek.Friday);
                    }
                    if (chkdayssaturday.Checked)
                    {
                        days.Add(DayOfWeek.Saturday);
                    }
                }
                else if (optweekdays.Checked)
                {
                    days.Add(DayOfWeek.Monday);
                    days.Add(DayOfWeek.Tuesday);
                    days.Add(DayOfWeek.Wednesday);
                    days.Add(DayOfWeek.Thursday);
                    days.Add(DayOfWeek.Friday);
                }
                else
                {
                    repeate_frequency = byte.Parse(txtnumdays.Text);
                }
                starttime = new DateTime(1900,01,01,byte.Parse(txtdayshour.Text), byte.Parse(txtdaysminutes.Text),0);
                rpt = new cDailyScheduledReport((int)ViewState["accountid"], scheduleid, (Guid)ViewState["reportid"], (int)ViewState["financialexportid"], (int)ViewState["employeeid"], ScheduleType.Day, outputtype, deliverymethod, startdate, enddate, starttime, days, repeate_frequency, columns, getCriteria(), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
                break;
            case ScheduleType.Week:
                repeate_frequency = byte.Parse(txtnumweeks.Text);
                if (chkweeksunday.Checked)
                {
                    days.Add(DayOfWeek.Sunday);
                }
                if (chkweekmonday.Checked)
                {
                    days.Add(DayOfWeek.Monday);
                }
                if (chkweektuesday.Checked)
                {
                    days.Add(DayOfWeek.Tuesday);
                }
                if (chkweekwednesday.Checked)
                {
                    days.Add(DayOfWeek.Wednesday);
                }
                if (chkweekthursday.Checked)
                {
                    days.Add(DayOfWeek.Thursday);
                }
                if (chkweekfriday.Checked)
                {
                    days.Add(DayOfWeek.Friday);
                }
                if (chkweeksaturday.Checked)
                {
                    days.Add(DayOfWeek.Saturday);
                }
                starttime = new DateTime(1900, 01, 01, byte.Parse(txtweekhour.Text), byte.Parse(txtweekminutes.Text), 0);
                rpt = new cWeeklyScheduledReport((int)ViewState["accountid"], scheduleid, (Guid)ViewState["reportid"], (int)ViewState["financialexportid"], (int)ViewState["employeeid"], ScheduleType.Week, outputtype, deliverymethod, startdate, enddate, starttime, days, repeate_frequency, getColumns(reqreport.columns), getCriteria(), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
                break;
            case ScheduleType.Month:
                byte week = 0;
                string calendardays = "";
                ArrayList months = new ArrayList();
                if (chkmonthjan.Checked)
                {
                    months.Add((byte)1);
                }
                if (chkmonthfeb.Checked)
                {
                    months.Add((byte)2);
                }
                if (chkmonthmarch.Checked)
                {
                    months.Add((byte)3);
                }
                if (chkmonthapril.Checked)
                {
                    months.Add((byte)4);
                }
                if (chkmonthmay.Checked)
                {
                    months.Add((byte)5);
                }
                if (chkmonthjune.Checked)
                {
                    months.Add((byte)6);
                }
                if (chkmonthjuly.Checked)
                {
                    months.Add((byte)7);
                }
                if (chkmonthaugust.Checked)
                {
                    months.Add((byte)8);
                }
                if (chkmonthsep.Checked)
                {
                    months.Add((byte)9);
                }
                if (chkmonthoct.Checked)
                {
                    months.Add((byte)10);
                }
                if (chkmonthnov.Checked)
                {
                    months.Add((byte)11);
                }
                if (chkmonthdec.Checked)
                {
                    months.Add((byte)12);
                }

                if (optmonthweek.Checked)
                {
                    week = byte.Parse(cmbmonthweek.SelectedValue);
                    if (chkmonthsunday.Checked)
                    {
                        days.Add(DayOfWeek.Sunday);
                    }
                    if (chkmonthmonday.Checked)
                    {
                        days.Add(DayOfWeek.Monday);
                    }
                    if (chkmonthtuesday.Checked)
                    {
                        days.Add(DayOfWeek.Tuesday);
                    }
                    if (chkmonthwednesday.Checked)
                    {
                        days.Add(DayOfWeek.Wednesday);
                    }
                    if (chkmonththursday.Checked)
                    {
                        days.Add(DayOfWeek.Thursday);
                    }
                    if (chkmonthfriday.Checked)
                    {
                        days.Add(DayOfWeek.Friday);
                    }
                    if (chkmonthsaturday.Checked)
                    {
                        days.Add(DayOfWeek.Saturday);
                    }
                }
                else
                {
                    calendardays = txtcalendardays.Text;
                }
                starttime = new DateTime(1900, 01, 01, byte.Parse(txtmonthhour.Text), byte.Parse(txtmonthminutes.Text), 0);
                rpt = new cMonthlyScheduledReport((int)ViewState["accountid"], scheduleid, (Guid)ViewState["reportid"], (int)ViewState["financialexportid"], (int)ViewState["employeeid"], ScheduleType.Month, outputtype, deliverymethod, startdate, enddate, starttime, months, week, days, calendardays, getColumns(reqreport.columns), getCriteria(), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
                break;
            case ScheduleType.Once:
                starttime = new DateTime(1900, 01, 01, byte.Parse(txtoncehour.Text), byte.Parse(txtonceminutes.Text), 0);
                rpt = new cScheduledReport((int)ViewState["accountid"], scheduleid, (Guid)ViewState["reportid"], (int)ViewState["financialexportid"], (int)ViewState["employeeid"], ScheduleType.Once, outputtype, deliverymethod, startdate, enddate, starttime, getColumns(reqreport.columns), getCriteria(), emailaddresses, ftpserver, ftpusername, ftppassword, emailbody, ftpusessl);
                break;
        }

        IScheduler clsscheduler = (IScheduler)Activator.GetObject(typeof(IScheduler), ConfigurationManager.AppSettings["SchedulerServicePath"] + "/scheduler.rem");

        Spend_Management.Action action = (Spend_Management.Action)ViewState["action"];
        if (action == Spend_Management.Action.Edit)
        {
            clsscheduler.updateSchedule(rpt);
        }
        else
        {
            clsscheduler.addSchedule(rpt);
        }

        int returnto = (int)ViewState["returnto"];

        switch (returnto)
        {
            case 1: //reports
                Response.Redirect("rptlist.aspx", true);
                break;
            case 2: //my schedules
                Response.Redirect("myschedules.aspx", true);
                break;
            case 3: // financial expoprts
                Response.Redirect(cMisc.Path + "/expenses/admin/financialexports.aspx", true);
                break;
        }
    }

    public SortedList getColumns(ArrayList columns)
    {
        cReportColumn column;
        cStaticColumn staticcol;
        SortedList lstcolumns = new SortedList();
        TextBox txtbox;
        for (int i = 0; i < columns.Count; i++)
        {
            column = (cReportColumn)columns[i];
            if (column.columntype == ReportColumnType.Static)
            {
                staticcol = (cStaticColumn)column;
                if (staticcol.runtime)
                {
                    txtbox = (TextBox)FindControl("ctl00$contentmain$criteria$statictxt" + staticcol.order);
                    lstcolumns.Add(staticcol.reportcolumnid, txtbox.Text);
                }
            }
        }
        return lstcolumns;
    }

    public SortedList getCriteria()
    {
        object[] value1;
        object[] value2 = null;
        object[] values;

        SortedList runtimecriteria = new SortedList();
        cReportCriterion reqcriteria;
        IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
        cReport reqreport = clsreports.getReportById((int)ViewState["accountid"], new Guid(ViewState["reportid"].ToString()));
        criteria.getRuntimeValues(ref reqreport);
        for (int i = 0; i < reqreport.criteria.Count; i++)
        {
            reqcriteria = (cReportCriterion)reqreport.criteria[i];
            if (reqcriteria.runtime)
            {
               
                value1 = reqcriteria.value1;
                value2 = reqcriteria.value2;
                if (reqcriteria.field.FieldType == "D")
                {
                    //if (value1 != null)
                    //{
                    //    if (value1.GetLength(0) > 0)
                    //    {
                    //        value1[0] = ((DateTime)value1[0]);
                    //    }
                    //}
                    //if (value2 != null)
                    //{
                    //    if (value2.GetLength(0) > 0)
                    //    {
                    //        value2[0] = ((DateTime)value2[0]);
                    //    }
                    //}
                }
                values = new object[2];
                values[0] = value1;
                if (value2 == null)
                {
                    values[1] = new object[0];
                }
                else
                {
                    
                    values[1] = value2;
                }
                runtimecriteria.Add(reqcriteria.criteriaid, values);
            }
        }
        return runtimecriteria;
    }
    protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
    {
        int returnto = (int)ViewState["returnto"];

        switch (returnto)
        {
            case 1: //reports
                Response.Redirect("rptlist.aspx", true);
                break;
            case 2: //my schedules
                Response.Redirect("myschedules.aspx", true);
                break;
            case 3: // financial expoprts
                Response.Redirect(cMisc.Path + "/expenses/admin/financialexports.aspx", true);
                break;
        }
    }
    protected void cmbdeliverymethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (cmbdeliverymethod.SelectedIndex)
        {
            case 0:
                deliveryview.ActiveViewIndex = -1;
                reqftpaddress.Enabled = false;
                reqftpusername.Enabled = false;
                reqftppassword.Enabled = false;
                reqemails.Enabled = false;
                break;
            case 1:
                deliveryview.ActiveViewIndex = 0;
                reqftpaddress.Enabled = false;
                reqftpusername.Enabled = false;
                reqftppassword.Enabled = false;
                reqemails.Enabled = true;
                break;
            case 2:
                deliveryview.ActiveViewIndex = 1;
                reqftpaddress.Enabled = true;
                reqftpusername.Enabled = true;
                reqftppassword.Enabled = true;
                reqemails.Enabled = false;
                break;
        }
    }
}
