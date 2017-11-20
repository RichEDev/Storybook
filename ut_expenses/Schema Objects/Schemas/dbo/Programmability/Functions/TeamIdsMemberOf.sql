CREATE FUNCTION [dbo].[TeamIdsMemberOf](@employeeId INT)
RETURNS @teamIdList table ([teamid] int) AS  
BEGIN
	insert into @teamIdList
	select teams.teamid from teams
	inner join teamemps on teams.teamid = teamemps.teamid
	where employeeid = @employeeId
	
	return
END