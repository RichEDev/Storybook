using Infragistics.WebUI.UltraWebNavigator;
using SpendManagementLibrary.Interfaces;

namespace Spend_Management
{
    using System;
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.HelpAndSupport;

    /// <summary>
    /// Help and Support Page
    /// </summary>
    public partial class helpAndSupport : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAccountProperties accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            this.ticketsLink.Visible = (user.Account.ContactHelpDeskAllowed || accountProperties.EnableInternalSupportTickets || user.Employee.ContactHelpDeskAllowed) && !user.isDelegate;
            this.circleLink.Visible = Knowledge.CanAccessCircle(user);

            this.Title = "Online Help";
            this.Master.PageSubTitle = this.Title;;

            if (IsPostBack == false)
            {
                /*var results = Knowledge.Search(user.AccountID, "validation", string.Empty, string.Empty);
                featuredArticlesRepeater.DataSource = results;
                featuredArticlesRepeater.DataBind();

                cmbArticleProductFeature.DataSource = Knowledge.GetArticleCategories("Product_Categories");
                cmbArticleProductFeature.DataValueField = "Key";
                cmbArticleProductFeature.DataTextField = "Value";
                cmbArticleProductFeature.DataBind();
                
                cmbArticleType.DataSource = Knowledge.GetArticleCategories("Document_Type");
                cmbArticleType.DataValueField = "Key";
                cmbArticleType.DataTextField = "Value";
                cmbArticleType.DataBind();*/

                //rdoIssueType.Items[0].Text = FilterQuestion1Text;

                DataSet questions = Knowledge.GetHeadingsAndQuestions(user);
                questionHeadingsRepeater.DataSource = questions.Tables["SupportQuestionHeadings"];
                questionHeadingsRepeater.DataBind();
            }
        }

        /// <summary>
        /// Close button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdClose_Click(object sender, EventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }

        /// <summary>
        /// Search button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdSearch_Click(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            var searchTerm = txtSearchTerm.Text.Trim();
            //var productFeature = cmbArticleProductFeature.SelectedValue;
            //var articleType = cmbArticleType.SelectedValue;

            var results = Knowledge.Search(currentUser.AccountID, searchTerm, string.Empty, string.Empty);// productFeature, articleType);

            knowledgeArticlesRepeater.DataSource = results;
            knowledgeArticlesRepeater.DataBind();

            // toggle the visibility of the search results / no results panels
            searchResults.Visible = results.Count > 0;
            noSearchResults.Visible = !(results.Count > 0);
        }

        /// <summary>
        /// Called for each data item in the article data source.
        /// </summary>
        /// <param name="sender">The asp:Repeater</param>
        /// <param name="e">The event</param>
        protected void ArticleViewItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // add the article type name and category as a CSS class
            var resultDiv = (Panel)e.Item.FindControl("searchResult");
            string itemType = e.Item.DataItem.GetType().Name;
            string productCategory = ((IKnowledgeArticle) e.Item.DataItem).ProductCategoryValue;
            resultDiv.CssClass += " " + itemType + " " + productCategory;

            var tooltip = string.Empty;
            if (itemType == "KnowledgeSalesForceArticle")
            {
                var anchor = (HyperLink) e.Item.FindControl("searchResultAnchor");
                anchor.NavigateUrl = ((KnowledgeSalesForceArticle) e.Item.DataItem).Url;

                switch (productCategory)
                {
                    case "Articles__kav":
                        tooltip = "Article";
                        break;
                    case "Walkthrough_Guide__kav":
                        tooltip = "Walkthrough Guide";
                        break;
                    case "Online_Demo__kav":
                        tooltip = "Online Demo";
                        break;
                    case "Feature_Overview__kav":
                        tooltip = "Feature Overview";
                        break;
                    case "Frequently_Asked_Question__kav":
                        tooltip = "Frequently Asked Question";
                        break;
                }
            }
            else
            {
                tooltip = "Article provided by your administrator";
            }

            resultDiv.ToolTip = tooltip;

        }

        /// <summary>
        /// Called for each data item in the receipts data source, this method sets up the correct template for the repeater items.
        /// </summary>
        /// <param name="sender">The asp:Repeater</param>
        /// <param name="e">The event</param>
        protected void QuestionDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = (DataRow)e.Item.DataItem;
            var multiView = (MultiView)e.Item.FindControl("questionMultiView");

            if (bool.Parse(item["SupportTicketSel"].ToString()))
            {
                multiView.ActiveViewIndex = 1;
            } 
            else if (bool.Parse(item["SupportTicketInternal"].ToString()))
            {
                multiView.ActiveViewIndex = 2;
            }
        }
    }
}
