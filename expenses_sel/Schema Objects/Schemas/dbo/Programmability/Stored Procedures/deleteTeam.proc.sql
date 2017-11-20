CREATE PROCEDURE [dbo].[deleteTeam]
@teamid int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	DECLARE @sql int;
	DECLARE @returnVal int;
	DECLARE @recordTitle nvarchar(2000);
	declare @subAccountId int;
	select @recordTitle = teamname, @subAccountId = subAccountId from teams where teamid = @teamid;

	SET @sql = (select count(*) from claims where teamid = @teamid);

	IF @sql > 0
	BEGIN
		SET @returnVal = 1;
		RETURN @returnVal;
	END

	SET @sql = (select count(*) from signoffs where signofftype = 3 and relid = @teamid);
	
	IF @sql > 0
	BEGIN
		SET @returnVal = 2;
		RETURN @returnVal;
	END

	SET @sql = (select count(*) from tasks where taskOwnerId = @teamid and taskOwnerType = 1);
	
	IF @sql > 0
	BEGIN
		SET @returnVal = 3;
		RETURN @returnVal;
	END

	-- Check that team is not used in audiences
	SET @sql = (select count(*) as teamCount from audienceTeams where teamID = @teamid);
	IF @sql > 0
	BEGIN
		SET @returnVal = 4;
		RETURN @returnVal;
	END
	
	DELETE FROM teams WHERE teamid = @teamid;
	
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamid, @recordTitle, @subAccountId;

	SET @returnVal = 0;

	RETURN @returnVal;
end
