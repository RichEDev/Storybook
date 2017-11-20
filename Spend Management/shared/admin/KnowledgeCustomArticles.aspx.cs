namespace Spend_Management.shared.admin
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.UI;

    using SpendManagementLibrary;
    using SpendManagementLibrary.HelpAndSupport;

    public partial class KnowledgeCustomArticles : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Knowledge Articles";
            Master.title = Title;
            Master.showdummymenu = true;

            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true, true);

            if (!IsPostBack)
            {
                this.lnkNewArticle.Visible = currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.FAQS, true, false);

                #region Grid

                string gridJavaScript = string.Empty;
                string[] gridStrings = this.Grid(currentUser);

                if (gridStrings.Length == 2)
                {
                    gridJavaScript = gridStrings[0];
                    litGrid.Text = gridStrings[1];
                }

                this.ClientScript.RegisterStartupScript(this.GetType(), "articlesGridJavaScript", cGridNew.generateJS_init("articlesGridJavaScript", new List<string> { gridJavaScript }, currentUser.CurrentActiveModule), true);

                #endregion Grid

                #region Form

                cmbProductCategory.DataSource = Knowledge.GetArticleCategories();
                cmbProductCategory.DataValueField = "Key";
                cmbProductCategory.DataTextField = "Value";
                cmbProductCategory.DataBind();
                
                #endregion Form
            }
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }

        /// <summary>
        /// Generates the knowledge articles grid
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>The generated grid</returns>
        private string[] Grid(ICurrentUser currentUser)
        {
            const string GridSql = "SELECT KnowledgeCustomArticleId, Title, ProductCategory, Published FROM KnowledgeCustomArticles";
            const string GridName = "gridArticles";

            var grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, GridName, GridSql)
            {
                KeyField = "KnowledgeCustomArticleId",
                EmptyText = "There are no knowledge articles to display.",
                editlink = "javascript:SEL.Knowledge.CustomArticle.Edit({KnowledgeCustomArticleId});",
                deletelink = "javascript:SEL.Knowledge.CustomArticle.Delete({KnowledgeCustomArticleId});",
                enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FAQS, true, false),
                enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.FAQS, true, false)
            };

            grid.getColumnByName("KnowledgeCustomArticleId").hidden = true;

            grid.InitialiseRow += this.KnowledgeCustomArticleGridOnInitialiseRow;
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.shared.admin.KnowledgeCustomArticles";
            grid.ServiceClassMethodForInitialiseRowEvent = "KnowledgeCustomArticleGridOnInitialiseRow";

            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FAQS, true, false))
            {
                grid.addTwoStateEventColumn("Published", (cFieldColumn)grid.getColumnByName("Published"), true, false, GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/document_down.png", "javascript:SEL.Knowledge.CustomArticle.TogglePublished({KnowledgeCustomArticleId});", "Click to un-publish.", "Un-publish", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/document_up.png", "javascript:SEL.Knowledge.CustomArticle.TogglePublished({KnowledgeCustomArticleId});", "Click to publish.", "Publish");
            }
            else
            {
                grid.getColumnByName("Published").hidden = true;
            }

            return grid.generateGrid();
        }

        /// <summary>
        /// Post grid-render method, called by cGridNew every time a grid's row is initialised
        /// </summary>
        /// <param name="row">The cGridNew row</param>
        /// <param name="gridInfo">A dictionary of objects, necessary for cGridNew but not used here</param>
        private void KnowledgeCustomArticleGridOnInitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo = null)
        {
            // Convert the ProductCategory Key into a friendly value
            OrderedDictionary categories = Knowledge.GetArticleCategories();
            var cell = row.getCellByID("ProductCategory");
            var productCategoryKey = (string)cell.Value;

            // If there is no ProductCategory assigned to the article (or the ProductCategory doesn't exist in the list) just output an empty string
            if (string.IsNullOrEmpty(productCategoryKey) == false && categories.Contains(productCategoryKey))
            {
                cell.Value = categories[productCategoryKey].ToString();
            }
            else
            {
                cell.Value = string.Empty;
            }
        }
    }
}