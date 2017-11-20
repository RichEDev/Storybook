namespace Spend_Management
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.HelpAndSupport;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Help and Support Page
    /// </summary>
    public partial class adminHelpAndSupportTicket : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        public string SupportTicketId { get; set; }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SupportTickets, true, true);

            this.SupportTicketId = Request.QueryString["SupportTicketId"];

            if (IsPostBack == false)
            {
                SupportTicketInternal ticket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));

                if (ticket == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl);
                }

                this.TicketCommentsRepeater.DataSource = ticket.Comments;
                this.TicketCommentsRepeater.DataBind();
                this.spnTicketCommentsEmpty.Visible = (ticket.Comments.Count < 1);

                this.TicketAttachmentsRepeater.DataSource = ticket.Attachments;
                this.TicketAttachmentsRepeater.DataBind();
                this.spnTicketAttachmentsEmpty.Visible = (ticket.Attachments.Count < 1);

                foreach (int statusValue in Enum.GetValues(typeof(SupportTicketStatus)))
                {
                    ddlStatus.Items.Add(new ListItem(Enum.GetName(typeof(SupportTicketStatus), statusValue).SplitCamel(), statusValue.ToString(CultureInfo.InvariantCulture)));
                }

                this.litSupportTicketId.Text = this.SupportTicketId;
                this.ddlStatus.SelectedValue = ((int)ticket.InternalStatus).ToString(CultureInfo.InvariantCulture);
                this.litOpenedOn.Text = ticket.CreatedOn.ToString("dd/MM/yyyy HH:mm");
                this.litUpdatedOn.Text = ticket.ModifiedOn.Equals(DateTime.MinValue) ? string.Empty : ticket.ModifiedOn.ToString("dd/MM/yyyy HH:mm");
                this.litSubject.Text = ticket.Subject;
                this.litDescription.Text = ticket.Description;

                Title = "Support Ticket Details";
                Master.PageSubTitle = Title;
                Master.title = string.Format("Support Ticket: {0}", litSupportTicketId.Text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdSubmitComment_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string commentBody = txtComment.Text;
            SupportTicketInternal ticket = null;

            int commentId = SupportTicketCommentInternal.Create(user, int.Parse(this.SupportTicketId), commentBody);
            if (commentId > 0)
            {
                ticket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));

                ticket.Status = SupportTicketStatus.PendingEmployeeResponse.ToString();
                ticket.ModifiedOn = DateTime.UtcNow;

                SupportTicketInternal.Update(user, ticket);
            }

            Response.Redirect(Request.Url.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdSubmit_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            SupportTicketInternal ticket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));

            ticket.InternalStatus = (SupportTicketStatus)int.Parse(ddlStatus.SelectedValue);
            ticket.ModifiedOn = DateTime.UtcNow;

            SupportTicketInternal.Update(user, ticket);

            this.CmdCancel_Click(sender, e);
        }

        /// <summary>
        /// Close button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdCancel_Click(object sender, EventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }
    }
}
