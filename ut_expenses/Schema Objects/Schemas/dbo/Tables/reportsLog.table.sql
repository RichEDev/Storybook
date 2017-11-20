CREATE TABLE [dbo].[reportsLog]
(
	[reportsLogId] INT IDENTITY(1,1) NOT NULL, 
    [reportId] UNIQUEIDENTIFIER NULL, 
    [employeeId] INT NOT NULL, 
    [reportName] NVARCHAR(150) NOT NULL, 
    [baseTableId] UNIQUEIDENTIFIER NOT NULL, 
    [limit] SMALLINT NOT NULL, 
    [subAccountId] INT NULL, 
    [forClaimants] BIT NOT NULL, 
    [processedRows] INT NOT NULL, 
    [requestNumber] INT NOT NULL, 
    [rowCount] INT NOT NULL, 
    [processedPercentage] TINYINT NOT NULL, 
    [startedOn] DATETIME NOT NULL, 
    [endedOn] DATETIME NOT NULL, 
    [isFinancialExport] BIT NOT NULL,
    [sql] NVARCHAR(MAX) NOT NULL 
)
