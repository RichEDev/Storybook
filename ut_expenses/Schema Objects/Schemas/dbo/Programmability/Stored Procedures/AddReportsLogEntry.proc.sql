CREATE PROCEDURE [dbo].[AddReportsLogEntry]

	@reportId UNIQUEIDENTIFIER,
	@employeeId INT,
	@reportName NVARCHAR(150),
	@baseTableId UNIQUEIDENTIFIER,
	@limit SMALLINT,
	@subAccountId INT,
	@forClaimants BIT,
	@requestNumber INT,
	@isFinancialExport BIT,
	@sql NVARCHAR(MAX)

AS
BEGIN

INSERT INTO [dbo].[reportsLog]
       ([reportId]
       ,[employeeId]
       ,[reportName]
       ,[baseTableId]
       ,[limit]
       ,[subAccountId]
       ,[forClaimants]
       ,[requestNumber]
       ,[startedOn]
       ,[isFinancialExport]
       ,[sql])
VALUES
       (@reportId
       ,@employeeId
       ,@reportName
       ,@baseTableId
       ,@limit
       ,@subAccountId
       ,@forClaimants
       ,@requestNumber
       ,CURRENT_TIMESTAMP
       ,@isFinancialExport
       ,@sql)

END
