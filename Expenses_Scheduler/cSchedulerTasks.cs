namespace Expenses_Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Net.Mail;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    using Syncfusion.XlsIO;

    public class cSchedulerTasks
    {
        public delegate void CompleteTasks();

        private readonly int _nAccountId;

        private readonly IDataFactory<IProductModule, Modules> _productModuleFactory =
            FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>();

        public cSchedulerTasks(int accountid)
        {
            _nAccountId = accountid;

            // pre-cache some stuff
            cEmployees customerUsers = new cEmployees(accountid);
			cUserdefinedFields ufields = new cUserdefinedFields(accountid);
        }

        public void DoTasks()
        {
            DBConnection fwDb = new DBConnection(cAccounts.getConnectionString(_nAccountId));
            cAccountSubAccounts subaccs = new cAccountSubAccounts(_nAccountId);

            foreach (KeyValuePair<int, cAccountSubAccount> kvp in subaccs.getSubAccountsCollection())
            {
                cAccountSubAccount subacc = (cAccountSubAccount)kvp.Value;
                cAccountProperties properties = subaccs.getSubAccountById(subacc.SubAccountID).SubAccountProperties;
                string errorFromAddress = (string.IsNullOrEmpty(properties.ErrorEmailFromAddress) ? "scheduler@selenity.com" : properties.ErrorEmailFromAddress);
                string errorEmailAddress = (string.IsNullOrEmpty(properties.ErrorEmailAddress) ? "scheduler@selenity.com" : properties.ErrorEmailAddress);
                EmailSender sender = new EmailSender((string.IsNullOrEmpty(properties.EmailServerAddress) ? "127.0.0.1" : properties.EmailServerAddress));
                //System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient((string.IsNullOrEmpty(properties.EmailServerAddress) ? "127.0.0.1" : properties.EmailServerAddress));

                try
                {
                    Expenses_Scheduler.DiagLog(string.Format("Scheduler : Checking overdue tasks for accountId {0}, subAccount {1}", _nAccountId, subacc.SubAccountID));
                    CheckOverdueTasks(_nAccountId, subacc.SubAccountID);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Scheduler error : CheckOverdueTasks() : " + ex.Message + "\n\n" + ex.StackTrace);
                    MailMessage msg = new MailMessage(errorFromAddress, errorEmailAddress, "Scheduler error : CheckOverdueTasks() on " + Environment.MachineName, ex.Message + "\n" + ex.StackTrace);
                    sender.SendEmail(msg);
                }
                try
                {
                    Expenses_Scheduler.DiagLog(string.Format("Scheduler : Checking late tasks for accountId {0}, subAccount {1}", _nAccountId, subacc.SubAccountID));
                    CheckLateTasks(_nAccountId, subacc.SubAccountID);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Scheduler error : CheckLateTasks() : " + ex.Message + "\n\n" + ex.StackTrace);
                    MailMessage msg = new MailMessage(errorFromAddress, errorEmailAddress, "Scheduler error : CheckLateTasks() on " + Environment.MachineName, ex.Message + "\n" + ex.StackTrace);
                    sender.SendEmail(msg);
                }

                try
                {
                    if (properties.EnableRecharge && properties.AutoUpdateCVRechargeLive)
                    {
                        // if it is 2am, run AnnualRechargeCPCost function to refresh all contract annual values
                        DateTime startdate = new DateTime(1900, 01, 01, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        DateTime enddate = startdate.AddMinutes(15);
                        DateTime triggerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 0, 0);

                        if(triggerTime >= startdate && triggerTime <= enddate)
                        {
                            string acv = "0";
                            if(properties.AutoUpdateAnnualContractValue)
                            {
                                acv = "1";
                            }

                            fwDb.ExecuteSQL("EXEC dbo.UpdateRechargeCPAnnualCost 0," + acv);
                        }
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Scheduler error : Recalc Recharge : " + ex.Message + "\n\n" + ex.StackTrace);
                    MailMessage msg = new MailMessage(errorFromAddress, errorEmailAddress, "Scheduler error : Recalc Recharge on " + Environment.MachineName, ex.Message + "\n" + ex.StackTrace);
                    sender.SendEmail(msg);
                }

                try
                {
                    Expenses_Scheduler.DiagLog(string.Format("Scheduler : Checking pending emails for accountId {0}", _nAccountId));
                    CheckPendingEmails(_nAccountId);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Scheduler error : CheckPendingEmails() : " + ex.Message);
                    MailMessage msg = new MailMessage(errorFromAddress, errorEmailAddress, "Scheduler error : CheckPendingEmails() on " + System.Environment.MachineName, ex.Message + "\n" + ex.StackTrace);
                    sender.SendEmail(msg);
                }

                try
                {
                    Expenses_Scheduler.DiagLog(string.Format("Scheduler : Calling IssueLogonStats for accountId {0}, subAccount {1}", _nAccountId, subacc.SubAccountID));
                    IssueLogonStats(_nAccountId, subacc.SubAccountID, this._productModuleFactory);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Scheduler error : IssueLogonStats() : " + ex.Message + "\n\n" + ex.StackTrace);
                    MailMessage msg = new MailMessage(errorFromAddress, errorEmailAddress, "Scheduler error : IssueLogonStats() on " + Environment.MachineName, ex.Message + "\n" + ex.StackTrace);
                    sender.SendEmail(msg);
                }

                try
                {
                    
                    int monthsToRetain;
                    if (int.TryParse(properties.RetainLabelsTime, out monthsToRetain) && monthsToRetain > 0)
                    {
                        var limitDate = DateTime.Now.AddMonths(-monthsToRetain).Date;
                        fwDb.sqlexecute.Parameters.Clear();
                        fwDb.sqlexecute.Parameters.AddWithValue("@LimitDate", limitDate);
                        fwDb.sqlexecute.Parameters.AddWithValue("@SubAccountID", subacc.SubAccountID);
                        fwDb.sqlexecute.CommandType = CommandType.Text;
                        fwDb.ExecuteSQL(
                            "delete al " +
                            "from addressLabels al " +
                            "where LastUsed < @LimitDate " +
                            "and exists (select * from employees e where al.employeeId = e.employeeid and e.defaultSubAccountId = @SubAccountID) ");
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("SchedulerError: delete old labels and favourites : " + ex.Message +"\n\n" + ex.StackTrace);
                    MailMessage msg = new MailMessage(errorFromAddress, errorEmailAddress, "Scheduler error : delete old labels and favourites, on " + Environment.MachineName, ex.Message + "\n" + ex.StackTrace);
                    sender.SendEmail(msg);

                }
            }
            return;
        }

        private static void IssueLogonStats(int accountid, int? subaccountid, IDataFactory<IProductModule, Modules> productModuleFactory)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
            cAccounts accs = new cAccounts();
            cAccount acc = accs.GetAccountByID(accountid);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            cAccountProperties properties = subaccountid.HasValue ? subaccs.getSubAccountById(subaccountid.Value).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;

            string companyname = acc.companyname;
            if (companyname.Trim() == "")
            {
                companyname = "Unknown Company ID. " + acc.companyid;
            }
         
            // issue logon stats if not processed and is not the current month
            string sql = "SELECT ISNULL(employees.firstname + ' ' + employees.surname,'Unknown') AS [Full Name], employees.username, logon_trace.[employeeId], [logonPeriod],[count] FROM logon_trace INNER JOIN employees ON logon_trace.employeeId = employees.employeeid WHERE employees.username <> 'admin' AND [Processed] = 0 AND DATEPART(month,[logonPeriod]) <> DATEPART(month,getdate()) ORDER BY [logonPeriod], [count] DESC";
            DataSet dset = db.GetDataSet(sql);

            int itemCount = dset.Tables[0].Rows.Count;
            if (itemCount > 0)
            {
                IWorkbook workBook = ExcelUtils.CreateWorkbook(1);
                // format columns
                workBook.Worksheets[0].Range[1, 1, itemCount, 4].ColumnWidth = 15;
                workBook.Worksheets[0].Range[3, 1, 3, 4].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                workBook.Worksheets[0].Range[3, 1, 3, 4].CellStyle.Font.Bold = true;
                
                workBook.Worksheets[0].Name = DateTime.Now.ToLongDateString();
                workBook.Worksheets[0].Range[1, 1].Text = "Logon statistics for " + companyname;
                workBook.Worksheets[0].Range[1, 1].CellStyle.Font.Underline = ExcelUnderline.Single;
                workBook.Worksheets[0].Range[1, 1].CellStyle.Font.Bold = true;

                // output headers
                workBook.Worksheets[0].Range[3, 1].Text = "Full Name";
                workBook.Worksheets[0].Range[3, 2].Text = "User Name";
                workBook.Worksheets[0].Range[3, 3].Text = "Logon Period";
                workBook.Worksheets[0].Range[3, 4].Text = "Logon Count";

                int rowIdx = 3;
                DateTime curDate = new DateTime(1900, 1, 1);

                foreach (DataRow drow in dset.Tables[0].Rows)
                {
                    if ((DateTime)drow["logonPeriod"] != curDate)
                    {
                        rowIdx++;
                        curDate = (DateTime)drow["logonPeriod"];
                    }
                    workBook.Worksheets[0].Range[rowIdx, 1].Text = (string)drow["Full Name"];
                    workBook.Worksheets[0].Range[rowIdx, 2].Text = (string)drow["username"];
                    workBook.Worksheets[0].Range[rowIdx, 3].Text = "[dd/MM/yyyy]";
                    workBook.Worksheets[0].Range[rowIdx, 3].Value = ((DateTime)drow["logonPeriod"]).ToShortDateString();
                    workBook.Worksheets[0].Range[rowIdx, 4].Number = (int)drow["Count"];
                    rowIdx++;
                }

                workBook.Worksheets[0].Range[4, 1, rowIdx, 4].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                string tmpFilename = "temp\\LogonStatus_" + acc.companyid + ".xls";
                if (System.IO.File.Exists(tmpFilename))
                {
                    System.IO.File.Delete(tmpFilename);
                }
                workBook.SaveAs(tmpFilename, ExcelSaveType.SaveAsXLS);

                var module = productModuleFactory[GlobalVariables.DefaultModule];

                // email to SEL
                EmailSender sender = new EmailSender(properties.EmailServerAddress);
                MailMessage mail = new MailMessage((properties.EmailServerFromAddress == String.Empty ? "support@selenity.com" : properties.EmailServerFromAddress), "framework-stats@selenity.com") {Subject = "Monthly Logon Activity for " + acc.companyname, Body = "The latest logon statistics for " + acc.companyname + " are contained in the attached spreadsheet\n\n**THIS IS AN AUTOMATED EMAIL FROM " + module.BrandName.ToUpper() + " - DO NOT REPLY **"};
                mail.Attachments.Add(new Attachment(tmpFilename));

                if (sender.SendEmail(mail))
                {

                    // set processed = 1 for all those issued
                    sql = "UPDATE logon_trace SET [Processed] = 1 WHERE [TraceId] IN (SELECT [TraceId] FROM logon_trace WHERE [Processed] = 0 AND DATEPART(month,[LogonPeriod]) <> DATEPART(month,getdate()))";
                    db.ExecuteSQL(sql);
                }

            }

            return;
        }

        /// <summary>
        ///  If a task in the database is not started, is past its start date and is half way to it's due date
        ///  then send an escalation email for the individual or team leader
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="subaccountid"></param>
        private static void CheckOverdueTasks(int accountid, int? subaccountid)
        {
            cTasks tasks = new cTasks(accountid, subaccountid);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            cAccountProperties properties = subaccountid.HasValue ? subaccs.getSubAccountById(subaccountid.Value).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;
			
            int escalationRepeat = properties.TaskEscalationRepeat;

            Dictionary<int, cTask> notstartedTasks = tasks.GetTasksByStatus(TaskStatus.NotStarted, -1);
            foreach (KeyValuePair<int, cTask> i in notstartedTasks)
            {
                cTask curTask = i.Value;

                if(!curTask.DueDate.HasValue || !curTask.StartDate.HasValue) continue;

                cPendingEmails pendingemail = new cPendingEmails(accountid, null);

                if (curTask.DueDate.Value < DateTime.Today)
                {
                    bool createEscalate = true;
                    if (curTask.TaskEscalatedDate.HasValue)
                    {
                        if ((DateTime.Today - curTask.TaskEscalatedDate.Value).Days < escalationRepeat)
                        {
                            createEscalate = false;
                        }
                    }

                    if (createEscalate)
                    {
                        // task has already passed it's due date and has not been started
                        pendingemail.CreatePendingTaskEmail(PendingMailType.TaskLate, curTask);
                    }
                }
                else
                {
                    // check that none started task hasn't got half way to it's due date
                    TimeSpan taskSpan = curTask.DueDate.Value - curTask.StartDate.Value;
                    int days = taskSpan.Days;
                    TimeSpan halfway = new TimeSpan(((int)days / 2), 0, 0, 0);
                    DateTime escalateDate = curTask.StartDate.Value + halfway;

                    if (escalateDate <= DateTime.Today)
                    {
                        if (curTask.TaskEscalatedDate.HasValue)
                        {
                            if (((DateTime.Today - curTask.TaskEscalatedDate.Value).Days >= escalationRepeat))
                            {
                                // send an overdue email message for the task
                                int pendingid = pendingemail.CreatePendingTaskEmail(PendingMailType.TaskOverdue, curTask);
                            }
                        }
                        else
                        {
                            pendingemail.CreatePendingTaskEmail(PendingMailType.TaskOverdue, curTask);
                        }
                    }
                }
            }
        }

        private static void CheckLateTasks(int accountid, int? subaccountid)
        {
            // if a task in the database is not started, is past its start date and is half way to it's due date
            // then send an escalation email for the individual or team leader
            cTasks tasks = new cTasks(accountid, subaccountid);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            cAccountProperties properties = subaccountid.HasValue ? subaccs.getSubAccountById(subaccountid.Value).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;

            int escalationRepeat = properties.TaskEscalationRepeat;

            Dictionary<int, cTask> startedTasks = tasks.GetTasksByStatus(TaskStatus.InProgress, -1);
            foreach (KeyValuePair<int, cTask> i in startedTasks)
            {
                cTask curTask = (cTask)i.Value;

                if(!curTask.DueDate.HasValue || !curTask.StartDate.HasValue) continue;

                cPendingEmails pendingemail = new cPendingEmails(accountid, null);

                if(curTask.DueDate.Value >= DateTime.Today) continue;

                bool doEscalate = true;

                if (curTask.TaskEscalatedDate.HasValue)
                {
                    // task has has been escalated already and is due for escalation again if false
                    if ((DateTime.Today - curTask.TaskEscalatedDate.Value).Days < escalationRepeat)
                    {
                        doEscalate = false;
                    }
                }

                if (doEscalate)
                {
                    pendingemail.CreatePendingTaskEmail(PendingMailType.TaskLate, curTask);
                }
            }
        }

        /// <summary>
        /// Check for any pending emails that need to be sent
        /// </summary>
        /// <param name="accountid"></param>
        private static void CheckPendingEmails(int accountid)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
            cPendingEmails pendingemails = new cPendingEmails(accountid, null);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            cAccountProperties properties = subaccs.getFirstSubAccount().SubAccountProperties;

            Dictionary<int, cPendingEmail> emails = pendingemails.GetPendingEmails();
            EmailSender sender = new EmailSender((string.IsNullOrEmpty(properties.EmailServerAddress) ? "127.0.0.1" : properties.EmailServerAddress));
            //System.Net.Mail.SmtpClient mclient = new System.Net.Mail.SmtpClient((string.IsNullOrEmpty(properties.EmailServerAddress) ? "127.0.0.1" : properties.EmailServerAddress));

            foreach (KeyValuePair<int, cPendingEmail> i in emails)
            {
                cPendingEmail curEmail = i.Value;
                MailMessage msg = new MailMessage();
                Dictionary<int, string> emailAddresses;

                switch (curEmail.RecipientType)
                {
                    case sendType.employee:
                    case sendType.lineManager:
                    case sendType.itemOwner:
                    case sendType.budgetHolder:
                        emailAddresses = Employee.GetEmailAddresses(accountid, new List<int> { curEmail.RecipientId });

                        if (emailAddresses.ContainsKey(curEmail.RecipientId) && !string.IsNullOrEmpty(emailAddresses[curEmail.RecipientId]))
                        {
                            msg.To.Add(emailAddresses[curEmail.RecipientId]);
                        }
                        break;
                    case sendType.team:
                        cTeams teams = new cTeams(accountid);
					    cTeam recipientTeam = teams.GetTeamById(curEmail.RecipientId);        

                        if (recipientTeam != null)
                        {
                            emailAddresses = Employee.GetEmailAddresses(accountid, recipientTeam.teammembers);
                            
                            foreach (KeyValuePair<int, string> kvp in emailAddresses)
                            {
                                if(string.IsNullOrEmpty(kvp.Value)) continue;

                                msg.To.Add(kvp.Value);
                            }
                        }
                    break;
                }

                if(msg.To.Count == 0) continue;

                try
                {
                    msg.From = new MailAddress(string.IsNullOrEmpty(properties.EmailServerFromAddress) ? "support@selenity.com" : properties.EmailServerFromAddress);
                    msg.Body = curEmail.MailBody;
                    msg.Subject = curEmail.Subject;
                    if (sender.SendEmail(msg))
                    {
                        const string sql = "delete from pending_email where emailId = @emailId";
                        db.sqlexecute.Parameters.Clear();
                        db.sqlexecute.Parameters.AddWithValue("@emailId", curEmail.EmailId);
                        db.ExecuteSQL(sql);
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("Scheduler : cSchedulerTasks : CheckPendingEmails : " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
}
