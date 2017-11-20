namespace Spend_Management
{
    using System;
    using System.Web;

    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.HelpAndSupport;
    using SpendManagementLibrary.Interfaces;


    /// <summary>
    /// Help and Support Page
    /// </summary>
    public partial class helpAndSupportTicket : System.Web.UI.Page
    {
        /// <summary>
        /// Does software europe provide support for this customer
        /// </summary>
        public bool SoftwareEuropeProvideSupport
        {
            get;
            set;
        }

        /// <summary>
        /// The identifier of the support ticket to load
        /// </summary>
        public string SupportTicketId { get; set; }

        /// <summary>
        /// The type of ticket being used by the page
        /// </summary>
        public SupportTicketType TicketType { get; set; }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            
            this.circleLink.Visible = Knowledge.CanAccessCircle(user);

            this.SupportTicketId = Request.QueryString["SupportTicketId"];
            this.TicketType = (SupportTicketType)int.Parse(Request.QueryString["TicketType"]);

            ISupportTicket ticket = null;
            SupportTicketInternal internalTicket=null;

            if (IsPostBack == false)
            {
                switch (this.TicketType)
                {
                    case SupportTicketType.SalesForce:
                    {
                        ticket = SupportTicketSalesForce.Get(user.Account.companyid, user, this.SupportTicketId);
                        break;
                    }

                    case SupportTicketType.Internal:
                    {
                        cmdConvertTicket.Visible = false;

                       internalTicket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));
                        ticket = internalTicket;
                        break;
                    }
                }

                if (ticket == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl);
                }
                else
                {
                    if (internalTicket != null && user.EmployeeID != internalTicket.Owner)
                    {
                        this.Response.Redirect("~/restricted.aspx", true);
                    }
                }

                if (ticket.Status == SupportTicketStatus.Closed.ToString())
                {
                    this.cmdCloseTicket.Visible = false;
                }

                this.litSupportTicketId.Text = ticket.CaseNumber;
                this.litStatus.Text = ticket.Status;
                this.litOpenedOn.Text = ticket.CreatedOn.ToString("dd/MM/yyyy HH:mm");
                this.litUpdatedOn.Text = ticket.ModifiedOn.Equals(DateTime.MinValue) ? string.Empty : ticket.ModifiedOn.ToString("dd/MM/yyyy HH:mm");
                this.litSubject.Text = ticket.Subject;
                this.litDescription.Text = ticket.Description;

                this.TicketCommentsRepeater.DataSource = ticket.Comments;
                this.TicketCommentsRepeater.DataBind();
                this.spnTicketCommentsEmpty.Visible = (ticket.Comments.Count < 1);

                this.TicketAttachmentsRepeater.DataSource = ticket.Attachments;
                this.TicketAttachmentsRepeater.DataBind();
                this.spnTicketAttachmentsEmpty.Visible = (ticket.Attachments.Count < 1);

                Title = string.Format("Help &amp; Support Ticket: {0}", litSupportTicketId.Text);
                Master.title = Title;
                Master.PageSubTitle = "Help &amp; Support Ticket Details";
            }
        }

        /// <summary>
        /// Close ticket button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdCloseTicket_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            
            switch (this.TicketType)
            {
                case SupportTicketType.Internal:
                {
                    SupportTicketInternal ticket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));
                    ticket.InternalStatus = SupportTicketStatus.Closed;
                    SupportTicketInternal.Update(user, ticket);
                    litStatus.Text = ticket.Status;

                    break;
                }
                case SupportTicketType.SalesForce:
                {
                    SupportTicketSalesForce.Close(user.Account.companyid, user, this.SupportTicketId);
                    SupportTicketSalesForce ticket = SupportTicketSalesForce.Get(user.Account.companyid, user, this.SupportTicketId);
                    litStatus.Text = ticket.Status;

                    break;
                }
            }
        }

        /// <summary>
        /// Send ticket to administrator button event method
        /// Retrieves an existing sales force support ticket, creates a new internal support ticket with the same information, then closes the sale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdConvertTicket_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            SupportTicketSalesForce ticket = SupportTicketSalesForce.Get(user.Account.companyid, user, this.SupportTicketId);

            int newTicketId = SupportTicketInternal.Create(user, ticket.Subject, ticket.Description);

            if (ticket.Comments.Count > 0 || ticket.Attachments.Count > 0)
            {
                foreach (SupportTicketSalesForceAttachment attachment in ticket.Attachments)
                {
                    var fullAttachment = SupportTicketSalesForceAttachment.Get(attachment.Id, this.SupportTicketId, user);
                    SupportTicketAttachment.Create(user, newTicketId, attachment.Filename, fullAttachment.FileBytes);
                }
            }

            SupportTicketSalesForce.Close(user.Account.companyid, user, ticket.Identifier);
                    
            Response.Redirect("/shared/helpAndSupportTicket.aspx?TicketType=1&SupportTicketId=" + newTicketId);
        }

        /// <summary>
        /// submit attachment button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdSubmitAttachment_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ISupportTicket ticket = null;

            if (!this.uplAttachment.HasFile)
            {
                return;
            }

            switch (this.TicketType)
            {
                case SupportTicketType.Internal:
                {
                    int attachmentId = SupportTicketAttachment.Create(user, int.Parse(this.SupportTicketId), this.uplAttachment.FileName, this.uplAttachment.FileBytes);
                    if (attachmentId > 0)
                    {
                        ticket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));
                        ticket.ModifiedOn = DateTime.UtcNow;

                        SupportTicketInternal.Update(user, ticket as SupportTicketInternal);
                    }

                    break;
                }
                case SupportTicketType.SalesForce:
                {
                    string attachmentId = SupportTicketSalesForceAttachment.Create(this.SupportTicketId, this.uplAttachment.FileName, this.uplAttachment.FileBytes);

                    if (attachmentId != string.Empty)
                    {
                        ticket = SupportTicketSalesForce.Get(user.Account.companyid, user, this.SupportTicketId);
                    }

                    break;
                }
            }

            if (ticket == null)
            {
                return;
            }

            Response.Redirect(Request.Url.ToString());
        }

        /// <summary>
        /// Submit comment button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdSubmitComment_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string commentBody = txtComment.Text;
            ISupportTicket ticket = null;

            switch (this.TicketType)
            {
                case SupportTicketType.SalesForce:
                {
                    string commentId = SupportTicketCommentSalesForce.Create(this.SupportTicketId, commentBody);
                    if (commentId != string.Empty)
                    {
                        ticket = SupportTicketSalesForce.Get(user.Account.companyid, user, this.SupportTicketId);
                    }

                    break;
                }
                case SupportTicketType.Internal:
                {
                    int commentId  = SupportTicketCommentInternal.Create(user, int.Parse(this.SupportTicketId), commentBody);
                    if (commentId > 0)
                    {
                        ticket = SupportTicketInternal.Get(user, int.Parse(this.SupportTicketId));

                        ticket.Status = SupportTicketStatus.PendingAdministratorResponse.ToString();
                        ticket.ModifiedOn = DateTime.UtcNow;

                        SupportTicketInternal.Update(user, ticket as SupportTicketInternal);
                    }

                    break;
                }
            }

            Response.Redirect(Request.Url.ToString());
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
