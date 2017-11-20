CREATE PROCEDURE [dbo].[UpdateItemCheckers]
(
	@claimid INT,
	@stage INT,
	@validatingOnly BIT = 0
)
AS
BEGIN
	DECLARE @claimantId INT;
	DECLARE @costcodeId INT;
	DECLARE @signoffGroupId INT;
	DECLARE @signoffType TINYINT;
	DECLARE @ownerBudgetHolderId INT;
	DECLARE @ownerEmployeeId INT;
	DECLARE @expenseid INT;
	DECLARE @checkerId INT;
	DECLARE @expIds IntPK;
	DECLARE @useAssignmentSupervisorForMissingCostCodeOwner bit = 0;

	SELECT @claimantId = employeeid FROM claims_base WHERE claimid = @claimid;
	SELECT @signoffGroupId = dbo.getGroupIdByClaim(@claimid, @claimantId);
	SELECT @signoffType = signofftype, @useAssignmentSupervisorForMissingCostCodeOwner = NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner FROM signoffs WHERE groupid = @signoffGroupId AND stage = @stage;

	IF @signoffType <> 8 -- CostCodeOwner
	BEGIN
		RETURN 0;
	END

	DECLARE @subAccountId INT = (SELECT TOP 1 subaccountid FROM accountsSubAccounts)
	DECLARE @defaultOwnerType INT;
	DECLARE @defaultOwnerId INT;
	DECLARE @ownerTeamId INT;
	DECLARE @currentItemCheckerId INT;
	DECLARE @doRollback BIT = 0;
	DECLARE @isTeamChecker BIT;

	EXEC dbo.GetDefaultCostCodeOwner @subAccountId, @defaultOwnerType OUT, @defaultOwnerId OUT;
	
	BEGIN TRANSACTION doUpdateItemCheckers

	DECLARE lp CURSOR FOR
	SELECT savedexpenses_costcodes.expenseid, savedexpenses_costcodes.costcodeid, ownerEmployeeId, ownerBudgetHolderId, ownerTeamId, savedexpenses.itemCheckerId
	FROM savedexpenses_costcodes
	LEFT JOIN costcodes ON savedexpenses_costcodes.costcodeid = costcodes.costcodeid
	INNER JOIN savedexpenses ON savedexpenses_costcodes.expenseid = savedexpenses.expenseid
	INNER JOIN claims_base ON savedexpenses.claimid = claims_base.claimid
	WHERE claims_base.claimid = @claimid
	ORDER BY savedexpenses.expenseid, percentused DESC
	OPEN lp
	FETCH NEXT FROM lp INTO @expenseid, @costcodeId, @ownerEmployeeId, @ownerBudgetHolderId, @ownerTeamId, @currentItemCheckerId
	WHILE @@FETCH_STATUS = 0
	BEGIN
	IF NOT EXISTS (SELECT c1 FROM @expIds WHERE c1 = @expenseid)
		BEGIN
			SET @checkerId = 0;
			SET @isTeamChecker = 0;
			
			declare @teamCheckerId int;
			set @teamCheckerId = null;

			IF @costcodeId IS NOT NULL
			BEGIN
				IF @ownerEmployeeId IS NOT NULL
				BEGIN
					SET @checkerId = @ownerEmployeeId;
				END
				ELSE IF @ownerBudgetHolderId IS NOT NULL
				BEGIN
					SET @checkerId = (SELECT employeeid FROM budgetholders WHERE budgetholderid = @ownerBudgetHolderId);
				END
				ELSE
				BEGIN
					IF @ownerTeamId is not null
					BEGIN
						SET @isTeamChecker = 1;
					END

					SET @checkerId = @currentItemCheckerId; -- leave the same as existing value (for returned items)
				END
			END

			IF @checkerId IS NULL AND @isTeamChecker = 0 AND @defaultOwnerId IS NOT NULL
			BEGIN
				IF @defaultOwnerType = 25 -- Employee
				BEGIN
					SET @checkerId = @defaultOwnerId;
				END

				IF @defaultOwnerType = 11 -- Budget Holder
				BEGIN
					SET @checkerId = (SELECT employeeid FROM budgetholders WHERE budgetholderid = @defaultOwnerId);
				END

				IF @defaultOwnerType = 49 -- Team
				BEGIN
					SET @isTeamChecker = 1;
					SET @checkerId = @currentItemCheckerId; -- leave the same as existing value (for returned items)
				END
			END

			IF (@checkerId IS NULL OR @checkerId = 0) AND @isTeamChecker = 0
			BEGIN
				IF (@useAssignmentSupervisorForMissingCostCodeOwner = 1)
				BEGIN

					declare @AssignmentSignOffOwner varchar(39)
					select
						@AssignmentSignOffOwner = esr_assignments.SignOffOwner
					from
						esr_assignments
							join savedexpenses on savedexpenses.esrAssignID = esr_assignments.esrAssignID
					where
						savedexpenses.expenseid = @expenseid;

					declare @elementType int;	-- SpendManagementElement
					declare @itemPrimaryID int;	-- EmployeeId, TeamId, etc.
					declare @pos as int = CHARINDEX(',', @AssignmentSignOffOwner);
					select
						@elementType = cast(substring(@AssignmentSignOffOwner, 1, @pos - 1) as int),
						@itemPrimaryID = cast(substring(@AssignmentSignOffOwner, @pos + 1, LEN(@AssignmentSignOffOwner)) as int);

					IF @elementType = 11 -- Budget Holder
					BEGIN
						select @checkerId = employeeid from budgetholders where budgetholderid = @itemPrimaryID;
					END

					IF @elementType = 25 -- Employee
					BEGIN
						SET @checkerId = @itemPrimaryID;
					END

					IF @elementType = 49 -- Team
					BEGIN
						SET @teamCheckerId = @itemPrimaryID;
					END

				END
				ELSE
				BEGIN
					SELECT @checkerId = linemanager FROM employees WHERE employeeid = @claimantId;
				END

				IF (@checkerId IS NULL
					OR @checkerId <= 0)
					and @teamCheckerId is null
				BEGIN
					SET @doRollback = 1;
				END
			END

			IF @validatingOnly = 0
			BEGIN
				UPDATE savedexpenses SET
					itemCheckerId = case when @checkerId = 0 then null else @checkerId end,
					itemCheckerTeamId = @teamCheckerId
				WHERE expenseid = @expenseid;
			END

			INSERT INTO @expIds VALUES (@expenseid);
		END

		FETCH NEXT FROM lp INTO @expenseid, @costcodeId, @ownerEmployeeId, @ownerBudgetHolderId, @ownerTeamId, @currentItemCheckerId
	END
	CLOSE lp
	DEALLOCATE lp

	IF @doRollback = 0 AND @validatingOnly = 0
	BEGIN
		COMMIT TRANSACTION doUpdateItemCheckers
		RETURN 0;
	END
	ELSE
	BEGIN
	
		if @validatingOnly = 1 and (@checkerId > 0 or @isTeamChecker = 1)
			begin
				ROLLBACK TRANSACTION doUpdateItemCheckers
				return -2
			end
		else
			begin
				ROLLBACK TRANSACTION doUpdateItemCheckers
				RETURN -1;
			end
	END
END
go

