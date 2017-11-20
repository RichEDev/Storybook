using System;
using System.Text;
using System.Net.Mail;
using SpendManagementLibrary;

namespace Expenses_Scheduler
{
    class cEmails
    {
        EmailSender sender;
     
        public cEmails(string SMTPserver)
        {
            sender = new EmailSender(SMTPserver);
        }

        public void SendOnStopEmail()
        {
            try
            {
                EmailSender sender = new EmailSender("127.0.0.1");
                sender.SendEmail("admin@sel-expenses.com", "support@selenity.com", GetApplicationName() + " Scheduler has stopped on " + System.Environment.MachineName, "The " + GetApplicationName() + " scheduler service has stopped on " + System.Environment.MachineName);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("On stop email not sent (" + ex.Message + ")");
            }
        }

        public void SendErrorMail(string from, string to, cScheduleRequest request, bool includeScheduleData, string errMsg = "", string errStackDump = "", string msgBody = "")
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Unfortunately an error has occurred sending your scheduled report.\n\n");
                if (request.ReportRequest != null)
                {
                    sb.Append("Report: " + request.ReportRequest.report.reportname + "\n");

                    if (includeScheduleData)
                    {
                        sb.Append("AccountID: " + request.AccountID + "\n");
                        sb.Append("ScheduleID: " + request.ScheduleID + "\n");

                        if (request.ReportRequest != null)
                        {
                            sb.Append("Request: " + request.ReportRequest.requestnum + "\n");
                            sb.Append("Processed Rows: " + request.ReportRequest.ProcessedRows + "\n\n");
                        }
                    }
                }

                if (msgBody != "")
                {
                    sb.Append(msgBody);
                }

                sb.Append("\n\nOur technical team has been notified of the error and will work to find an effective solution.\n");
                if (errMsg != "")
                {
                    sb.Append("\nError encountered: " + errMsg + "\n\n");

                    if (errStackDump != "")
                    {
                        sb.Append(errStackDump);
                        sb.Append("\n\n");
                    }
                }

                string subject = "Scheduled Report: ";
                if (request.ReportRequest != null)
                {
                    subject += request.ReportRequest.report.reportname;
                }

                sender.SendEmail(new MailMessage(from, to, subject, sb.ToString()));
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Scheduler failed to send email, error is:\r\n " + ex.Message + " " + ex.StackTrace);
            }
        }

        public bool SendMail(string from, string to, string subject, string body, Attachment attachment)
        {
            bool emailSent = false;
            try
            {
                MailMessage msg = new MailMessage(from, to, subject, body);

                if (attachment != null)
                {
                    msg.Attachments.Add(attachment);
                }

                emailSent = sender.SendEmail(msg);
                cEventlog.LogEntry("Scheduler successfully sent email, to " + to);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Scheduler failed to send email, error is:\r\n " + ex.Message + " " + ex.StackTrace);
            }

            return emailSent;
        }

        private string GetApplicationName()
        {
            cModules clsModules = new cModules();
            cModule reqModule = clsModules.GetModuleByID((int)GlobalVariables.DefaultModule);
            return reqModule.BrandNamePlainText;
        }
    }

    
}
