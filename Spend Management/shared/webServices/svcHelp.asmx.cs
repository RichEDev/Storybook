using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using System.Configuration;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcHelp
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class svcHelp : System.Web.Services.WebService
    {
        // NEW SP METHOD
        //[WebMethod(EnableSession=true)]
        //[ScriptMethod]
        //public int SaveArticle(int articleID, int productID, int productAreaID, int queryTypeID, string question, string answer)
        //{
        //    //Validate input
        //    //Create a knowledgebase article for that company (subaccount?)
        //    //Hide cGridNew
        //    // update to include subaccount?
        //    CurrentUser currentUser = cMisc.GetCurrentUser();

        //    svcHelpAndSupport.svcHelpAndSupport helpService = new svcHelpAndSupport.svcHelpAndSupport();
        //    return helpService.saveArticle(currentUser.AccountID, articleID, svcHelpAndSupport.ArticleType.FAQ.ToString(), "customer created", productID, productAreaID, queryTypeID, question, string.Empty, answer, null, true);
        //}

        // REMOVE AND REPLACE WITH ABOVE WHEN NEW SP READY
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveArticle(int articleID, int productAreaID, int queryTypeID, string question, string answer)
        {
            //Validate input
            //Create a knowledgebase article for that company (subaccount?)
            //Hide cGridNew
            // update to include subaccount?
            CurrentUser currentUser = cMisc.GetCurrentUser();

            svcHelpAndSupport.svcHelpAndSupport helpService = new svcHelpAndSupport.svcHelpAndSupport();
            // find the Spend Management Module we're using
            int helpProdID = 0;
            svcHelpAndSupport.cProduct[] lstKbaProducts = helpService.getProducts();
            foreach (svcHelpAndSupport.cProduct product in lstKbaProducts)
            {
                if (product.ProductName == currentUser.CurrentActiveModule.ToString())
                {
                    helpProdID = product.ProductID;
                    break;
                }
            }
            return helpService.saveArticle(currentUser.AccountID, articleID, svcHelpAndSupport.ArticleType.FAQ.ToString(), "customer created", helpProdID, productAreaID, queryTypeID, question, string.Empty, answer, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="missingArea"></param>
        /// <param name="message"></param>
        /// <returns>1 = Worked, -1 = Failed to send, -2 = no main admin email address</returns>
        [WebMethod(EnableSession = true)]
        public bool ReportEllision(string missingArea, string message) // WHEN NEW SP READY - RETURN TYPE SHOULD BE INT
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cModules clsModules = new cModules();
            string productName = clsModules.GetModuleByID((int)currentUser.CurrentActiveModule).BrandNamePlainText;
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            cEmployees clsemployees = new cEmployees(currentUser.AccountID);
            Employee reqemp = clsemployees.GetEmployeeById(currentUser.EmployeeID);

            string changes = reqemp.Title + " " + reqemp.Forename + " " + reqemp.Surname + ", with username " + reqemp.Username + ", has reported some missing or incorrect information in " + productName + ".\n";
            changes += "Concerning the product area \"" + missingArea + "\" - the user submitted the following message.\n\n";
            changes += message;

            Employee mainemp = clsemployees.GetEmployeeById(reqProperties.MainAdministrator);

            if (mainemp != null && mainemp.EmailAddress != null)
            {
                EmailSender sender = new EmailSender(reqProperties.EmailServerAddress);
                string fromAddress = "";

                string subject = "Some information is incorrect in " + productName + ".";

                if (reqProperties.SourceAddress == 1)
                {
                    fromAddress = (reqProperties.EmailAdministrator == "" ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
                }
                else
                {
                    if (reqemp.EmailAddress != "")
                    {
                        fromAddress = reqemp.EmailAddress;
                    }
                    else
                    {
                        //If no email address set then send from admin
                        fromAddress = (reqProperties.EmailAdministrator == "" ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
                    }
                }

                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(fromAddress, mainemp.EmailAddress, subject, changes);

                //    try
                //    {
                //        mserver.Send(msg);
                //    }
                //    catch
                //    {
                //        return -1;
                //    }
                //}
                //else
                //{
                //    return -2;
                //}

                //return 1;

                // REMOVE AND REPLACE WITH ABOVE WHEN NEW SP READY
                if (sender.SendEmail(msg) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public bool EmailKBA(int articleID)
        {
            if (articleID > 0)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
                cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                cEmployees clsemployees = new cEmployees(currentUser.AccountID);

                EmailSender sender = new EmailSender(reqProperties.EmailServerAddress);

                string subject = "Requested Knowledge Base Article";
                string body = "<html><body>" + GetKBAHTML(articleID) + "</body></html>";
                string fromAddress = "";

                if (reqProperties.SourceAddress == 1)
                {
                    fromAddress = (reqProperties.EmailAdministrator == "" ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
                }
                else
                {
                    if (currentUser.Employee.EmailAddress != "")
                    {
                        fromAddress = currentUser.Employee.EmailAddress;
                    }
                    else
                    {
                        //If no email address set then send from admin
                        fromAddress = (reqProperties.EmailAdministrator == "" ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
                    }
                }

                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(fromAddress, currentUser.Employee.EmailAddress, subject, body);
                msg.IsBodyHtml = true;

                if (sender.SendEmail(msg) == false)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        [WebMethod(EnableSession = true)]
        public bool SubmitKBAFeedback(int articleID, string articleTitle, string feedbackText, int ratingValue)
        {
            if (articleID > 0)
            {
                // NEW SP METHOD
                //string spAuthName = ConfigurationManager.AppSettings["spAuthName"];
                //string spAuthPassword = ConfigurationManager.AppSettings["spAuthPassword"];

                //CurrentUser currentUser = cMisc.GetCurrentUser();
                //svcHelpAndSupportAPI.svcSupportPortalAPI supportService = new svcHelpAndSupportAPI.svcSupportPortalAPI();

                //if (supportService.submitArticleFeedBack(spAuthName, spAuthPassword, currentUser.EmployeeID, currentUser.AccountID, articleID, ratingValue, feedbackText) == 0)
                //{
                //    return true;
                //}

                // REMOVE AND REPLACE WITH ABOVE WHEN NEW SP READY
                CurrentUser currentUser = cMisc.GetCurrentUser();
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
                cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                EmailSender sender = new EmailSender(reqProperties.EmailServerAddress);

                string subject = "Feedback Submitted for a Help Section Article";
                string body = "<html><body style=\"font-size: 10pt; font-family: Arial, sans-serif; color: #333333; background-color:#cccccc;\">The following feedback comments were submitted regarding the article: <a href=\"http://help.software-europe.co.uk/viewArticle.aspx?id=" + articleID.ToString() + "\">" + articleTitle + "</a><br />\n<br />\n<strong>\"</strong>" + feedbackText + "<strong>\"</strong><br />\n<br />\nand gave it a rating value of: <strong>" + ratingValue.ToString() + "</strong></body></html>";

                string fromAddress = (reqProperties.EmailAdministrator == "" ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);

                if (currentUser.Employee.EmailAddress != "")
                {
                    fromAddress = currentUser.Employee.EmailAddress;
                }

                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(fromAddress, "helpfeedback@software-europe.co.uk", subject, body);
                msg.IsBodyHtml = true;

                if (sender.SendEmail(msg) == false)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the HTML version of an article by id
        /// </summary>
        /// <param name="articleID">Knowledgebase Article ID</param>
        /// <returns>full article HTML</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetKBAHTML(int articleID)
        {
            svcHelpAndSupport.svcHelpAndSupport help = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cKnowledgeBaseArticle kba = help.getKnowledgeBaseArticleByID(articleID);

            switch (kba.ArticleType)
            {
                case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
                    return ((Spend_Management.svcHelpAndSupport.cWalkthrough)kba).HTML;
                case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
                    return ((Spend_Management.svcHelpAndSupport.cFAQ)kba).HTML;
                case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
                    return ((Spend_Management.svcHelpAndSupport.cOnlineDemo)kba).HTML;
                default:
                    return kba.ToString();
            }
        }

        /// <summary>
        /// Get the HTML version of an article by id without styling
        /// </summary>
        /// <param name="articleID">Knowledgebase Article ID</param>
        /// <returns>basic article HTML</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetKBAHTMLNoStyle(int articleID)
        {
            svcHelpAndSupport.svcHelpAndSupport help = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cKnowledgeBaseArticle kba = help.getKnowledgeBaseArticleByID(articleID);

            switch (kba.ArticleType)
            {
                case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
                    return ((Spend_Management.svcHelpAndSupport.cWalkthrough)kba).HTMLNoStyle;
                case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
                    return ((Spend_Management.svcHelpAndSupport.cFAQ)kba).HTMLNoStyle;
                case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
                    return ((Spend_Management.svcHelpAndSupport.cOnlineDemo)kba).HTMLNoStyle;
                default:
                    return kba.ToString();
            }
        }

        /// <summary>
        /// Submit a query to the Support Portal
        /// </summary>
        /// <param name="productID">Should be passed through from the codebehind if coming from the product itself</param>
        /// <param name="productAreaID">Chosen from the dropdown generated by the support portal service</param>
        /// <param name="queryTypeID">Chosen from the dropdown generated by the support portal service</param>
        /// <param name="queryText">User's query text</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] SubmitHelpQuery(int? productID, int? productAreaID, int? queryTypeID, string queryText)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            svcHelpAndSupport.svcHelpAndSupport help = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles = help.searchForArticles(productID, productAreaID, queryTypeID, queryText, currentUser.AccountID);

            return formatKBAcGridNew(lstArticles);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetAllKnowledgebaseArticles(int? productID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            svcHelpAndSupport.svcHelpAndSupport help = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles = help.GetAllArticles(currentUser.AccountID, productID);

            return formatKBAcGridNew(lstArticles);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetEditableKnowledgebaseArticles(int? productID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            svcHelpAndSupport.svcHelpAndSupport help = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles = help.GetAllArticles(currentUser.AccountID, productID);

            return formatKBAcGridNew(lstArticles);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetArticlesByType(int? productID, string articleType)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            svcHelpAndSupport.ArticleType eArticleType = new Spend_Management.svcHelpAndSupport.ArticleType();
            eArticleType = (Spend_Management.svcHelpAndSupport.ArticleType)Enum.Parse(typeof(Spend_Management.svcHelpAndSupport.ArticleType), articleType);
            svcHelpAndSupport.svcHelpAndSupport help = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles = help.GetArticlesByType(currentUser.AccountID, productID, eArticleType);

            return formatKBAMenuList(lstArticles);
        }

        // NEW SUPPORT PORTAL METHOD
        //    [WebMethod(EnableSession = true)]
        //    [ScriptMethod]
        //    public object[] CreateSupportPortalTicket(string subject, string description, int? productAreaId, SupportPortalAction eSupportPortalAction)
        //    {
        //        string spAuthName = ConfigurationManager.AppSettings["spAuthName"];
        //        string spAuthPassword = ConfigurationManager.AppSettings["spAuthPassword"];
        //        string retmsg = "Failed to create support request. Please contact your administrator";

        //        CurrentUser currentUser = cMisc.GetCurrentUser();

        //        if (currentUser.Employee != null) // && currentUser.Employee.SupportPortalAccountID != null && currentUser.Employee.SupportPortalPassword != string.Empty)
        //        {
        //            svcHelpAndSupportAPI.svcSupportPortalAPI supportService = new svcHelpAndSupportAPI.svcSupportPortalAPI();
        //            if (!supportService.userExists(spAuthName, spAuthPassword, currentUser.AccountID, currentUser.EmployeeID, currentUser.Employee.email))
        //            {
        //                int spId = supportService.createUser(spAuthName, spAuthPassword, currentUser.AccountID, currentUser.EmployeeID, currentUser.Employee.firstname, currentUser.Employee.surname, currentUser.Employee.email, currentUser.Employee.position, currentUser.Employee.telno);

        //                if (spId == -1)
        //                {
        //                    // creation of support portal user failed
        //                    return new object[] { 0, "Failed to create new support portal user" };
        //                }
        //            }

        //            cModules clsModules = new cModules();
        //            string productName = clsModules.GetModuleByID((int)currentUser.CurrentActiveModule).BrandNamePlainText;

        //            subject = subject + " submitted from " + productName + " help and support";

        //            // this will need getting dynamically - but coded for now
        //            int spProductID = 0;
        //            switch (currentUser.CurrentActiveModule)
        //            {
        //                case Modules.expenses:
        //                    spProductID = 1;
        //                    break;
        //                case Modules.contracts:
        //                    spProductID = 2;
        //                    break;
        //                default:
        //                    return new object[] { 0, "Failed to identify the product you are seeking support for" };
        //            }

        //            int ticketID = 0;
        //            switch (eSupportPortalAction)
        //            {
        //                case SupportPortalAction.HelpRequest:
        //                    ticketID = supportService.createTicket(spAuthName, spAuthPassword, currentUser.EmployeeID, currentUser.AccountID, spProductID, productAreaId, null, null, subject, description);
        //                    break;
        //                case SupportPortalAction.SuggestEnhancement:
        //                    ticketID = supportService.suggestAnEnhancement(spAuthName, spAuthPassword, currentUser.EmployeeID, currentUser.AccountID, description, spProductID, productAreaId);
        //                    break;
        //                default:
        //                    break;
        //            }

        //            if (ticketID > 0)
        //            {
        //                return new object[] { 1, "Your support request has been created successfully. Active support tickets can be viewed in the My Tickets screen in Help and Support" };
        //            }
        //            else
        //            {
        //                switch (ticketID)
        //                {
        //                    case -1:
        //                        retmsg = "Support request failed. Your company is not currently set up with the support service. Please report this to Software Europe for resolution";
        //                        break;
        //                    case -2:
        //                        retmsg = "Support request failed. Your company does not have a support provider set. Please report this to Software Europe for resolution";
        //                        break;
        //                    case -3:
        //                        retmsg = "Support request failed. The problem category select could not be found. Please report this to Software Europe for resolution";
        //                        break;
        //                    case -4:
        //                        retmsg = "Support request failed. Your email address already exists in the support service. Please report this to Software Europe for resolution";
        //                        break;
        //                    case -5:
        //                        retmsg = "Support request failed. Authentication failed while making the support request. Please report this to Software Europe for resolution";
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //        }
        //        return new object[] {0, retmsg};
        //    }


        // REMOVE AND REPLACE WITH ABOVE WHEN NEW SP READY
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool CreateSupportPortalTicket(string subject, string description)
        {
            cModules clsmodules = new cModules();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string productName = clsmodules.GetModuleByID((int)currentUser.CurrentActiveModule).BrandNamePlainText;
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            string fromAddress = (reqProperties.EmailAdministrator == "" ? "admin@sel-expenses.com" : reqProperties.EmailAdministrator);
            EmailSender sender = new EmailSender(string.IsNullOrEmpty(reqProperties.EmailServerAddress) ? "127.0.0.1" : reqProperties.EmailServerAddress);

            string message = "Company: " + currentUser.Account.companyname + "\n";
            message += "User Details: " + currentUser.Employee.Title + " " + currentUser.Employee.Forename + " " + currentUser.Employee.Surname + ", with username " + currentUser.Employee.Username + "\n";
            message += "Module name: " + productName + "\n";
            message += description;

            string body = "<html><body style=\"font-size: 10pt; font-family: Arial, sans-serif; color: #333333; background-color:#cccccc;\">" + message.Replace("\n", "<br />") + "</body></html>";
            subject += " submitted from " + productName + " help and support";

            if (reqProperties.SourceAddress == 1)
            {
                fromAddress = "admin@sel-expenses.com";
            }
            else
            {
                if (currentUser.Employee.EmailAddress != "")
                {
                    fromAddress = currentUser.Employee.EmailAddress;
                }
                else
                {
                    //If no email address set then send from admin
                    fromAddress = "admin@sel-expenses.com";
                }
            }

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(fromAddress, "support@software-europe.co.uk", subject, body);
            msg.IsBodyHtml = true;

            if (sender.SendEmail(msg) == false)
            {
                return false;
            } 
            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool SuggestEnhancement(string area, string description)
        {
            /*
             * not to be used till CRM license obtained
             * 
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string subject = "Enhancement Request created from " + currentUser.CurrentActiveModule.ToString() + " Help and Support section by user '" + currentUser.Employee.username + "' on '" + currentUser.Account.companyname + "'.";
            string message = "Product Area: " + area + "\n\nSuggestion:\n" + description;
            svcHelpAndSupport.svcHelpAndSupport helpservice = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            return helpservice.CreateTicket(subject, message);*/
            return false;
        }

        //public string formatKBAGrid(svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles)
        //{
        //    StringBuilder cGridNewText = new StringBuilder();
        //    string sRow = "row1";
        //    string sArticleType;
        //    string sArticleID;
        //    string sArticleTitle;
        //    string sArticlePA;
        //    string sArticleQT;

        //    cGridNewText.Append("<table class=\"cGrid\">");
        //    if (lstArticles != null && lstArticles.Length > 0)
        //    {
        //        cGridNewText.Append("<tr><th>Type</th><th>Title</th><th>Product Area</th><th>Query Type</th></tr>");
        //        foreach (svcHelpAndSupport.cKnowledgeBaseArticle kba in lstArticles)
        //        {
        //            switch (kba.ArticleType)
        //            {
        //                case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
        //                    sArticleTitle = ((svcHelpAndSupport.cFAQ)kba).Question;
        //                    sArticleType = "<img alt=\"Frequently Asked Question\" src=\"" + appPath + "/shared/images/icons/24/plain/question_and_answer.png\" />";
        //                    break;
        //                case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
        //                    sArticleTitle = ((svcHelpAndSupport.cWalkthrough)kba).Title;
        //                    sArticleType = "<img alt=\"Walkthrough Guide\" src=\"" + appPath + "/shared/images/icons/24/plain/step.png\" />";
        //                    break;
        //                case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
        //                    sArticleTitle = ((svcHelpAndSupport.cOnlineDemo)kba).Title;
        //                    sArticleType = "<img alt=\"Video Demonstration\" src=\"" + appPath + "/shared/images/icons/24/plain/plasma-tv.png\" />";
        //                    break;
        //                default:
        //                    sArticleTitle = "(No Title)";
        //                    sArticleType = kba.ArticleType.ToString();
        //                    break;
        //            }

        //            sArticleID = kba.ArticleID.ToString();
        //            sArticlePA = kba.ProductArea.AreaName;
        //            sArticleQT = kba.QueryType.QueryTypeName;

        //            cGridNewText.Append("<tr><td class=\"" + sRow + "\">" + sArticleType + "</td><td class=\"" + sRow + "\"><a href=\"javascript:OpenKnowledgebaseArticleID(" + sArticleID + ");\">" + sArticleTitle + "</a></td><td class=\"" + sRow + "\">" + sArticlePA + "</td><td class=\"" + sRow + "\">" + sArticleQT + "</td></tr>");

        //            sRow = (sRow == "row1") ? "row2" : "row1";
        //        }
        //    }
        //    else
        //    {
        //        cGridNewText.Append("<tr><th>No Results</th></tr>");
        //        cGridNewText.Append("<tr><td class=\"" + sRow + "\" style=\"padding: 20px;\">There were no articles that matched your enquiry. Consider changing the product area or query type to \"[None]\" to broaden your query and then retry the search.<br /><br />Alternatively, if your search is proving fruitless, you can submit your query to the helpdesk team using the appropriate button on the form.</td></tr>");
        //    }
        //    cGridNewText.Append("</table>");


        //    return cGridNewText.ToString();
        //}

        public string[] formatKBAcGridNew(svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var clsFields = new cFields(currentUser.AccountID);
            var clsTables = new cTables(currentUser.AccountID);
            cTable tbl = clsTables.GetTableByID(new Guid("314a8e7e-470a-430f-85be-9a1a928609d5"));

            var lstColumns = new List<cNewGridColumn>();
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("58A5861A-DE2F-4C1E-8C65-D2C38B81CA05"))));//articleID
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("8EE181CE-A67B-4099-BAB2-78F3B7D2F96A"))));//type
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("65EB4A84-3303-4EC2-9777-294995FA7FE0"))));//title
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("0670EDD9-B163-4CC7-9CB2-677C4CD024BB"))));//productarea
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("16DE32E1-0544-42B1-A695-D80D693470D4"))));//querytype

            var ds = new System.Data.DataSet { DataSetName = "Knowledge Base Articles" };
            var dt = new System.Data.DataTable { TableName = "Knowledge Base Articles" };
            dt.Columns.Add("articleID", typeof(int));
            dt.Columns.Add("Type", typeof(svcHelpAndSupport.ArticleType));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Product Area", typeof(string));
            dt.Columns.Add("Query Type", typeof(string));

            foreach (svcHelpAndSupport.cKnowledgeBaseArticle kba in lstArticles)
            {
                switch (kba.ArticleType)
                {
                    case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
                        dt.Rows.Add(new object[5] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cFAQ)kba).Question, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName });
                        break;
                    case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
                        dt.Rows.Add(new object[5] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cWalkthrough)kba).Title, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName });
                        break;
                    case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
                        dt.Rows.Add(new object[5] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cOnlineDemo)kba).Title, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName });
                        break;
                    default:
                        break;
                }
            }
            ds.Tables.Add(dt);

            cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "tblKBAGrid", tbl, lstColumns, ds);

            grid.KeyField = "articleID";
            grid.EmptyText = "No Items";
            grid.enabledeleting = false;
            grid.enableupdating = false;
            grid.enablepaging = true;
            grid.pagesize = 10;
            grid.showfooters = true;
            grid.showheaders = true;
            grid.DisplayFilter = true;

            grid.getColumnByName("articleID").hidden = true;
            grid.SortedColumn = grid.getColumnByName("Title");

            ((cFieldColumn)grid.getColumnByName("Type")).addValueListItem((int)svcHelpAndSupport.ArticleType.FAQ, "<img alt=\"Frequently Asked Question\" title=\"Frequently Asked Question\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/question_and_answer.png") + "\" />");
            ((cFieldColumn)grid.getColumnByName("Type")).addValueListItem((int)svcHelpAndSupport.ArticleType.Walkthrough, "<img alt=\"Walkthrough Guide\" title=\"Walkthrough Guide\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/step.png") + "\" />");
            ((cFieldColumn)grid.getColumnByName("Type")).addValueListItem((int)svcHelpAndSupport.ArticleType.OnlineDemo, "<img alt=\"Video Demonstration\" title=\"Video Demonstration\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/plasma-tv.png") + "\" />");

            grid.addEventColumn("viewKBA", "View", "javascript:OpenKnowledgebaseArticleID({articleID},{Type},'{Title}');", "View Article");

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        public string formatKBAMenuList(svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles)
        {
            cMasterMenu menu = new cMasterMenu();
            string sArticleType;
            string sArticleID;
            string sArticleTitle;
            string sArticlePA;
            string sArticleQT;
            string sArticleDescription;

            if (lstArticles != null && lstArticles.Length > 0)
            {
                foreach (svcHelpAndSupport.cKnowledgeBaseArticle kba in lstArticles)
                {
                    switch (kba.ArticleType)
                    {
                        case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
                            sArticleTitle = ((svcHelpAndSupport.cFAQ)kba).Question;
                            sArticleType = "question_and_answer";
                            sArticleDescription = ((svcHelpAndSupport.cFAQ)kba).Answer;
                            break;
                        case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
                            sArticleTitle = ((svcHelpAndSupport.cWalkthrough)kba).Title;
                            sArticleType = "step";
                            sArticleDescription = ((svcHelpAndSupport.cWalkthrough)kba).Description;
                            break;
                        case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
                            sArticleTitle = ((svcHelpAndSupport.cOnlineDemo)kba).Title;
                            sArticleType = "plasma-tv";
                            sArticleDescription = ((svcHelpAndSupport.cOnlineDemo)kba).Description;
                            break;
                        default:
                            sArticleTitle = "(No Title)";
                            sArticleType = "error";
                            sArticleDescription = "error";
                            break;
                    }

                    sArticleID = kba.ArticleID.ToString();
                    sArticlePA = kba.ProductArea.AreaName;
                    sArticleQT = kba.QueryType.QueryTypeName;

                    menu.AddMenuItem(sArticleType, 24, sArticleTitle, sArticleDescription, "javascript:OpenKnowledgebaseArticleID(" + sArticleID + ");", "png");
                }
            }

            return menu.CreateMenuHTML(1);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool ApproveArticle(int articleID, bool approve)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            svcHelpAndSupport.svcHelpAndSupport helpService = new svcHelpAndSupport.svcHelpAndSupport();
            return helpService.ApproveAccountArticle(currentUser.AccountID, articleID, approve);
        }

        // NEW SUPPORT PORTAL METHOD
        //[WebMethod(EnableSession = true)]
        //[ScriptMethod]
        //public int DeleteArticle(int articleID, int productID)
        //{
        //    //Validate input
        //    //Create a knowledgebase article for that company (subaccount?)
        //    //Hide cGridNew
        //    // update to include subaccount?
        //    CurrentUser currentUser = cMisc.GetCurrentUser();

        //    svcHelpAndSupport.svcHelpAndSupport helpService = new svcHelpAndSupport.svcHelpAndSupport();
        //    return helpService.DeleteArticle(currentUser.AccountID, articleID, productID);
        //}

        // REMOVE AND REPLACE WITH ABOVE WHEN NEW SP READY
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteArticle(int articleID)
        {
            //Validate input
            //Create a knowledgebase article for that company (subaccount?)
            //Hide cGridNew
            // update to include subaccount?
            CurrentUser currentUser = cMisc.GetCurrentUser();

            svcHelpAndSupport.svcHelpAndSupport helpService = new svcHelpAndSupport.svcHelpAndSupport();
            // find the Spend Management Module we're using
            int helpProdID = 0;
            svcHelpAndSupport.cProduct[] lstKbaProducts = helpService.getProducts();
            foreach (svcHelpAndSupport.cProduct product in lstKbaProducts)
            {
                if (product.ProductName == currentUser.CurrentActiveModule.ToString())
                {
                    helpProdID = product.ProductID;
                    break;
                }
            }
            return helpService.DeleteArticle(currentUser.AccountID, articleID, helpProdID);
        }

        // NEW SUPPORT PORTAL METHOD
        //    [WebMethod(EnableSession = true)]
        //    public string RefreshKnowledgeBaseArticlesGrid(int productID)
        //    {
        //        CurrentUser currentUser = cMisc.GetCurrentUser();

        //        svcHelpAndSupport.svcHelpAndSupport helpService = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
        //        svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles = helpService.GetAccountArticles(currentUser.AccountID, productID);

        //        cTables clsTables = new cTables(currentUser.AccountID);
        //        cTable tbl = clsTables.getTableById(new Guid("314a8e7e-470a-430f-85be-9a1a928609d5"));
        //        cFields clsFields = new cFields(currentUser.AccountID);
        //        //SpendManagementLibrary.cTable tbl = new SpendManagementLibrary.cTable(new Guid(), "KnowledgeBaseArticles", 1, false, "Knowledgebase Articles", new Guid(), false, new Guid(), DateTime.Now, false, false, false, new Guid());

        //        List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();
        //        lstColumns.Add(new cNewGridColumn(clsFields.getFieldByTableAndFieldName("supportPortal_knowledgeBaseArticles", "articleID")));
        //        lstColumns.Add(new cNewGridColumn(clsFields.getFieldByTableAndFieldName("supportPortal_knowledgeBaseArticles", "Type")));
        //        lstColumns.Add(new cNewGridColumn(clsFields.getFieldByTableAndFieldName("supportPortal_knowledgeBaseArticles", "Title")));
        //        lstColumns.Add(new cNewGridColumn(clsFields.getFieldByTableAndFieldName("supportPortal_knowledgeBaseArticles", "Product Area")));
        //        lstColumns.Add(new cNewGridColumn(clsFields.getFieldByTableAndFieldName("supportPortal_knowledgeBaseArticles", "Query Type")));
        //        lstColumns.Add(new cNewGridColumn(clsFields.getFieldByTableAndFieldName("supportPortal_knowledgeBaseArticles", "Published")));

        //        System.Data.DataSet ds = new System.Data.DataSet();
        //        ds.DataSetName = "Knowledge Base Articles";
        //        System.Data.DataTable dt = new System.Data.DataTable();
        //        dt.TableName = "Knowledge Base Articles";
        //        dt.Columns.Add("articleID", typeof(int));
        //        dt.Columns.Add("Type", typeof(svcHelpAndSupport.ArticleType));
        //        dt.Columns.Add("Title", typeof(string));
        //        dt.Columns.Add("Product Area", typeof(string));
        //        dt.Columns.Add("Query Type", typeof(string));
        //        dt.Columns.Add("Published", typeof(int));

        //        int pub = 0;
        //        foreach (svcHelpAndSupport.cKnowledgeBaseArticle kba in lstArticles)
        //        {
        //            pub = (kba.Approved == true) ? 1 : 0;
        //            switch (kba.ArticleType)
        //            {
        //                case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
        //                    dt.Rows.Add(new object[6] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cFAQ)kba).Question, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName, pub });
        //                    break;
        //                case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
        //                    dt.Rows.Add(new object[6] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cWalkthrough)kba).Title, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName, pub });
        //                    break;
        //                case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
        //                    dt.Rows.Add(new object[6] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cOnlineDemo)kba).Title, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName, pub });
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        ds.Tables.Add(dt);

        //        cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "tblKBAGrid", tbl, lstColumns, ds);

        //        grid.KeyField = "articleID";
        //        grid.EmptyText = "No Items";
        //        grid.enabledeleting = true;
        //        grid.deletelink = "javascript:DeleteArticle({articleID});";
        //        grid.enableupdating = true;
        //        grid.editlink = "aeKnowledgebaseArticle.aspx?kbaid={articleID}";
        //        grid.enablepaging = true;
        //        grid.pagesize = 10;
        //        grid.showfooters = true;
        //        grid.showheaders = true;
        //        grid.DisplayFilter = true;

        //        grid.getColumnByName("articleID").hidden = true;
        //        grid.SortedColumn = grid.getColumnByName("Title");

        //        grid.getColumnByName("Type").addValueListItem((int)svcHelpAndSupport.ArticleType.FAQ, "<img alt=\"Frequently Asked Question\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/question_and_answer.png") + "\" />");
        //        grid.getColumnByName("Type").addValueListItem((int)svcHelpAndSupport.ArticleType.Walkthrough, "<img alt=\"Walkthrough Guide\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/note.png") + "\" />");
        //        grid.getColumnByName("Type").addValueListItem((int)svcHelpAndSupport.ArticleType.OnlineDemo, "<img alt=\"Video Demonstration\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/plasma-tv.png") + "\" />");

        //        grid.getColumnByName("Published").addValueListItem(1, "<a href=\"javascript:void(0);\" onclick=\"ApproveArticle({articleID},false);\">Published</a>");
        //        grid.getColumnByName("Published").addValueListItem(0, "<a href=\"javascript:void(0);\" onclick=\"ApproveArticle({articleID},true);\">Not Published</a>");

        //        return grid.generateGrid();
        //    }

        // REMOVE AND REPLACE WITH ABOVE WHEN NEW SP READY
        [WebMethod(EnableSession = true)]
        public string[] RefreshKnowledgeBaseArticlesGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int productID = 0;

            svcHelpAndSupport.svcHelpAndSupport helpService = new Spend_Management.svcHelpAndSupport.svcHelpAndSupport();
            svcHelpAndSupport.cProduct[] helpProducts = helpService.getProducts();
            foreach (svcHelpAndSupport.cProduct prod in helpProducts)
            {
                if (currentUser.CurrentActiveModule.ToString() == prod.ProductName)
                {
                    productID = prod.ProductID;
                }
            }
            svcHelpAndSupport.cKnowledgeBaseArticle[] lstArticles = helpService.GetAccountArticles(currentUser.AccountID, productID);

            var clsTables = new cTables(currentUser.AccountID);
            var tbl = clsTables.GetTableByID(new Guid("314a8e7e-470a-430f-85be-9a1a928609d5"));
            var clsFields = new cFields(currentUser.AccountID);

            var lstColumns = new List<cNewGridColumn>();
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("58A5861A-DE2F-4C1E-8C65-D2C38B81CA05"))));//articleID
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("8EE181CE-A67B-4099-BAB2-78F3B7D2F96A"))));//type
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("65EB4A84-3303-4EC2-9777-294995FA7FE0"))));//title
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("0670EDD9-B163-4CC7-9CB2-677C4CD024BB"))));//productarea
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("16DE32E1-0544-42B1-A695-D80D693470D4"))));//querytype
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("97326562-E049-4BA5-BF20-175AE0FF52DA"))));//Published

            var ds = new System.Data.DataSet { DataSetName = "Knowledge Base Articles" };
            var dt = new System.Data.DataTable { TableName = "Knowledge Base Articles" };
            dt.Columns.Add("articleID", typeof(int));
            dt.Columns.Add("Type", typeof(svcHelpAndSupport.ArticleType));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Product Area", typeof(string));
            dt.Columns.Add("Query Type", typeof(string));
            dt.Columns.Add("Published", typeof(int));

            int pub = 0;
            foreach (svcHelpAndSupport.cKnowledgeBaseArticle kba in lstArticles)
            {
                pub = (kba.Approved == true) ? 1 : 0;
                switch (kba.ArticleType)
                {
                    case Spend_Management.svcHelpAndSupport.ArticleType.FAQ:
                        dt.Rows.Add(new object[6] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cFAQ)kba).Question, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName, pub });
                        break;
                    case Spend_Management.svcHelpAndSupport.ArticleType.Walkthrough:
                        dt.Rows.Add(new object[6] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cWalkthrough)kba).Title, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName, pub });
                        break;
                    case Spend_Management.svcHelpAndSupport.ArticleType.OnlineDemo:
                        dt.Rows.Add(new object[6] { kba.ArticleID, kba.ArticleType, ((svcHelpAndSupport.cOnlineDemo)kba).Title, kba.ProductArea.AreaName, kba.QueryType.QueryTypeName, pub });
                        break;
                    default:
                        break;
                }
            }
            ds.Tables.Add(dt);

            cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "tblKBAGrid", tbl, lstColumns, ds);

            grid.KeyField = "articleID";
            grid.EmptyText = "No Items";
            grid.enabledeleting = true;
            grid.deletelink = "javascript:DeleteArticle({articleID});";
            grid.enableupdating = true;
            grid.editlink = "aeKnowledgebaseArticle.aspx?kbaid={articleID}";
            grid.enablepaging = true;
            grid.pagesize = 10;
            grid.showfooters = false;
            grid.showheaders = true;
            grid.DisplayFilter = true;

            grid.getColumnByName("articleID").hidden = true;
            grid.SortedColumn = grid.getColumnByName("Title");

            ((cFieldColumn)grid.getColumnByName("Type")).addValueListItem((int)svcHelpAndSupport.ArticleType.FAQ, "<img alt=\"Frequently Asked Question\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/question_and_answer.png") + "\" />");
            ((cFieldColumn)grid.getColumnByName("Type")).addValueListItem((int)svcHelpAndSupport.ArticleType.Walkthrough, "<img alt=\"Walkthrough Guide\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/note.png") + "\" />");
            ((cFieldColumn)grid.getColumnByName("Type")).addValueListItem((int)svcHelpAndSupport.ArticleType.OnlineDemo, "<img alt=\"Video Demonstration\" src=\"" + grid.ResolveUrl("/shared/images/icons/24/plain/plasma-tv.png") + "\" />");

            ((cFieldColumn)grid.getColumnByName("Published")).addValueListItem(1, "<a href=\"javascript:void(0);\" onclick=\"ApproveArticle({articleID},false);\">Published</a>");
            ((cFieldColumn)grid.getColumnByName("Published")).addValueListItem(0, "<a href=\"javascript:void(0);\" onclick=\"ApproveArticle({articleID},true);\">Not Published</a>");

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }
    }
}

