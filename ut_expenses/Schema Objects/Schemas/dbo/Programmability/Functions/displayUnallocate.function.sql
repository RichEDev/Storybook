CREATE FUNCTION [dbo].[displayUnallocate] (@groupid int, @stage tinyint, @claimid int) 
RETURNS bit
AS
BEGIN
	DECLARE @signofftype tinyint;
	DECLARE @relId INT;
	
	SELECT @signofftype = signofftype, @relId = relid from signoffs where groupid = @groupid and stage = @stage

	if @signofftype = 3
	begin
		return 1;
	end

	IF @signofftype = 6 -- Approval Matrix
	BEGIN
		DECLARE @isTeam INT;
		DECLARE @claimTotal MONEY;
		
		SELECT @claimTotal = [total] FROM [claims] WHERE [claimid] = @claimid;
		
		IF @claimTotal IS NOT NULL
		BEGIN
		
			IF EXISTS (SELECT [approvalMatrixLevelId] FROM [approvalMatrixLevels] WHERE [approvalMatrixId] = @relId AND [approvalLimit] >= @claimTotal) -- check that the claim would go to a level
			BEGIN
				SELECT TOP 1 @isTeam = [approverTeamId] FROM [approvalMatrixLevels] WHERE [approvalMatrixId] = @relId AND [approvalLimit] >= @claimTotal ORDER BY [approvalLimit] ASC;
				
				IF @isTeam IS NOT NULL -- is the level a team
				BEGIN
					RETURN 1;
				END
			END
			ELSE
			BEGIN
				SELECT TOP 1 @isTeam = [defaultApproverTeamId] FROM [approvalMatrices] WHERE [approvalMatrixId] = @relId;
				
				IF @isTeam IS NOT NULL -- is the default approver a team
				BEGIN
					RETURN 1;
				END
			END
			
		END
		
	END
	
	if @signofftype = 8 -- Cost Code Owner
	begin
		declare @defaultOwnerType int = 0;
		declare @defaultProperty nvarchar(20);
		select top 1 @defaultProperty = stringValue from accountProperties where stringKey = 'defaultCostCodeOwner';

		if @defaultProperty is not null and len(@defaultProperty) > 0
		begin
			declare @separatorIdx int = (CHARINDEX(',',@defaultProperty));
			if @separatorIdx > 1
			begin
				set @defaultOwnerType = CAST(SUBSTRING(@defaultProperty,1,@separatorIdx-1) as int);
			end
		end

		declare @count int;
		select @count = count(savedexpenses_costcodes.savedcostcodeid) from savedexpenses_costcodes
		inner join savedexpenses on savedexpenses_costcodes.expenseid = savedexpenses.expenseid
		left join costcodes on savedexpenses_costcodes.costcodeid = costcodes.costcodeid
		where savedexpenses.claimid = @claimId
		and
		(
			(savedexpenses_costcodes.costcodeid is null and @defaultOwnerType = 49)
			or 
			(
				savedexpenses_costcodes.costcodeid is not null and 
				(
					costcodes.OwnerTeamId is not null
					or
					(
						costcodes.OwnerTeamId is null and costcodes.OwnerEmployeeId is null and costcodes.OwnerBudgetHolderId is null and @defaultOwnerType = 49
					)
				)
			)
		)
	
		if @count > 0
		begin
			return 1;
		end
	end

	return 0;
END