CREATE TABLE [dbo].[SupportTicketAttachments]
(
	[SupportTicketAttachmentId] INT NOT NULL IDENTITY, 
    [SupportTicketId] INT NOT NULL, 
    [Filename] NVARCHAR(255) NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [Filedata] VARBINARY(MAX) NOT NULL, 
    CONSTRAINT [FK_SupportTicketAttachments_Employees] FOREIGN KEY ([CreatedBy]) REFERENCES [employees]([employeeid]), 
    CONSTRAINT [FK_SupportTicketAttachments_SupportTickets] FOREIGN KEY ([SupportTicketId]) REFERENCES [SupportTickets]([SupportTicketId]), 
    CONSTRAINT [PK_SupportTicketAttachments] PRIMARY KEY ([SupportTicketAttachmentId])
)
