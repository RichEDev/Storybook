namespace Spend_Management.shared.webServices
{
    using System.Web;
    using System.Web.Services;

    using SpendManagementLibrary;
    using SpendManagementLibrary.HelpAndSupport;

    /// <summary>
    /// A set of web service methods for Knowledge
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcKnowledge : WebService
    {
        /// <summary>
        /// Deletes a <see cref="KnowledgeCustomArticle"/>
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <returns>0 if the article was deleted, otherwise an error code</returns>
        [WebMethod(EnableSession = true)]
        public int DeleteCustomArticle(int identifier)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.FAQS, true, false) == false)
            {
                return -999;
            }

            return KnowledgeCustomArticle.Delete(user, identifier);
        }

        /// <summary>
        /// Retrieves a <see cref="KnowledgeCustomArticle"/> by its identifier
        /// </summary>
        /// <param name="identifier">The article's identifier</param>
        /// <returns>The requested <see cref="KnowledgeCustomArticle"/></returns>
        [WebMethod(EnableSession = true)]
        public KnowledgeCustomArticle GetCustomArticle(int identifier)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            return KnowledgeCustomArticle.Get(user.AccountID, identifier);
        }

        /// <summary>
        /// Saves an existing <see cref="KnowledgeCustomArticle"/> if an identifier greaeter than 0 is provided, creates a new <see cref="KnowledgeCustomArticle" /> if an identifier of 0 is provided
        /// </summary>
        /// <param name="identifier">The identifier (0 to create new)</param>
        /// <param name="productCategory">The <see cref="KnowledgeCustomArticle" /> product category value</param>
        /// <param name="title">The <see cref="KnowledgeCustomArticle" /> title text</param>
        /// <param name="summary">The <see cref="KnowledgeCustomArticle" /> summary text</param>
        /// <param name="body"><see cref="KnowledgeCustomArticle" /> body text</param>
        /// <returns>The identifier of the saved <see cref="KnowledgeCustomArticle" /> or a negative number to indicate failure</returns>
        [WebMethod(EnableSession = true)]
        public int SaveCustomArticle(int identifier, string productCategory, string title, string summary, string body)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            KnowledgeCustomArticle article;
            if (identifier > 0)
            {
                if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FAQS, true, false) == false)
                {
                    return -999;
                }

                article = KnowledgeCustomArticle.Get(user.AccountID, identifier);
            }
            else
            {
                if (user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.FAQS, true, false) == false)
                {
                    return -999;
                }

                article = new KnowledgeCustomArticle { Identifier = identifier };
            }

            article.Title = title;
            article.ProductCategoryValue = productCategory;
            article.Summary = summary;
            article.Body = HttpUtility.HtmlDecode(body);

            return article.Save(user);
        }

        /// <summary>
        /// Toggles the Published state of a <see cref="KnowledgeCustomArticle"/>
        /// </summary>
        /// <param name="identifier">The <see cref="KnowledgeCustomArticle"/> identifier</param>
        /// <returns>The identifier of the saved <see cref="KnowledgeCustomArticle" /> or a negative number to indicate failure</returns>
        [WebMethod(EnableSession = true)]
        public int ToggleCustomArticlePublished(int identifier)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FAQS, true, false) == false)
            {
                return -999;
            }

            return KnowledgeCustomArticle.TogglePublished(user, identifier);
        }
    }
}
