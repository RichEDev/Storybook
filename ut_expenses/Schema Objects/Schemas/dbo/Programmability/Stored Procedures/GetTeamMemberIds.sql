CREATE PROCEDURE [dbo].[GetTeamMemberIds]
	@teamId int
AS
BEGIN
	SELECT employeeid FROM teamemps WHERE teamid = @teamId
END
