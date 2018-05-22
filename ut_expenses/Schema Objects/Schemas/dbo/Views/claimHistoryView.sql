CREATE VIEW claimHistoryView
AS
SELECT claimhistory.claimhistoryid
	,claimhistory.claimid
	,dbo.claimhistory.datestamp
	,ISNULL(dbo.employees.title + ' ' + dbo.employees.firstname + ' ' + dbo.employees.surname, 'System') AS employee
	,dbo.claimhistory.comment
	,dbo.claimhistory.stage
	,dbo.claimhistory.refnum
FROM dbo.claimhistory
LEFT JOIN dbo.employees ON dbo.employees.employeeid = dbo.claimhistory.employeeid