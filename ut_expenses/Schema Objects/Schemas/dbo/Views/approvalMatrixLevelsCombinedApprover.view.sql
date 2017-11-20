CREATE VIEW [dbo].[approvalMatrixLevelsCombinedApprover]
	AS 
	select 
		approvalMatrixId, 
		approvalMatrixLevelId, 
		approvalLimit, 
		CASE WHEN approverEmployeeId is not null THEN N'Employee' 
			 WHEN approverBudgetHolderId IS NOT NULL THEN N'Budget Holder' 
			 WHEN approverTeamId IS NOT NULL THEN N'Team' END AS approverType ,
		CASE WHEN approverEmployeeId IS NOT NULL THEN dbo.getEmployeeFullName(approverEmployeeId) 
			 WHEN approverBudgetHolderId IS NOT NULL THEN (SELECT budgetholder FROM budgetholders WHERE budgetholderid = approverBudgetHolderId) 
			 WHEN approverTeamId IS NOT NULL THEN (SELECT teamname FROM teams WHERE teamid = approverTeamId) END AS approver 
		FROM approvalMatrixLevels;
