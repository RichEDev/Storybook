namespace Expenses_Reports
{
    using System;
    using System.Data.SqlClient;
    using System.Text;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;

    class cEmails
    {
        public cEmails()
        {

        }

        public void SendOnStopEmail()
        {
            EmailSender sender = new EmailSender("127.0.0.1");
            sender.SendEmail("admin@sel-expenses.com", "support@selenity.com", GetApplicationName() + " Reports has stopped on " + System.Environment.MachineName, "The " + GetApplicationName() + " reports service has stopped on " + System.Environment.MachineName);
        }

        public void sendErrorMail(Exception error, int accountId)
        {

            string companyId = this.GetCompanyID(accountId);
            
            StringBuilder sb = new StringBuilder("An error has been produced trying to cache reports for: " + companyId + "\n\n");
            if (error.InnerException != null)
            {
                sb.Append("Error Message: " + error.InnerException.Message + "\n");
                sb.Append("Stack Trace: " + error.InnerException.StackTrace); ;
            }
            else
            {
                sb.Append("Error Message: " + error.Message + "\n");
                sb.Append("Stack Trace: " + error.StackTrace);
            }

            EmailSender sender = new EmailSender("127.0.0.1");
            sender.SendEmail("errors@sel-expenses.com", "expenses@selenity.com", GetApplicationName() + " Reports Cache Error " + companyId + " on " + System.Environment.MachineName, sb.ToString());
        }

        public void sendErrorMail(Exception error, cReportRequest request)
        {
            string companyid = this.GetCompanyID(request.accountid);

            StringBuilder sb = new StringBuilder();
            string reportname = "Unknown";

            sb.Append("The following error occurred in reports:\n\n");
            if (request.report != null)
            {
                reportname = request.report.reportname;
                sb.Append("ReportID: " + request.report.reportid.ToString() + "\n");
            }

            sb.Append("Report: " + reportname + "\n");
            sb.Append("AccountID: " + request.accountid + "\n");
            sb.Append("CompanyID: " + companyid + "\n");
            sb.Append("EmployeeID: " + request.employeeid + "\n");
            sb.Append("Processed Rows: " + request.ProcessedRows + "\n\n");
            
            if (error.InnerException != null)
            {
                sb.Append("Error Message: " + error.InnerException.Message + "\n");
                sb.Append("Stack Trace: " + error.InnerException.StackTrace);
            }
            else
            {
                sb.Append("Error Message: " + error.Message + "\n");
                sb.Append("Stack Trace: " + error.StackTrace);
            }

            EmailSender sender = new EmailSender("127.0.0.1");
            sender.SendEmail("errors@sel-expenses.com", "expenses@selenity.com", GetApplicationName() + " Report Error: " + reportname, sb.ToString());
        }

        private string GetApplicationName()
        {
            var module = FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>()[GlobalVariables.DefaultModule];
            return module.BrandName;
        }

        private string GetCompanyID(int accountID)
        {
            string companyid = "";
            DBConnection dbcon = new DBConnection(GlobalVariables.MetabaseConnectionString);
            dbcon.sqlexecute.Parameters.AddWithValue("@accountid", accountID);
            using (SqlDataReader reader = dbcon.GetReader("select companyid from registeredusers where accountid = @accountid"))
            {
                while (reader.Read())
                {
                    companyid = reader.GetString(0);
                }
                reader.Close();
                dbcon.sqlexecute.Parameters.Clear();
            }
            return companyid;
        }
    }
}
