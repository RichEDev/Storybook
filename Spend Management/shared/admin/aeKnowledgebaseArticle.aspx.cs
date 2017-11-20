using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    public partial class aeKnowledgebaseArticle : System.Web.UI.Page
    {
        public int kbID
        {
            get { return kbaID; }
            set { }
        }
        private int kbaID;

        private int nHelpProdID = 0;
        /// <summary>
        /// The product ID on the support portal that matches the current module
        /// </summary>
        public int SupportPortalProductID
        {
            get { return nHelpProdID; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            // suggestion - update this to be able to overwrite our articles with customer ones
            if (IsPostBack == false)
            {
                Title = "Knowledge Base Article";
                Master.title = Title;
                Master.enablenavigation = false;

                int.TryParse(Request.QueryString["kbaid"], out kbaID);
                if (kbaID == 0)
                {
                    Master.PageSubTitle = "Add Article";
                }
                else
                {
                    Master.PageSubTitle = "Edit Article";
                }


                CurrentUser currentUser = cMisc.GetCurrentUser();
                currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FAQS, true, true);
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
                cAccountProperties clsAccountProperties;
                if (currentUser.CurrentSubAccountId >= 0)
                {
                    clsAccountProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                }
                else
                {
                    clsAccountProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
                }


                svcHelpAndSupport.svcHelpAndSupport helpService = new svcHelpAndSupport.svcHelpAndSupport();
                svcHelpAndSupport.cKnowledgeBaseArticle kba = new Spend_Management.svcHelpAndSupport.cKnowledgeBaseArticle();
                svcHelpAndSupport.cFAQ faq = new Spend_Management.svcHelpAndSupport.cFAQ();

                string helpProdName = currentUser.CurrentSupportPortalProductName;

                // find the Spend Management Module we're using
                svcHelpAndSupport.cProduct[] lstKbaProducts = helpService.getProducts();
                foreach (svcHelpAndSupport.cProduct product in lstKbaProducts)
                {
                    if (product.ProductName == helpProdName)
                    {
                        nHelpProdID = product.ProductID;
                        break;
                    }
                }

                #region add items to dropdowns
                svcHelpAndSupport.cProductArea[] productAreaListItems = helpService.getProductAreas(nHelpProdID);
                svcHelpAndSupport.cQueryType[] queryTypeListItems = helpService.getQueryTypes();

                ListItem[] lstProductAreas = new ListItem[productAreaListItems.Length];
                ListItem[] lstQueryTypes = new ListItem[queryTypeListItems.Length];

                for (int i = 0; i < productAreaListItems.Length; i++)
                {
                    lstProductAreas[i] = new ListItem();
                    lstProductAreas[i].Value = productAreaListItems[i].ProductAreaID.ToString();
                    lstProductAreas[i].Text = productAreaListItems[i].AreaName;
                }
                for (int i = 0; i < queryTypeListItems.Length; i++)
                {
                    lstQueryTypes[i] = new ListItem();
                    lstQueryTypes[i].Value = queryTypeListItems[i].QueryTypeID.ToString();
                    lstQueryTypes[i].Text = queryTypeListItems[i].QueryTypeName;
                }

                ddlArticleProductArea.Items.AddRange(lstProductAreas);
                ddlArticleQueryType.Items.AddRange(lstQueryTypes);
                #endregion add items to dropdowns

                if (kbaID != 0)
                {
                    kba = helpService.getKnowledgeBaseArticleByID(kbaID);
                    if (kba != null && kba.ArticleType == svcHelpAndSupport.ArticleType.FAQ)
                    {
                        faq = (svcHelpAndSupport.cFAQ)kba;
                        ddlArticleProductArea.SelectedIndex = ddlArticleProductArea.Items.IndexOf(ddlArticleProductArea.Items.FindByValue(kba.ProductArea.ProductAreaID.ToString()));
                        ddlArticleQueryType.SelectedIndex = ddlArticleQueryType.Items.IndexOf(ddlArticleQueryType.Items.FindByValue(kba.QueryType.QueryTypeID.ToString()));
                        txtArticleQuestion.Text = faq.Question;
                        txtArticleAnswerText.Text = faq.Answer;
                        //
                    }
                }
            }
        }
    }
}
