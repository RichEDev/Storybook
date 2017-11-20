

CREATE PROCEDURE [dbo].[CheckFinancialExportESRAssignments]

AS
BEGIN
	SELECT employees.firstname, employees.surname, employees.username, claims.name, [savedexpenses_previous].[date], [savedexpenses_previous].total, subcats.subcat  FROM employees inner join claims ON claims.employeeid = employees.employeeid inner join savedexpenses_previous ON savedexpenses_previous.claimid = claims.claimid inner join subcats on subcats.subcatid = savedexpenses_previous.subcatid WHERE esrAssignID is null
END

