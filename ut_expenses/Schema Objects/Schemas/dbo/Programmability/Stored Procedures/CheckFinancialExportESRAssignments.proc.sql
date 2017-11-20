

CREATE PROCEDURE [dbo].[CheckFinancialExportESRAssignments]

AS
BEGIN
	SELECT employees.firstname, employees.surname, employees.username, claims.name, [savedexpenses].[date], [savedexpenses].total, subcats.subcat  
	FROM employees inner join claims ON claims.employeeid = employees.employeeid inner join savedexpenses ON savedexpenses.claimid = claims.claimid 
	inner join subcats on subcats.subcatid = savedexpenses.subcatid WHERE esrAssignID is null and savedexpenses.tempallow = 1 and savedexpenses.itemtype = 1
END

