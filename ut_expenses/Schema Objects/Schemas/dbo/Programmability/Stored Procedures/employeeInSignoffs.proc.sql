CREATE PROCEDURE [dbo].[employeeInSignoffs]
	@employeeId INT
AS
BEGIN
	IF EXISTS (SELECT [signoffid] FROM [signoffs] where [signoffs].[signofftype] = 2 AND [signoffs].[relid] = @employeeid)
	BEGIN
		RETURN 1;
	END

	IF EXISTS (SELECT [signoffid] FROM [signoffs] INNER JOIN [budgetholders] ON [signoffs].[relid] = [budgetholders].[budgetholderid] WHERE [signoffs].[signofftype] = 1 AND [budgetholders].[employeeid] = @employeeid)					
	BEGIN
		RETURN 1;
	END
	
	IF EXISTS (SELECT [signoffid] FROM [signoffs] INNER JOIN [teamemps] ON [signoffs].[relid] = [teamemps].[teamid] WHERE [signoffs].[signofftype] = 3 AND [teamemps].[employeeid] = @employeeid)
	BEGIN
		RETURN 1;
	END
	
	IF EXISTS (SELECT [signoffs].[signoffid] FROM [signoffs] 
		INNER JOIN [approvalMatrices] AS am ON [signoffs].[relid] = am.[approvalMatrixId]
		INNER JOIN [approvalMatrixLevels] AS aml ON am.[approvalMatrixId] = aml.[approvalMatrixId]
		LEFT JOIN [budgetholders] AS bh ON am.[defaultApproverBudgetHolderId] = bh.[budgetholderid]
		LEFT JOIN [budgetholders] AS bhl ON aml.[approverBudgetHolderId] = bhl.[budgetholderid]
		LEFT JOIN [teamemps] AS te ON am.[defaultApproverTeamId] = te.[teamid]
		LEFT JOIN [teamemps] AS tel ON aml.[approverTeamId] = tel.[teamid]
		WHERE ([signoffs].[signofftype] = 6 OR [signoffs].[signofftype] = 7)
			AND (
				(am.[defaultApproverBudgetHolderId] IS NOT NULL AND bh.[employeeid] = @employeeid)
				OR
				(aml.[approverBudgetHolderId] IS NOT NULL AND bhl.[employeeid] = @employeeid)
				OR
				(am.[defaultApproverEmployeeId] IS NOT NULL AND am.[defaultApproverEmployeeId] = @employeeid)
				OR
				(aml.[approverEmployeeId] IS NOT NULL AND aml.[approverEmployeeId] = @employeeid)
				OR
				(am.[defaultApproverTeamId] IS NOT NULL AND te.[employeeid] = @employeeid)
				OR
				(aml.[approverTeamId] IS NOT NULL AND tel.[employeeid] = @employeeid)
			))
	BEGIN
		RETURN 1;
	END
			
	RETURN 0;
END