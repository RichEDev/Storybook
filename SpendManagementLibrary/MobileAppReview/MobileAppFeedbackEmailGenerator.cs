namespace SpendManagementLibrary.MobileAppReview
{

    using System.IO;
    using System.Net.Mail;
    using System.Web;

    using BusinessLogic.Cache;

    using Common.Logging;

    /// <summary>
    /// The mobile app feedback email generator.
    /// </summary>
    public class MobileAppFeedbackEmailGenerator
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<MobileAppFeedbackEmailGenerator>().GetLogger();

        /// <summary>
        /// Generates an email to the service desk with the submitted mobile app feedback.
        /// </summary>
        /// <param name="user">
        /// An instance of <see cref="ICurrentUserBase"/>
        /// </param>
        /// <param name="feedbackCategory">
        /// The feedback category.
        /// </param>
        /// <param name="feedback">
        /// The feedback.
        /// </param>
        /// <param name="email">
        /// The email of the employee providing feedback.
        /// </param>
        /// <param name="appVersion">
        /// The app version.
        /// </param>
        /// <param name="mobileMetricId">
        /// The mobile metric id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>, whether the action was successful.
        /// </returns>
        public bool SendEmailToServiceDesk(ICurrentUserBase user, string feedbackCategory, string feedback, string email, string appVersion, int mobileMetricId)
        {      
            cAccounts accounts = new cAccounts();
            cAccount account = accounts.GetAccountByID(user.AccountID);
         
            var messageBody = MobileAppFeedbackServiceDeskEmailTemplate();

            if (messageBody == string.Empty)
            {
                Log.Error("The mobile app feedback service desk email template text could not be found.");
                return false;
            }
   
            if (email == string.Empty)
            {
                // remove email reply link text from message
                var replyEmailText = GetStringBetweenEmailReplyTags(messageBody, "[EmailReplyStart]", "[EmailReplyEnd]");
                messageBody = messageBody.Replace(replyEmailText, "");
            }
       
            messageBody = messageBody.Replace("[FeedbackCategory]", feedbackCategory)
                .Replace("[Feedback]", feedback)
                .Replace("[AppVersion]", appVersion)
                .Replace("[Email]", email)
                .Replace("[EmailReplyStart]", string.Empty)
                .Replace("[EmailReplyEnd]", string.Empty)
                .Replace("[MobileMetricId]", mobileMetricId.ToString())
                .Replace("[CustomerName]", account.companyname)
                .Replace("[EmployeeName]", user.Employee.FullName);

            var fromAddress = "mobileappfeedback@selenity.com";
            var emailAddress = "teammobile@selenity.com";
            var subject = "Expenses Mobile feedback received";

            MailMessage msg = new MailMessage(fromAddress, emailAddress, subject, messageBody) { IsBodyHtml = true };

            cAccountProperties accountProperties = new cAccountSubAccountsBase(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            EmailSender sender = new EmailSender(accountProperties.EmailServerAddress);

            return sender.SendEmail(msg); 
        }

        /// <summary>
        /// Gets the mobile app feedback service desk email template body text.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> the template body text.
        /// </returns>
        private string MobileAppFeedbackServiceDeskEmailTemplate()
        {
           
            //build up path to guideline text
            var directoryInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")).Parent;

            if (directoryInfo != null)
            {
                var templatePath =
                    $"{directoryInfo.FullName}/expenses/help_text/MobileAppFeedback/MobileAppReviewServiceDeskEmailTemplate.txt";

                return System.IO.File.ReadAllText(templatePath);     
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the string between email reply tags.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="tagFrom">
        /// The tag from.
        /// </param>
        /// <param name="tagTo">
        /// The tag to.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> of text between the two start and end email reply tags.
        /// </returns>
        public string GetStringBetweenEmailReplyTags(string input, string tagFrom, string tagTo)
        {
            int positionFrom = input.IndexOf(tagFrom) + 16;

            if (positionFrom != -1)
            {
                int positionTp = input.IndexOf(tagTo, positionFrom + 1);

                if (positionTp != -1)
                {
                    return input.Substring(positionFrom + 1, positionTp - positionFrom - 1);
                }

            }

            return string.Empty;
        }
    }
}
