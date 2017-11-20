CREATE VIEW [dbo].[CustomEntityAudiences]
	AS 
select audienceBudgetHolders.audienceID, employeeid from budgetholders inner join audienceBudgetHolders on audienceBudgetHolders.budgetHolderID = budgetholders.budgetholderid
union all
select audienceEmployees.audienceID, audienceEmployees.employeeID from audienceEmployees
union all
select  audienceTeams.audienceID, employeeId from teamemps 
inner join audienceTeams on audienceTeams.teamID = teamemps.teamid
