using System;
using SpendManagementLibrary;
using Spend_Management;

namespace Expenses_Scheduler
{
    class cSchedulerSvc : MarshalByRefObject, IScheduler
    {
        #region IScheduler Members

        public int addSchedule(cScheduledReport schedule)
        {
            cScheduledReports clsschedules = new cScheduledReports(schedule.accountid);
            return clsschedules.AddSchedule(schedule);
        }

        public int updateSchedule(cScheduledReport schedule)
        {
            cScheduledReports clsschedules = new cScheduledReports(schedule.accountid);
            return clsschedules.UpdateSchedule(schedule);
        }

        public void deleteSchedule(int accountId, int scheduleid)
        {
            cScheduledReports clsschedules = new cScheduledReports(accountId);
            clsschedules.DeleteSchedule(scheduleid);
        }

        public cScheduledReport getScheduledReportById(int accountId, int scheduleid)
        {
            cScheduledReports clsschedules = new cScheduledReports(accountId);
            return clsschedules.GetScheduledReportById(scheduleid);
        }
        public System.Data.DataTable getGrid(int accountid, int employeeid)
        {
            cScheduledReports clsschedules = new cScheduledReports(accountid);
            return clsschedules.GetGrid(employeeid);
        }
        public void AddTask(int accountid, int employeeid, int? subaccountid, cTask task)
        {
            cTasks tasks = new cTasks(accountid, subaccountid);
            tasks.AddTask(task, employeeid);
        }
        public void UpdateTask(int accountid, int employeeid, int? subaccountid, cTask task)
        {
            cTasks tasks = new cTasks(accountid, subaccountid);
            tasks.UpdateTask(task, employeeid);
        }
        public void DeleteTask(int accountid, int employeeid, int? subaccountid, int taskid)
        {
            cTasks tasks = new cTasks(accountid, subaccountid);
            tasks.DeleteTask(taskid, employeeid);
        }
        #endregion
    }
}
