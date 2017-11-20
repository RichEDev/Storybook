CREATE VIEW [dbo].[claimHistoryView]
AS
SELECT     claimhistory.claimhistoryid, claimhistory.claimid, dbo.claimhistory.datestamp, dbo.employees.title + ' ' + dbo.employees.firstname + ' ' + dbo.employees.surname AS employee, 
                      dbo.claimhistory.comment, dbo.claimhistory.stage, dbo.claimhistory.refnum
FROM         dbo.claimhistory LEFT OUTER JOIN
                      dbo.employees ON dbo.employees.employeeid = dbo.claimhistory.employeeid




GO