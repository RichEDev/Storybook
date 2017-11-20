using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    public interface IScheduler
    {
        int addSchedule(cScheduledReport schedule);
        int updateSchedule(cScheduledReport schedule);
        System.Data.DataTable getGrid(int accountid, int employeeid);
        cScheduledReport getScheduledReportById(int accountId, int scheduleid);
        void deleteSchedule(int accountId, int scheduleid);
        void AddTask(int accountid, int employeeid, int? subaccountid, cTask task);
        void UpdateTask(int accountid, int employeeid, int? subaccountid, cTask task);
        void DeleteTask(int accountid, int employeeid, int? subaccountid, int taskid);
    }

    public enum DeliveryType
    {
        email = 1,
        multipleemail,
        FTP
    }
}
