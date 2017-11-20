namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.SalesForceApi;

    /// <summary>
    /// Represents a sales force support ticket
    /// </summary>
    public class SupportTicketSalesForce : ISupportTicket
    {
        #region Properties

        /// <summary>
        /// The identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Not the identifier, but a friendly number used to identify the support ticket
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        /// The status of the ticket, as a friendly string
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The subject line of the support request
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The full description of the problem
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// When the ticket was last modified
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// When the ticket was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// A list of attached files
        /// </summary>
        public List<ISupportTicketAttachment> Attachments { get; set; }

        /// <summary>
        /// A list of associated comments
        /// </summary>
        public List<ISupportTicketComment> Comments { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="SupportTicketSalesForce"/> object
        /// </summary>
        public SupportTicketSalesForce()
        {
            Attachments = new List<ISupportTicketAttachment>();
            Comments = new List<ISupportTicketComment>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Sets the status of a support ticket to "closed"
        /// </summary>
        /// <param name="companyId">The company id</param>
        /// <param name="user">The current user</param>
        /// <param name="ticketId">The ticket identifier</param>
        /// <returns></returns>
        public static void Close(string companyId, ICurrentUserBase user, string ticketId)
        {
            var ticket = new Case { Status = "Closed", Id = ticketId };

            var binding = Knowledge.GetSalesForceBinding();
            binding.update(new sObject[] { ticket });
        }

        /// <summary>
        /// Creates a new support ticket
        /// </summary>
        /// <param name="companyId">The company id</param>
        /// <param name="user">The current user</param>
        /// <param name="subject">The subject line of the support request</param>
        /// <param name="description">The full description of the problem</param>
        /// <returns></returns>
        public static string Create(string companyId, ICurrentUserBase user, string subject, string description)
        {
            string ticketId = null;

            string internalComments = string.Empty;
            SaveResult[] saveResults;
            var binding = Knowledge.GetSalesForceBinding();

            var ticket = new Case
                            {
                                Status = "New", 
                                Subject = subject, 
                                Description = description, 
                                Origin = "Web"
                            };

            // link the ticket to the correct customer service contract
            ServiceContract contract = null;
            QueryResult result = binding.query(string.Format("SELECT Name, AccountId, Service_Contract_Custom_ID__c FROM ServiceContract WHERE Company_ID__c = '{0}' AND Product__r.ProductCode = '{1}'", companyId, user.CurrentActiveModule.ToString()));

            if (result.records != null)
            {
                foreach (var searchResult in result.records)
                {
                    contract = (ServiceContract)searchResult;
                    break;
                }
            }
            else
            {
                internalComments += string.Format("No service contract found for companyid \"{0}\", product \"{1}\"", companyId, user.CurrentActiveModule);
            }

            if (contract != null)
            {
                ticket.Service_Contract__c = contract.Service_Contract_Custom_ID__c;
                ticket.AccountId = contract.AccountId;

                // link the ticket to the contact record, if one isn't found create one and use that
                Contact contact;
                result = binding.query(string.Format("SELECT Name, Id FROM Contact WHERE Email = '{0}'", user.Employee.EmailAddress));

                if (result.records != null)
                {
                    // link the ticket to the first contact that was returned (there should only ever be one)
                    contact = (Contact)result.records.First();
                    ticket.ContactId = contact.Id;
                }
                else
                {
                    // create the new contact using the employee's details
                    contact = new Contact
                                {
                                    Email = user.Employee.EmailAddress, 
                                    FirstName = user.Employee.Forename, 
                                    LastName = user.Employee.Surname, 
                                    AccountId = contract.AccountId
                                };

                    saveResults = binding.create(new sObject[] { contact });
                    foreach (var r in saveResults)
                    {
                        if (r.success)
                        {
                            // link the ticket to the new contact
                            ticket.ContactId = r.id;
                        }
                        else
                        {
                            internalComments = r.errors.Aggregate(internalComments, (current, e) => current + string.Format("\nFailed to create contact for {0} - {1}", user.Employee.EmailAddress, e.message));
                        }
                    }
                }
            }

            // create the ticket
            saveResults = binding.create(new sObject[] { ticket });
            foreach (var r in saveResults)
            {
                if (r.success)
                {
                    ticketId = r.id;
                }
            }

            // If service contract matching / contact matching or creating failed append the message as a comment to the ticket, 
            // so that helpdesk will know what went wrong
            if (ticketId != null && !string.IsNullOrEmpty(internalComments))
            {
                var comment = new CaseComment { CommentBody = internalComments, ParentId = ticketId };

                binding.create(new sObject[] { comment });
            }

            return ticketId;
        }

        /// <summary>
        /// Retrieves a support ticket
        /// </summary>
        /// <param name="companyId">The company id</param>
        /// <param name="user">The current user</param>
        /// <param name="ticketId">The ticket identifier</param>
        /// <returns></returns>
        public static SupportTicketSalesForce Get(string companyId, ICurrentUserBase user, string ticketId)
        {
            SupportTicketSalesForce ticket = null;

            var binding = Knowledge.GetSalesForceBinding();
            var query = string.Format("SELECT Id, CaseNumber, Status, Subject, Description, CreatedDate, LastModifiedDate FROM Case WHERE Contact.Email = '{0}' AND Service_Contract__r.Company_ID__c = '{1}' AND Id = '{2}'", user.Employee.EmailAddress, companyId, ticketId);

            QueryResult result = binding.query(query);

            if (result.records != null)
            {
                var salesForceTicket = (Case)result.records.First();

                ticket = new SupportTicketSalesForce
                        { 
                            Identifier = salesForceTicket.Id,
                            CaseNumber = salesForceTicket.CaseNumber,
                            Subject = salesForceTicket.Subject,
                            Status = salesForceTicket.Status,
                            Description = salesForceTicket.Description,
                            ModifiedOn = salesForceTicket.LastModifiedDate.HasValue ? salesForceTicket.LastModifiedDate.Value : DateTime.UtcNow,
                            CreatedOn = salesForceTicket.CreatedDate.HasValue ? salesForceTicket.CreatedDate.Value : DateTime.UtcNow
                        };

                // retrieve any associated comments
                query = string.Format("SELECT CommentBody, CreatedBy.Name, CreatedDate FROM CaseComment WHERE IsPublished = true AND ParentId = '{0}' ORDER BY CreatedDate DESC", ticketId);
                result = binding.query(query);

                if (result.records != null)
                {
                    foreach (var record in result.records)
                    {
                        var salesForceTicketComment = (CaseComment)record;

                        var comment = new SupportTicketCommentSalesForce
                                        {
                                            Body = salesForceTicketComment.CommentBody,
                                            CreatedOn = salesForceTicketComment.CreatedDate.HasValue ? salesForceTicketComment.CreatedDate.Value : DateTime.UtcNow,
                                            ContactName = salesForceTicketComment.CreatedBy.Name1 == "apiuser apiuser" ? user.Employee.FullName : salesForceTicketComment.CreatedBy.Name1
                                        };

                        ticket.Comments.Add(comment);
                    }
                }

                // retrieve any associated attachment details
                query = string.Format("SELECT Id, Name, BodyLength FROM Attachment WHERE ParentId = '{0}' ORDER BY CreatedDate DESC", ticketId);
                result = binding.query(query);

                if (result.records != null)
                {
                    foreach (var record in result.records)
                    {
                        var salesForceTicketAttachment = (Attachment)record;

                        var attachment = new SupportTicketSalesForceAttachment
                        {
                            Id = salesForceTicketAttachment.Id,
                            Filename = salesForceTicketAttachment.Name,
                            FileSize = salesForceTicketAttachment.BodyLength.HasValue ? salesForceTicketAttachment.BodyLength.Value : 0
                        };

                        ticket.Attachments.Add(attachment);
                    }
                }
            }

            return ticket;
        }

        /// <summary>
        /// Retrieves support ticket data for use with a <see cref="cGridNew"/>
        /// </summary>
        /// <param name="companyId">The company id</param>
        /// <param name="user">The current user</param>
        /// <returns></returns>
        public static DataSet GetDataset(string companyId, ICurrentUserBase user)
        {
            List<SupportTicketSalesForce> tickets = GetList(companyId, user);

            var table = new DataTable();
            table.Columns.Add("SupportTicketId", typeof(string));
            table.Columns.Add("CaseNumber", typeof(string));
            table.Columns.Add("Subject", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("CreatedOn", typeof(DateTime));
            table.Columns.Add("ModifiedOn", typeof(DateTime));

            var ticketData = new DataSet();
            ticketData.Tables.Add(table);

            foreach (SupportTicketSalesForce ticket in tickets)
            {
                DataRow row = ticketData.Tables[0].NewRow();
                row["SupportTicketId"] = ticket.Identifier;
                row["CaseNumber"] = ticket.CaseNumber;
                row["Subject"] = ticket.Subject;
                row["Status"] = ticket.Status;
                row["CreatedOn"] = ticket.CreatedOn;
                row["ModifiedOn"] = ticket.ModifiedOn;

                ticketData.Tables[0].Rows.Add(row);
            }

            return ticketData;
        }

        /// <summary>
        /// Retrieves a list of <see cref="SupportTicketSalesForce"/> for a particular account
        /// </summary>
        /// <param name="companyId">The company id</param>
        /// <param name="user">The current user</param>
        /// <returns></returns>
        public static List<SupportTicketSalesForce> GetList(string companyId, ICurrentUserBase user)
        {
            var tickets = new List<SupportTicketSalesForce>();

            var binding = Knowledge.GetSalesForceBinding();
            var query = string.Format("SELECT Id, CaseNumber, Status, Subject, CreatedDate, LastModifiedDate FROM Case WHERE Contact.Email = '{0}' AND Service_Contract__r.Company_ID__c = '{1}' AND Service_Contract__r.Product__r.ProductCode = '{2}'", user.Employee.EmailAddress, companyId ,user.CurrentActiveModule);

            QueryResult result = binding.query(query);

            if (result.records != null)
            {
                foreach (var record in result.records)
                {
                    var ticket = (Case)record;

                    tickets.Add(new SupportTicketSalesForce
                                {
                                    Identifier = ticket.Id,
                                    CaseNumber = ticket.CaseNumber,
                                    Status = ticket.Status,
                                    Subject = ticket.Subject,
                                    CreatedOn = ticket.CreatedDate.HasValue ? ticket.CreatedDate.Value : DateTime.UtcNow,
                                    ModifiedOn = ticket.LastModifiedDate.HasValue ? ticket.LastModifiedDate.Value : DateTime.UtcNow
                                });
                }
            }

            return tickets;
        }
        
        #endregion Methods
    }
}
