using System;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Data;

namespace Expenses_Scheduler
{
    /// <summary>
    /// cEmailSender class
    /// </summary>
    public class cEmailSender
    {
        int _nNumBlankNotifies = 0;

        cEmailLog _clslog;
        System.Collections.SortedList _slInvalidRecipients;
        readonly cAccount _cAccount;

        /// <summary>
        /// cEmailSender constructor
        /// </summary>
        /// <param name="account"></param>
        /// <param name="log"></param>
        public cEmailSender(cAccount account, ref cEmailLog log)
        {
            _cAccount = account;
            _clslog = log;
        }

#region properties
        /// <summary>
        /// Current customer account being actioned
        /// </summary>
        public cAccount Account
        {
            get { return _cAccount; }
        }
#endregion

        /// <summary>
        /// Processes any email schedules that are due
        /// </summary>
        public void SendSchedules()
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(Account.accountid));
            const string sql = "SELECT count(*) FROM [email_schedule] WHERE [nextRunDate] <= CONVERT(DateTime,GetDate(), 120)";
            int numScheds = data.getcount(sql);
            if (numScheds == 0)
            {
                return;
            }
            DataSet ds = data.GetDataSet("SELECT * FROM [email_schedule] WHERE [nextRunDate] <= CONVERT(DateTime,GetDate(), 120)");
            _clslog.AddToLog(numScheds.ToString() + " email schedules retrieved for processing.", Account);
            foreach (DataRow dRow in ds.Tables[0].Rows)
            {
                DealWithRow(dRow, Account);
            }
        }

        /// <summary>
        /// Processes an individual schedule that is due
        /// </summary>
        /// <param name="dRow">Database record row of schedule to be actioned</param>
        /// <param name="account">Customer account scheduled to be applied to</param>
        private void DealWithRow(DataRow dRow, cAccount account)
        {
            string emParam, templateFile;

            DBConnection db = new DBConnection(cAccounts.getConnectionString(account.accountid));

            int schedId = (int)dRow["scheduleId"];
            if (dRow["templateId"] == DBNull.Value)
            {
                _clslog.AddToLog("Cannot execute scheduled notification due to missing Email Template. Schedule being skipped", account);
                return;
            }
            int templateId = (int)dRow["templateId"];
            int emailTypeId = (int)dRow["emailType"];
            if (dRow["emailParam"] == DBNull.Value)
            {
                emParam = "";
            }
            else
            {
                emParam = (string)dRow["emailParam"];
            }

            int dbLocationId = (int)dRow["runSubAccountId"];

            string tPath = "";
            string tFilename = "";

            db.sqlexecute.Parameters.AddWithValue("@templateId", templateId);
            using (SqlDataReader reader = db.GetReader("select templatePath, templateFilename from email_templates where templateId = @templateId"))
            {
                while (reader.Read())
                {
                    tPath = reader.GetString(0);
                    tFilename = reader.GetString(1);
                }
                reader.Close();

                templateFile = tFilename != "" ? System.IO.Path.Combine(tPath, tFilename) : "";
            }

            _clslog.AddToLog("Handling Schedule " + schedId + " using Template " + templateFile, account);

            switch ((emailType)emailTypeId)
            {
                case emailType.AuditCleardown:
                    break;

                case emailType.ContractReview:
                    if (emParam != "")
                    {
                        ContractReview(templateFile, Convert.ToInt32(emParam), dbLocationId, ref _slInvalidRecipients);
                    }
                    else
                    {
                        ContractReview(templateFile, 0, dbLocationId, ref _slInvalidRecipients);
                    }
                    break;
                case emailType.LicenceExpiry:
                    LicenceExpiry(templateFile, dbLocationId);
                    break;
                case emailType.OverdueInvoice:
                    InvoiceOverdue(templateFile, dbLocationId);
                    break;
            }

            emailFreq freq = (emailFreq)dRow["emailFrequency"];
            switch (freq)
            {
                case emailFreq.Once:
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@schedId", schedId);
                    db.ExecuteSQL("DELETE FROM [email_schedule] WHERE [scheduleId] = @schedId");

                    _clslog.AddToLog("Schedule " + schedId.ToString() + " set to run once only. Schedule deleted.", account);
                    break;
                default:
                    DateTime tmpCurDateWithRunTime = (DateTime)dRow["nextRunDate"];
                    DateTime nrd = GetNextRunDate(DateTime.Now, tmpCurDateWithRunTime, freq, (int)dRow["frequencyParam"]);

                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@schedId", schedId);
                    db.sqlexecute.Parameters.AddWithValue("@NextRunDate", nrd.ToString("yyyy-MM-dd HH:mm:ss"));
                    //CONVERT(datetime,'" + nrd.ToString("yyyy-MM-dd HH:mm:ss") + "',120)
                    db.ExecuteSQL("UPDATE [email_schedule] SET [nextRunDate] = @NextRunDate WHERE [scheduleId] = @schedId");

                    _clslog.AddToLog("Next Scheduled Run of email notification set to " + nrd.ToString("yyyy-MM-dd HH:mm:ss"), account);
                    break;
            }
        }

        /// <summary>
        /// Issues email notifications for contract reviews for a given subaccount
        /// </summary>
        /// <param name="template">Name of the email template to be used</param>
        /// <param name="emParam">Schedule parameter to look x days ahead</param>
        /// <param name="dbLocationId">Subaccount ID to check records in</param>
        /// <param name="InvalidRecipients">Collection to be populated with any invalid email recipients</param>
        private void ContractReview(string template, int emParam, int dbLocationId, ref System.Collections.SortedList InvalidRecipients)
        {
            DataSet dSet;
            DataSet rcpdSet = null;

            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));
            const string sql = "SELECT [contractNumber],[contractId],[contractDescription],[noticePeriod],[reviewPeriod],[reviewDate],[reviewComplete],[contractKey],[maintenanceType],[maintenancePct],[endDate],[supplier_details].[supplierName] FROM [contract_details] INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [contract_details].[supplierId] WHERE ([reviewComplete] = 0 OR [reviewComplete] IS NULL) AND [reviewDate] <= CONVERT(datetime,DATEADD(day,@emParam,getdate())) AND [Archived] = 'N' AND [contract_details].[subAccountId] = @locId";

            try
            {
                //Console.WriteLine("Fetching contracts up for review...");
                _clslog.AddToLog("Fetching contracts up for review...", Account);
                db.sqlexecute.Parameters.AddWithValue("@locId", dbLocationId);
                db.sqlexecute.Parameters.AddWithValue("@emParam", emParam);
                dSet = db.GetDataSet(sql);
                //Console.WriteLine("Contracts retrieved.");
                _clslog.AddToLog("Contracts retrieved.", Account);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Could not get contracts up for review.");
                _clslog.AddToLog("FAIL - Could not get contracts up for review.", Account);
                return;
            }

            _nNumBlankNotifies = 0;
            if (InvalidRecipients == null)
            {
                InvalidRecipients = new System.Collections.SortedList();
            }
            else
            {
                InvalidRecipients.Clear();
            }

            foreach (DataRow dRow in dSet.Tables[0].Rows)
            {
                try
                {
                    int cid = (int)dRow["ContractId"];
                    if (dRow["ContractKey"] == DBNull.Value)
                    {
                        dRow["ContractKey"] = "No Key Specified";
                    }
                    _clslog.AddToLog("Fetching recipient list for contract " + dRow["ContractKey"] + "...", Account);
                    
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@contractId", cid);
                    db.sqlexecute.Parameters.AddWithValue("@activeSubAccountId", dbLocationId);
                    rcpdSet = db.GetDataSet("SELECT * FROM dbo.GetNotifyRecipients(@contractId, @activeSubAccountId)");

                    _clslog.AddToLog("Got recipient list for contract " + dRow["ContractKey"] + ".", Account);
                }
                catch (Exception ex)
                {
                    _clslog.AddToLog("FAIL - Could not get recipient list. " + ex.ToString(), Account);
                }

                try
                {
                    cModMailer modMailer = new cModMailer(Account, ref _clslog) {RunLocationId = dbLocationId};
                    modMailer.ContractReviewMail(dRow, rcpdSet, template, ref InvalidRecipients, ref _nNumBlankNotifies);
                }
                catch (Exception ex)
                {
                    _clslog.AddToLog("Failed in email module. " + ex.ToString(), Account);
                }
            }

            _clslog.AddToLog("Number of administration notifications for unspecified email addresses : " + InvalidRecipients.Count.ToString(), Account);
            _clslog.AddToLog("Number of contracts without Notification specified : " + _nNumBlankNotifies.ToString(), Account);
        }
        
        /// <summary>
        /// Issues email notifications of licence expiries for a particular subaccount
        /// </summary>
        /// <param name="template">Name of the email template to be used</param>
        /// <param name="dbLocationId">Subaccount Id to check records in</param>
        private void LicenceExpiry(string template, int dbLocationId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));

            try
            {
                db.sqlexecute.Parameters.AddWithValue("@locId", dbLocationId);
                const string sql = "SELECT productLicences.[LicenceId], [LicenceKey], [LicenceType],[Location],[Expiry],productLicences.[RenewalType], productLicences.[NotifyId], [NotifyType], productLicences.[NotifyDays] FROM [productLicences] INNER JOIN productDetails ON productDetails.[ProductId] = productLicences.[ProductId] WHERE productDetails.[subAccountId] = @locId AND [Expiry] IS NOT NULL";

                DataSet ds = db.GetDataSet(sql);

                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    DateTime expiryD = (DateTime)dRow["Expiry"];

                    if (dRow["NotifyDays"] != DBNull.Value)
                    {
                        double takeOff = Double.Parse("-" + dRow["NotifyDays"]);
                        expiryD = expiryD.AddDays(takeOff);
                    }

                    if(expiryD >= DateTime.Now) continue;

                    cModMailer modMailer = new cModMailer(Account, ref _clslog) {RunLocationId = dbLocationId};
                    modMailer.LicenceExpiryMail(dRow, template);
                }
            }
            catch (Exception ex)
            {
                _clslog.AddToLog("Exception occurred during Licence Expiry Email", Account);
                _clslog.AddToLog(ex.Message + "\n\n" + ex.StackTrace, Account);
            }
        }

        /// <summary>
        /// Issues email notifications for invoices that are overdue
        /// </summary>
        /// <param name="template">Name of the template</param>
        /// <param name="dbLocationId">Subaccount Id to check records in</param>
        private void InvoiceOverdue(string template, int dbLocationId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));

            try
            {
                const string sql = "SELECT * FROM [contract_forecastdetails] INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_forecastdetails].[ContractId] WHERE [contract_details].[subAccountId] = @locId AND [contract_details].[Archived] = 'N' AND [paymentDate] <= getdate() ";

                db.sqlexecute.Parameters.AddWithValue("@locId", dbLocationId);
                DataSet ds = db.GetDataSet(sql);

                foreach(DataRow dRow in ds.Tables[0].Rows)
                {
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@contractId", (int)dRow["ContractId"]);
                    db.sqlexecute.Parameters.AddWithValue("@activeSubAccountId", dbLocationId);
                    DataSet dsrcp = db.GetDataSet("SELECT * FROM dbo.GetNotifyRecipients(@contractId, @activeSubAccountId)");

                    cModMailer modMailer = new cModMailer(Account, ref _clslog) {RunLocationId = dbLocationId};
                    modMailer.OverdueInvoiceMail(dRow, dsrcp, template);
                }
            }
            catch(Exception ex)
            {
                _clslog.AddToLog("Exception occurred during Invoice Overdue Email", Account);
                _clslog.AddToLog(ex.Message + "\n\n" + ex.StackTrace, Account);
            }
        }

        /// <summary>
        /// Calculates the next run date for the schedule according to its frequency parameters
        /// </summary>
        /// <param name="curDate">Current Run Date</param>
        /// <param name="lastRunDate">Last Run Date</param>
        /// <param name="freqType">Frequency Type</param>
        /// <param name="freqParam">Frequency Number (type changes dependent on Frequency Type)</param>
        /// <returns></returns>
        public DateTime GetNextRunDate(DateTime curDate, DateTime lastRunDate, emailFreq freqType, int freqParam)
        {
            DateTime nextDate = DateTime.Now;
            DateTime tmpDateTime = new DateTime(curDate.Year, curDate.Month, curDate.Day, lastRunDate.Hour, lastRunDate.Minute, lastRunDate.Second);

            DateTime retDate = curDate;
            try
            {
                switch (freqType)
                {
                    case emailFreq.Daily:
                        _clslog.AddToLog("Frequency type set to 'Daily'", Account);
                        nextDate = tmpDateTime.AddDays(1);
                        break;
                    case emailFreq.Every_n_Days:
                        _clslog.AddToLog("Frequency type set to 'Every " + freqParam.ToString() + " days'", Account);
                        nextDate = tmpDateTime.AddDays(freqParam);
                        break;
                    case emailFreq.MonthlyOnDay:
                        _clslog.AddToLog("Frequency type set to 'Monthly'", Account);
                        nextDate = tmpDateTime.AddMonths(1);
                        break;
                    case emailFreq.MonthlyOnFirstXDay:
                        _clslog.AddToLog("Frequency type set to 'Monthly on 1st X day' = [" + (DayOfWeek)freqParam + "]", Account);


                        for (int x = 1; x <= 7; x++)
                        {
                            string tmpDateStr;
                            if (curDate.Month + 1 > 12)
                            {
                                tmpDateStr = tmpDateTime.Year + 1 + "-01-" + x.ToString("00") + " " + tmpDateTime.Hour.ToString("HH") + ":" + tmpDateTime.Minute.ToString() + ":00";
                            }
                            else
                            {
                                tmpDateStr = tmpDateTime.Year + "-" + (tmpDateTime.Month + 1).ToString("00") + "-" + x.ToString("00") + " " + tmpDateTime.Hour.ToString() + ":" + lastRunDate.Minute.ToString() + ":00";
                            }

                            nextDate = DateTime.Parse(tmpDateStr);
                            if (nextDate.DayOfWeek == (DayOfWeek)freqParam)
                            {
                                // found the 1st occurrence of the required day
                                break;
                            }
                        }
                        break;
                    case emailFreq.Once:
                        // this shouldn't be called for a one off execution
                        break;
                    case emailFreq.Weekly:
                        _clslog.AddToLog("Frequency type set to 'Weekly'", Account);
                        nextDate = tmpDateTime.AddDays(7);
                        break;
                }
                retDate = nextDate;
            }
            catch (Exception ex)
            {
                _clslog.AddToLog("Error occurred obtaining the next scheduled run date", Account);
                _clslog.AddToLog(ex.Message, Account);
            }
            return retDate;
        }
    }
}
