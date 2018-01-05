using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;
using SpendManagementLibrary;

/// <summary>
/// A class for sending emails
/// </summary>
public class EmailSender
    {
        /// <summary>
        /// Hostname for the mail server
        /// </summary>
        private readonly string MailServer = string.Empty;

    /// <summary>
    /// A private instance of <see cref="SmtpClient"/>
    /// </summary>
    private SmtpClient smtpClient;

    /// <summary>
    /// Create a new instance of <see cref="EmailSender"/>
    /// </summary>
    /// <param name="mailServerHostname">The address of the mail server.</param>
    public EmailSender(string mailServerHostname)
        {
            this.MailServer = mailServerHostname;
            this.smtpClient = new SmtpClient(this.MailServer);
    }

    /// <summary>
    /// Send an email to a specific email address
    /// </summary>
    /// <param name="sendFrom">The email address to use as the sender</param>
    /// <param name="sendTo">The recipient of the email</param>
    /// <param name="subject">The subject line</param>
    /// <param name="body">The body of the email</param>
    /// <returns>True if the email send to the server.</returns>
    public bool SendEmail(string sendFrom, string sendTo, string subject, string body)
        {
            return this.SendEmail(new MailMessage(sendFrom, sendTo, subject, body));
        }

    /// <summary>
    /// Send an <see cref="MailMessage"/> via the current email server.
    /// </summary>
    /// <param name="message">An instance of <see cref="MailMessage"/>to send</param>
    /// <returns>True if the email send to the server.</returns>
    public bool SendEmail(MailMessage message)
        {
            bool sent;
            try
            {
                this.smtpClient.Send(message);
                sent = true;
            }
            catch (SmtpException exception)
            {
                cEventlog.LogEntry("EmailSender: SendEmail: " + exception.Message, true, EventLogEntryType.Error, cEventlog.ErrorCode.DebugInformation);
                sent = false;
            }

            return sent;
        }
    }
