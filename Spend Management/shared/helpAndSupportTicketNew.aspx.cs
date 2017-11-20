namespace Spend_Management
{
    using System;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;

    using Microsoft.JScript;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.HelpAndSupport;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Help and Support Page
    /// </summary>
    public partial class helpAndSupportTicketNew : System.Web.UI.Page
    {
        /// <summary>
        /// The type (SalesForce/Internal) of ticket to create
        /// </summary>
        public SupportTicketType TicketType { get; set; }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // don't allow people to just come to this page, they have to go to HelpAndSupport.aspx first
            if (IsPostBack == false && Request.UrlReferrer == null)
            {
                //Response.Redirect("/shared/helpAndSupport.aspx");
            }

            CurrentUser user = cMisc.GetCurrentUser();
            this.circleLink.Visible = Knowledge.CanAccessCircle(user);

            // users can only raise SEL helpdesk tickets if enabled on their account
            this.TicketType = (SupportTicketType)int.Parse(Request.QueryString["TicketType"]);
            if (this.TicketType == SupportTicketType.SalesForce && user.Account.ContactHelpDeskAllowed == false && user.Employee.ContactHelpDeskAllowed == false)
            {
                this.TicketType = SupportTicketType.Internal;
            }

            Title = "Help &amp; Support Ticket Details";
            Master.PageSubTitle = Title;
            Master.title = "New Help &amp; Support Ticket";

            if (IsPostBack == false)
            {
                switch (this.TicketType)
                {
                    case SupportTicketType.SalesForce:
                    {
                        customerHelpText.Visible = false;
                        internalWelcomeMessage.Visible = false;
                        cmdClose.Visible = false;
                        break;
                    }
                    case SupportTicketType.Internal:
                    {
                        selDisclaimer.Visible = false;
                        selWelcomeMessage.Visible = false;

                        cAccountProperties accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
                        this.ticketForm.Visible = accountProperties.EnableInternalSupportTickets;

                        #region build the customer specified contact and help text
                        var clsSubAccounts = new cAccountSubAccounts(user.AccountID);
                        cAccountProperties clsAccountProperties = user.CurrentSubAccountId >= 0 ? clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties : clsSubAccounts.getFirstSubAccount().SubAccountProperties;

                        var sCustomerContact = new StringBuilder();
                        var sCustomerInformation = new StringBuilder();

                        if (clsAccountProperties.CustomerHelpContactName != null && clsAccountProperties.CustomerHelpContactName != String.Empty)
                        {
                            sCustomerContact.Append("<div><span class=\"ocptitle\">Help Contact Name</span> " + clsAccountProperties.CustomerHelpContactName + "</div>");
                        }
                        if (clsAccountProperties.CustomerHelpContactTelephone != null && clsAccountProperties.CustomerHelpContactTelephone != String.Empty)
                        {
                            sCustomerContact.Append("<div><span class=\"ocptitle\">Telephone</span> " + clsAccountProperties.CustomerHelpContactTelephone + "</div>");
                        }
                        if (clsAccountProperties.CustomerHelpContactFax != null && clsAccountProperties.CustomerHelpContactFax != String.Empty)
                        {
                            sCustomerContact.Append("<div><span class=\"ocptitle\">Fax</span> " + clsAccountProperties.CustomerHelpContactFax + "</div>");
                        }
                        if (clsAccountProperties.CustomerHelpContactEmailAddress != null && clsAccountProperties.CustomerHelpContactEmailAddress != String.Empty)
                        {
                            sCustomerContact.Append("<div><span class=\"ocptitle\">E-mail</span> " + clsAccountProperties.CustomerHelpContactEmailAddress + "</div>");
                        }

                        if (sCustomerContact.Length > 0)
                        {
                            sCustomerContact.Insert(0, "<span id=\"ocpcontactinfo\">");
                            sCustomerContact.Append("</span>");
                        }

                        if (clsAccountProperties.CustomerHelpContactAddress != null && clsAccountProperties.CustomerHelpContactAddress != String.Empty)
                        {
                            sCustomerContact.Append("<span id=\"ocpcontactaddress\">");
                            sCustomerContact.Append("<span class=\"ocptitle\">Postal Address</span>");
                            sCustomerContact.Append("<span class=\"ocpcontactaddresstext\">" + clsAccountProperties.CustomerHelpContactAddress.Replace("\n", "<br />") + "</span>");
                            sCustomerContact.Append("</span>");
                        }

                        if (sCustomerContact.Length > 0)
                        {
                            sCustomerContact.Insert(0, "<div id=\"ocpcontact\">");
                            sCustomerContact.Append("</div>");
                        }

                        if (clsAccountProperties.CustomerHelpInformation != null && clsAccountProperties.CustomerHelpInformation != String.Empty)
                        {
                            if (sCustomerContact.Length > 0)
                            {
                                sCustomerInformation.Append("<div style=\"border-top: 1px dotted grey; padding-top: 10px;\">");
                            }
                            else
                            {
                                sCustomerInformation.Append("<div>");
                            }
                            sCustomerInformation.Append(clsAccountProperties.CustomerHelpInformation + "</div>");
                        }

                        //if (sCustomerContact.Length > 0)
                        //{
                        //    if (sCustomerInformation.Length > 0)
                        //    {
                        //        sCustomerContact.Insert(0, "<div style=\"width: 35%; padding-left: 5px; padding-bottom: 5px; margin-bottom: 5px; margin-left: 5px; border-left: 1px dotted #666666; border-bottom: 1px dotted #666666; float: right;\">");
                        //    }
                        //    else
                        //    {
                        //        sCustomerContact.Insert(0, "<div>");
                        //    }
                        //    sCustomerContact.Append("</div>");
                        //}

                        if (sCustomerContact.Length > 0 || sCustomerInformation.Length > 0)
                        {
                            litCustomerHelpText.Text = sCustomerContact.ToString() + sCustomerInformation.ToString();
                        }
                        else
                        {
                            if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.CompanyHelpAndSupportInformation, false))
                            {
                                litCustomerHelpText.Text = "Please enter some contact information or usage advice in the \"Help &amp; Support Information\" section of the Administrative Settings > Help &amp; Support Management page. As this is where your users will be advised to look for information when they wish their account details changed or other information changed.";
                            }
                            else
                            {
                                litCustomerHelpText.Text = "There are currently no administrator contact details or advice available at this time. When these become available they will be displayed here.";
                            }
                        }
                        #endregion build the customer specified contact and help text

                        break;
                    }
                }

                this.txtSubject.Text = Request["Subject"];
                this.hdnSearchTerm.Value = Request["Subject"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdSubmit_Click(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (Page.IsValid)
            {
                switch (this.TicketType)
                {
                    case SupportTicketType.SalesForce:
                    {
                        string salesForceTicketId = SupportTicketSalesForce.Create(user.Account.companyid, user, txtSubject.Text, txtDescription.Text);
                        if (uplAttachment.HasFile)
                        {
                            SupportTicketSalesForceAttachment.Create(salesForceTicketId, uplAttachment.FileName, uplAttachment.FileBytes);
                        }

                        successCommentSel.Visible = true;
                        break;
                    }
                    case SupportTicketType.Internal:
                    {
                        int customEntityId = 0;
                        int.TryParse(Request.QueryString["CustomEntityId"], out customEntityId);

                        int internalTicketId = SupportTicketInternal.Create(user, txtSubject.Text, txtDescription.Text, customEntityId);
                        if (uplAttachment.HasFile)
                        {
                            SupportTicketAttachment.Create(user, internalTicketId, uplAttachment.FileName, uplAttachment.FileBytes);
                        }

                        successCommentInternal.Visible = true;
                        break;
                    }
                }

                divErrorMessage.Visible = false;
                ticketForm.Visible = false;
                customerHelpText.Visible = false;

                successMessage.Visible = true;
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
    }
}
