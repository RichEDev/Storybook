namespace EmailRemainder
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceProcess;
    using System.Text;

    using Spend_Management.Expedite;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Class representing email notifications on fund limits
    /// </summary>
    public class FundLimitEmail
    {
        /// <summary>
        /// Sends an email for expedite customers when the available Fund falls below fund limit .
        /// </summary>
        public void SendEmailForMinimumFundLimit()
        {
            bool enableMinimumFundEmail;
            
            if (!bool.TryParse(ConfigurationManager.AppSettings["EnableMinimumFundEmail"].ToString(CultureInfo.InvariantCulture), out enableMinimumFundEmail))
            {
                enableMinimumFundEmail = false;
            }

            if (enableMinimumFundEmail)
            {
                try
                {
                    var expConnection =
                        new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ToString());
                    var expediteCustomers = expConnection.GetProcDataSet("GetExpediteCustomers");
                    if (expediteCustomers == null || expediteCustomers.Tables[0].Rows == null)
                    {
                        return;
                    }

                    var fromEmail = ConfigurationManager.AppSettings["expediteEmailAddress"];
                    if (string.IsNullOrEmpty(fromEmail))
                    {
                        throw new Exception("From email address not configured");
                    }

                    foreach (DataRow dr in expediteCustomers.Tables[0].Rows)
                    {
                        try
                        {
                            expConnection.sqlexecute.Parameters.Clear();

                            expConnection.sqlexecute.Parameters.AddWithValue(
                                "@accountId",
                                Convert.ToInt32(dr["accountid"].ToString()));

                            DataSet customerFundInfo =
                                expConnection.GetProcDataSet("GetCustomerFundDetailsForExpediteEmail");
                            var customerFundInfoRow = customerFundInfo.Tables[0].Rows[0];
                            var minTopUpRequired =
                                Convert.ToDecimal(customerFundInfoRow["MinTopUpRequired"].ToString());
                            var firstName = customerFundInfoRow["FirstName"].ToString();
                            var fundLimit =
                                Convert.ToDecimal(customerFundInfoRow["FundLimit"].ToString());
                            var emailServerAddress =
                                customerFundInfoRow["EmailServerAddress"].ToString();
                            if (minTopUpRequired > 0)
                            {
                                string adminEmail = customerFundInfoRow["Email"].ToString();

                                // prepare the email body
                                var message = new StringBuilder();
                                const string Subject =
                                    "Subject: Expenses Administration: Alert: Fund balance reached minimum limit";
                                message.Append("<p>Dear " + firstName + ",</p>");
                                message.Append(
                                    "The float for payment of expenses for your company has reached the minimum limit of "
                                    + fundLimit.ToString("0.00")
                                    + ".  Please arrange a top up of the float. If you have queries then please contact the Expedite team on 01522 507882 or expedite@software-europe.com. <br /><br />");
                                message.Append("Regards, <br />");
                                message.Append("The Expedite team");

                                var expediteEmail = new ExpediteEmail();
                                expediteEmail.SendExpediteEmail(
                                    fromEmail,
                                    adminEmail,
                                    Subject,
                                    message.ToString(),
                                    emailServerAddress,
                                    true);
                            }
                        }
                        catch (Exception ex)
                        {
                            var serviceBase = new ServiceBase();
                            serviceBase.EventLog.WriteEntry(
                                string.Format("Couldn't send an email to the customer {0} error message {1}", Convert.ToInt32(dr["accountid"].ToString()), ex.Message),
                                EventLogEntryType.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry(string.Format("Scheduler : SendMinimumFundLimitEmial : Error {0}\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }
    }
}
