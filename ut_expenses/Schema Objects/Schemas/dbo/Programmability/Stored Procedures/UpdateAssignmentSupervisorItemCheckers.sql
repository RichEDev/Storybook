CREATE PROCEDURE [dbo].[UpdateAssignmentSupervisorItemCheckers] (@claimId INT, @stage INT, @validatingOnly BIT)
AS
BEGIN
	DECLARE @signoffGroupId INT;
	DECLARE @signoffType TINYINT;
	DECLARE @claimantId INT;
	
	select @claimantId = employeeid from claims_base where claimid = @claimid;
	select @signoffGroupId = dbo.getGroupIdByClaim(@claimid, @claimantId);
	select @signoffType = signofftype from signoffs where groupid = @signoffGroupId and stage = @stage;
	
	if @signoffType <> 9 -- Assignment Supervisor
	begin
		return 0;
	end
	
	DECLARE @expenseid INT;
	DECLARE @signOffOwner varchar(39);
	DECLARE @doRollback BIT = 0;

	BEGIN TRANSACTION DoUpdatesToItemCheckers
	
	DECLARE lp CURSOR FOR
		SELECT
			savedexpenses.expenseid,
			esr_assignments.SignOffOwner
		from
			savedexpenses
				left join esr_assignments on esr_assignments.esrAssignID = savedexpenses.esrAssignID
		where
			savedexpenses.claimId = @claimId
	OPEN lp
	FETCH NEXT FROM lp INTO @expenseid, @signOffOwner
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @signOffOwner IS NOT NULL
		BEGIN
		
			declare @elementType int;	-- SpendManagementElement
			declare @itemPrimaryID int;	-- EmployeeId, TeamId, etc.
			declare @pos as int = CHARINDEX(',', @signOffOwner);
			select
				@elementType = cast(substring(@signOffOwner, 1, @pos - 1) as int),
				@itemPrimaryID = cast(substring(@signOffOwner, @pos + 1, LEN(@signOffOwner)) as int);
				
			declare @itemCheckerId int		= NULL;	-- EmployeeId
			declare @itemCheckerTeamId int	= NULL;	-- TeamId

			if @elementType = 11 -- BudgetHolder
			begin
				select @itemCheckerId = employeeid from budgetholders where budgetholderid = @itemPrimaryID;
			end
			else if @elementType = 25 -- Employee
			begin
				set @itemCheckerId = @itemPrimaryID;
			end
			else if @elementType = 49 -- Team
			begin
				set @itemCheckerTeamId = @itemPrimaryID;
			end
			else
			begin
				SET @doRollback = 1;
			end

			if @doRollback = 0 AND @validatingOnly = 0
				UPDATE
					savedexpenses
				SET
					itemCheckerId = @itemCheckerId,
					itemCheckerTeamId = @itemCheckerTeamId
				WHERE
					expenseid = @expenseid;
		END
		ELSE
		BEGIN
			SET @doRollback = 1;
		END
				
		FETCH NEXT FROM lp INTO @expenseid, @signOffOwner
	END
	CLOSE lp;
	DEALLOCATE lp;

	IF @doRollback = 1
	BEGIN
		ROLLBACK TRANSACTION DoUpdatesToItemCheckers
		RETURN -1; -- error code. Do not allow move to next stage
	END
	ELSE
	BEGIN
		IF @validatingOnly = 0
		BEGIN
			COMMIT TRANSACTION DoUpdatesToItemCheckers
		END
		ELSE
		BEGIN
			ROLLBACK TRANSACTION DoUpdatesToItemCheckers
		END
		RETURN 0
	END
END
