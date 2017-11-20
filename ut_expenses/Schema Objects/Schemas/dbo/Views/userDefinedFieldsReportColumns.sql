CREATE VIEW [dbo].[userDefinedFieldsReportColumns]
AS
SELECT dbo.reports.reportID, dbo.reports.reportname AS [Report Name], dbo.reportcolumns.fieldID, 
               dbo.employees.title + ' ' + dbo.employees.firstname + ' ' + dbo.employees.surname AS [Report Owner], 
               dbo.accountsSubAccounts.description AS [Sub Account]
FROM  dbo.reportcolumns INNER JOIN
               dbo.reports ON dbo.reportcolumns.reportID = dbo.reports.reportID INNER JOIN
               dbo.employees ON dbo.reports.employeeid = dbo.employees.employeeid INNER JOIN
               dbo.accountsSubAccounts ON dbo.reports.subAccountId = dbo.accountsSubAccounts.subAccountID