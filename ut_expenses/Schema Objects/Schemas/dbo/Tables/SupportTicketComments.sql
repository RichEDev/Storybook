CREATE TABLE [dbo].[SupportTicketComments]
(
	[SupportTicketCommentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SupportTicketId] INT NOT NULL, 
    [Body] NVARCHAR(4000) NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    CONSTRAINT [FK_SupportTicketComments_SupportTickets] FOREIGN KEY ([SupportTicketId]) REFERENCES [SupportTickets]([SupportTicketId]),
    CONSTRAINT [FK_SupportTicketComments_Employees] FOREIGN KEY ([CreatedBy]) REFERENCES [employees]([employeeid])
)
