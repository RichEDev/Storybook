CREATE TABLE [dbo].[SupportTickets]
(
	[SupportTicketId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Subject] NVARCHAR(250) NOT NULL, 
    [Description] NVARCHAR(4000) NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [ModifiedOn] DATETIME NULL, 
    [ModifiedBy] INT NULL, 
    [Status] TINYINT NOT NULL, 
    [Owner] INT NOT NULL, 
    CONSTRAINT [FK_SupportTickets_CreatedBy_Employees] FOREIGN KEY ([CreatedBy]) REFERENCES [employees]([employeeid]), 
    CONSTRAINT [FK_SupportTickets_ModifiedBy_Employees] FOREIGN KEY ([ModifiedBy]) REFERENCES [employees]([employeeid]), 
    CONSTRAINT [FK_SupportTickets_Owner_Employees] FOREIGN KEY ([Owner]) REFERENCES [employees]([employeeid]), 
    CONSTRAINT [CK_SupportTickets_Status] CHECK ([Status] >= 1 AND [Status] <= 6)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'(1 = New, 2 = In Progress, 3 = Pending Administrator Response, 4 = Pending Employee Response, 5 = On Hold, 6 = Closed)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SupportTickets',
    @level2type = N'COLUMN',
    @level2name = N'Status'