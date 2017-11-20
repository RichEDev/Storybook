namespace Spend_Management.Expedite
{
    using System;
    using System.Text;
    using System.Configuration;
    using System.Diagnostics;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Expedite;

    /// <summary>
    /// ExpediteEmail class includes implementation of needed for sending emails regarding expedite communications.
    /// </summary>
    public class ExpediteEmail
    {
        #region Public methods for expedite mails
        /// <summary>
        /// Gets the filled template that should be sent to Expedite.
        /// </summary>
        /// <param name="accountId">Account Id of the customer</param>
        /// <param name="topUpRequired">the extra amount that is to be credited in order to mark financial export as executed</param>
        /// <returns>boolean telling sending email is successful</returns>
        public bool SendTopUpEmailNotification(int accountId, decimal topUpRequired)
        {
            try
            { 
                cAccountProperties clsAccountProperties;
                Employee mainAdministrator;
                var clsSubAccounts = new cAccountSubAccounts(accountId);
                clsAccountProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
                var clsEmployees = new cEmployees(accountId);
                mainAdministrator = clsEmployees.GetEmployeeById(clsAccountProperties.MainAdministrator);
                const string FromEmail = "admin@sel-expenses.com";
                const string Subject = "Subject: Expenses Administration: Alert: Insufficient fund in the account";
                var message = new StringBuilder();
                message.Append("<p>Dear " + mainAdministrator.Forename + " " + mainAdministrator.Surname + ",</p>");
                message.Append(
                "Your Expenses account does not have sufficient funds to carry out the pending reimbursements. Requesting you to kindly top-up your account for a minimum of "
                + topUpRequired.ToString("0.00") + " pounds. <br /><br />");
                message.Append("Regards <br />");
                message.Append("Team Expenses");
                return this.SendExpediteEmail(
                    FromEmail,
                    mainAdministrator.EmailAddress,
                    Subject,
                    message.ToString(),
                    clsAccountProperties.EmailServerAddress,
                    true);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Failed to send email\n\n" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Send email to administrator when float falls below fund limit
        /// </summary>
        /// <returns>flag indicating success/failure of notification</returns>
        public bool NotifyAdministratorsOfFloatBelowLimit()
        {
            try
            {
                //get the list of expedite customer details 
                var customersEmailDetails = Funds.GetFundDetailsForExpediteEmail();

                if (customersEmailDetails == null || customersEmailDetails.Count == 0)
                {
                    cEventlog.LogEntry(
                        "Error while retrieving expedite customers details", true,
                        EventLogEntryType.Error);
                    return false;
                }

                //check if the from email is set
                var fromEmail = ConfigurationManager.AppSettings["expediteEmailAddress"];
                if (string.IsNullOrEmpty(fromEmail))
                {
                    cEventlog.LogEntry("From email address is not set", true,EventLogEntryType.Error);
                    return false;
                }

                foreach (var customerDetails in customersEmailDetails)
                {
                    //check if the available fund falls below fund limit
                    if (customerDetails.MinTopUpRequired > 0)
                    {
                        //subject of the email
                        const string Subject =
                        "Subject: Expenses Administration: Alert: Fund balance reached minimum limit";

                        //prepare the body
                        var message = new StringBuilder();
                        message.Append("<p>Dear " + customerDetails.FirstName + ",</p>");
                        message.Append(
                            "The float for payment of expenses for your company has fallen below the fund limit of "
                            + customerDetails.FundLimit.ToString("0.00")
                            + ".  Please arrange a top up of the float. If you have queries then please contact the Expedite team on 01522 507882 or expedite@selenity.com. <br /><br />");
                        message.Append("Regards, <br />");
                        message.Append("The Expedite team");

                        //send the email
                        if (!SendExpediteEmail(fromEmail, customerDetails.AdminEmail, Subject, message.ToString(), customerDetails.EmailServerAddress, true))
                        {
                            cEventlog.LogEntry(string.Format("Couldn't send an email to the customer {0}}",true, customerDetails.AccountId));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                cEventlog.LogEntry(
                    ex.Message,true,
                    EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        #endregion

        #region Private Helper Methods
        /// <summary>
        /// A generic method, which can be called to send emails, using these parameters
        /// </summary>
        /// <param name="fromEmail">Email sent by</param>
        /// <param name="sendingToEmail">Email being sent to</param>
        /// <param name="subject">Subject to the mail</param>
        /// <param name="body">Body of the mail</param>
        /// <param name="emailServerAddress">Server address configured to send mails</param>
        /// <param name="bodyInHtml">whether the body is in html format or not</param>
        /// <returns>returns boolean stating whether sending was successful</returns>
        public bool SendExpediteEmail(string fromEmail, string sendingToEmail, string subject, string body, string emailServerAddress, bool bodyInHtml = false)
        {
            var msg = new System.Net.Mail.MailMessage(fromEmail, sendingToEmail, subject, body)
            {
                IsBodyHtml = bodyInHtml
            };
            var emailSender = new EmailSender(emailServerAddress);
            return emailSender.SendEmail(msg);
        }
        #endregion
    }
}
