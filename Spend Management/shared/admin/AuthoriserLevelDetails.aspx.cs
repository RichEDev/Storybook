namespace Spend_Management
{
    using Spend_Management.shared.code;
    using System;
    public partial class AuthoriserLevelDetails : System.Web.UI.Page
    {
        /// <summary>
        /// Called when page is loaded.
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Events argument</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.enablenavigation = false;
            this.Master.UseDynamicCSS = true;
            var authoriserLevelDetailId = Request.QueryString["authoriserLevelDetailId"];
            if (authoriserLevelDetailId != null)
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "authoriserLevelDetailId", "var authoriserLevelDetailId =" + authoriserLevelDetailId.ToString() + ";", true);
                var authoriserLevelDetail = new AuthoriserLevelDetail();
                authoriserLevelDetail.GetAuthoriserLevelDetail(Convert.ToInt16(authoriserLevelDetailId));
                txtAmount.Text = authoriserLevelDetail.Amount.ToString();
                txtDescription.Text = authoriserLevelDetail.Description;
                this.Title = "Authoriser Level: " + txtAmount.Text;
            }
            else
            {
                this.Title = "New Authoriser Level";
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "authoriserLevelDetailId", "var authoriserLevelDetailId = 0;", true);
            }
            this.Master.title = Title;
            lblAuthoriserLevelHeader.Text = Title;
            this.Master.PageSubTitle = "Authoriser Level Details";
            this.Master.showdummymenu = true;

        }
    }
}