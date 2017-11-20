CREATE TABLE [dbo].[CorporateCardImportLog]
(
	[ImportId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[CardProviderId] INT NOT NULL, 
    [LogMessage] NVARCHAR(MAX) NOT NULL, 
    [Date] DATETIME NOT NULL, 
    [StatementId] INT NULL,
	[Status] TINYINT NOT NULL,
	[NumberOfErrors] INT NOT NULL
)
