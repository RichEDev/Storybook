CREATE TABLE [dbo].[SpendManagementApiLog]
(
	[Id] INT NOT NULL CONSTRAINT PK_SpendManagementApiLog PRIMARY KEY,
	[EmployeeId] INT NOT NULL CONSTRAINT FK_SpendManagementApiLog_Employees FOREIGN KEY REFERENCES [employees],
    [UserName] NVARCHAR(50) NOT NULL, 
    [Uri] NVARCHAR(MAX) NOT NULL, 
    [Result] NVARCHAR(MAX) NULL
)

GO

CREATE INDEX [IX_SpendManagementApiLog_EmployeeId] ON [dbo].[SpendManagementApiLog] ([EmployeeId])
