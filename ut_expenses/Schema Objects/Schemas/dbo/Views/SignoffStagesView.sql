CREATE VIEW [dbo].[SignoffStagesView]
AS
SELECT signoffs.groupid
	,signoffid
	,stage
	,CASE signofftype
		WHEN 1
			THEN 'Budget Holder'
		WHEN 2
			THEN 'Employee'
		WHEN 3
			THEN 'Team'
		WHEN 4
			THEN 'Line Manager'
		WHEN 5
			THEN 'Determined by Claimant'
		WHEN 6
			THEN 'Approval Matrix'
		WHEN 7
			THEN 'Determined by Claimant from Approval Matrix'
		WHEN 8
			THEN 'Cost Code Owner'
		WHEN 9
			THEN 'Assignment Supervisor'
		WHEN 100
			THEN 'Scan & Attach'
		WHEN 101
			THEN 'Validation'
		END AS signofftype
	,CASE signofftype
		WHEN 1
			THEN budgetholders.budgetholder + ' (' + budgetholderowners.surname + ', ' + budgetholderowners.title + ' ' + budgetholderowners.firstname + ')'
		WHEN 2
			THEN employees.surname + ', ' + employees.title + ' ' + employees.firstname
		WHEN 3
			THEN teams.teamname
		WHEN 6
			THEN approvalMatrices.name
		WHEN 7
			THEN approvalMatrices.name
		END AS relid
	,CASE [include]
		WHEN 1
			THEN 'Always include stage'
		WHEN 2
			THEN 'Only include stage if claim amount is above ' + Cast(amount AS NVARCHAR(20))
		WHEN 3
			THEN 'Only if an item exceeds allowed amount'
		WHEN 4
			THEN 'Only if claim includes specified cost code'
		WHEN 5
			THEN 'Only include stage if claim amount is below  ' + Cast(amount AS NVARCHAR(20))
		WHEN 6
			THEN 'Only if claim include specified expense item'
		WHEN 7
			THEN 'Only if claim includes an expense item older than ' + Cast(amount AS NVARCHAR(20)) + ' days'
		WHEN 8
			THEN 'Only if claim includes specified department'
		WHEN 9
			THEN 'Only if an expense item fails validation twice'
		END AS [include]
	,CASE notify
		WHEN 1
			THEN 'Stage is notified of claim'
		WHEN 2
			THEN 'Stage is to check claim'
		END AS notify
FROM signoffs
LEFT JOIN budgetholders ON budgetholders.budgetholderid = signoffs.relid
LEFT JOIN employees AS budgetholderowners ON budgetholderowners.employeeid = budgetholders.employeeid
LEFT JOIN employees ON employees.employeeid = signoffs.relid
LEFT JOIN teams ON teams.teamid = signoffs.relid
LEFT JOIN approvalMatrices ON approvalMatrices.approvalMatrixId = signoffs.relid
