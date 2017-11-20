CREATE VIEW [dbo].[signOffEntities]
AS
	SELECT '25,' + CAST(employeeid as NVARCHAR) as id, dbo.getEmployeeFullName(employeeid) + ' (' + employees.username + ') (Employee)' as Name FROM employees WHERE username NOT LIKE 'Admin%' AND archived = 0 AND active = 1
	UNION
	SELECT '11,' + CAST(budgetholderid AS NVARCHAR) AS id, budgetholder + ' (Budget Holder)' AS Name FROM budgetholders
	UNION
	SELECT '49,' + CAST(teamid AS nvarchar) AS id, teamname +  ' (Team)' AS Name FROM teams