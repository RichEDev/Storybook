CREATE FUNCTION [dbo].[getCEAudienceMemberCount] (@audienceID int, @employeeID int)
RETURNS int AS  
BEGIN
      DECLARE @count int = 0;
      
      if @audienceID is null
            return -1;
      
      select @count = sum(empCount) from (
      select count(employeeID) as empCount from budgetHolders where budgetHolderID in (select budgetHolderID from audienceBudgetHolders where audienceID = @audienceID) and employeeID = @employeeID
      union all
      select count(employeeID) as empCount from audienceEmployees where audienceID = @audienceID and employeeID = @employeeID
      union all
      select count(employeeID) as empCount from teamemps where teamid in (select teamid from audienceTeams where audienceID = @audienceID) and employeeID = @employeeID
      ) as unionEmps
      
      return @count;
END
