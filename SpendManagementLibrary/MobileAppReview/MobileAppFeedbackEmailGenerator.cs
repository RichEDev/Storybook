namespace SpendManagementLibrary.MobileAppReview
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class MobileAppFeedbackEmailGenerator
    {
        public static bool SendEmailToServiceDesk(ICurrentUserBase user, string feedbackCategory, string feedback, string email, string appVersion, int mobileMetricId)
        {

            cAccountProperties accountProperties = new cAccountSubAccountsBase(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            cAccounts accounts = new cAccounts();
            cAccount account = accounts.GetAccountByID(user.AccountID);

            var emailSender = new EmailSender(accountProperties.EmailServerAddress);

            var fromAddress = "mobileappfeedback@selenity.com";
            var emailAddress = "richard.edwards@selenity.com";
            var subject = "Mobile App Feedback Received";
         



            var message = Get();

            if (message == string.Empty)
            {
               // log error
                return false;
            }

      

            if (email == string.Empty)
            {

                // remove email reply link text from message

                var g = GetStringBetweenCharacters(message, "[EmailReplyStart]", "[EmailReplyEnd]");
                message = message.Replace(g, "");
            }

            //Get data
            message = message.Replace("[FeedbackCategory]", feedbackCategory)
                .Replace("[Feedback]", feedback)
                .Replace("[AppVersion]", appVersion)
                .Replace("[Email]", email)
                .Replace("[EmailReplyStart]", string.Empty)
                .Replace("[EmailReplyEnd]", string.Empty)
                .Replace("[MobileMetricId]", mobileMetricId.ToString())
                .Replace("[CustomerName]", account.companyname)
                .Replace("[EmployeeName]", user.Employee.FullName);

            MailMessage msg = new MailMessage(fromAddress, emailAddress, subject.ToString(), message)
                                  { IsBodyHtml = true };


            EmailSender sender = new EmailSender(accountProperties.EmailServerAddress);

            bool emailSent = sender.SendEmail(msg);

      

      //      emailSender.SendEmail(fromAddress, emailAddress, subject, message);

            return true;
        }


        private static string Get()
        {
            string infoUrl = GlobalVariables.GetAppSetting("MobileAppFeedbackServiceDeskEmailTemplate");

            //build up path to guideline text
            var directoryInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")).Parent;

            if (directoryInfo != null)
            {
                var textpath = string.Format("{0}/expenses{1}", directoryInfo.FullName, infoUrl);
                var t = System.IO.File.ReadAllText(textpath);

                return t;
            }

            return string.Empty;
        }


        public static string GetStringBetweenCharacters(string input, string charFrom, string charTo)
        {
            int posFrom = input.IndexOf(charFrom) + 16;
            if (posFrom != -1) //if found char
            {
                int posTo = input.IndexOf(charTo, posFrom + 1);
                if (posTo != -1) //if found char
                {
                    return input.Substring(posFrom + 1, posTo - posFrom - 1);
                }
            }

            return string.Empty;
        }
    }
}
