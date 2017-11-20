CREATE PROCEDURE [dbo].[deleteTeam]
@teamid int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	DECLARE @count int;
	DECLARE @returnVal int;
	DECLARE @recordTitle nvarchar(2000);
	declare @subAccountId int;
	select @recordTitle = teamname, @subAccountId = subAccountId from teams where teamid = @teamid;

	SET @count = (select count(*) from claims where teamid = @teamid);

	IF @count > 0
	BEGIN
		RETURN 1;
	END

	SET @count = (select count(*) from signoffs where signofftype = 3 and relid = @teamid);
	
	IF @count > 0
	BEGIN
		RETURN 2;
	END

	SET @count = (select count(*) from tasks where taskOwnerId = @teamid and taskOwnerType = 1);
	
	IF @count > 0
	BEGIN
		RETURN 3;
	END

	-- Check that team is not used in audiences
	SET @count = (select count(*) as teamCount from audienceTeams where teamID = @teamid);
	IF @count > 0
	BEGIN
		RETURN 4;
	END

	SET @count = (select count(costcodeid) from costcodes where OwnerTeamId = @teamid);
	if @count > 0
	begin
		RETURN 5;
	end
	
	declare @teamTableId uniqueidentifier = (select tableid from tables where tablename = 'teams');
	exec @returnVal = dbo.checkReferencedBy @teamTableId, @teamid;
	if @returnVal <> 0
		return @returnVal;

	IF EXISTS (SELECT approvalMatrixId FROM approvalMatrices WHERE defaultApproverTeamId = @teamid) OR EXISTS (SELECT approvalMatrixLevelId FROM approvalMatrixLevels WHERE approverTeamId = @teamid)
		RETURN 6;

	DELETE FROM teams WHERE teamid = @teamid;
	
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamid, @recordTitle, @subAccountId;

	SET @returnVal = 0;

	RETURN @returnVal;
end