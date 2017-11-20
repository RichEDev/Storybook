CREATE PROCEDURE UpdateTeamEmployees
	@teamId int,
	@toRemove [dbo].[IntPK] READONLY, 
	@toAdd [dbo].[IntPK] READONLY,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	-- declare vars
	DECLARE @eid int;
	DECLARE @employeeFullName nvarchar(max) = [dbo].getEmployeeFullName(@CUemployeeID);
	DECLARE removeLoop CURSOR LOCAL FAST_FORWARD FOR
		SELECT c1 AS 'eid'
		FROM @toRemove;
	DECLARE addLoop CURSOR LOCAL FAST_FORWARD FOR 
		SELECT c1 AS 'eid'
		FROM @toAdd;
	
	
	-- Remove items if they exist.
	OPEN removeLoop
	FETCH NEXT FROM removeLoop INTO @eid
	WHILE @@FETCH_STATUS = 0 BEGIN		
	
		DELETE FROM [dbo].[teamemps] 
			WHERE [dbo].[teamemps].[teamid] = @teamId 
			AND [dbo].[teamemps].[employeeid] = @eid;
		exec addDeleteEntryWithValueToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamId, @eid, @employeeFullName, null;	
	
		FETCH NEXT FROM removeLoop INTO @eid;
	END
	CLOSE removeLoop
	DEALLOCATE removeLoop


	-- Add items if they don't exist.
	OPEN addLoop
	FETCH NEXT FROM addLoop INTO @eid
	WHILE @@FETCH_STATUS = 0 BEGIN
		
		IF NOT EXISTS 
			(SELECT * FROM [dbo].[teamemps]
			WHERE [dbo].[teamemps].[teamid] = @teamId 
			AND [dbo].[teamemps].[employeeid] = @eid)		
		BEGIN
			INSERT INTO [dbo].[teamemps]( teamid, employeeid)
			VALUES (@teamId, @eid);
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @eid, '992a7faf-fe98-4a0a-82fd-7c3cf10dd0dd', null, @teamId, @eid, null;
		END
	
		FETCH NEXT FROM addLoop INTO @eid;
	END
	CLOSE addLoop
	DEALLOCATE addLoop
	
--	INSERT INTO [dbo].[teamemps]( teamid, employeeid) select @teamId, c1 from @toAdd where c1 not in (select employeeid from teamemps where teamid=@teamId);
END
GO
