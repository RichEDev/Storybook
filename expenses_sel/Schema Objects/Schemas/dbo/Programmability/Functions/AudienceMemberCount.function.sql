create function dbo.AudienceMemberCount (
@audienceID int,
@employeeID int
)
returns int
as
begin
	declare @countSum int

	set @countSum = (select count(audiencebudgetholderid) from audienceBudgetHolders where audienceBudgetHolders.audienceID = @audienceId and audienceBudgetHolders.budgetholderid in 
	(
		select budgetholders.budgetholderid from budgetholders where employeeid = @employeeID
	)
	)

	set @countSum = @countSum + (select count(audienceEmployeeID) from audienceEmployees where audienceEmployees.audienceID = @audienceID and audienceEmployees.employeeid = @employeeID)

	set @countSum = @countSum + (select count(audienceTeamID) from audienceTeams where audienceTeams.audienceID = @audienceID and audienceTeamID in
	(
		select teams.teamID from teams
		inner join teamemps on teamemps.teamid = teams.teamID 
		where teamemps.employeeid = @employeeID
	)
	)

	return @countSum
end
