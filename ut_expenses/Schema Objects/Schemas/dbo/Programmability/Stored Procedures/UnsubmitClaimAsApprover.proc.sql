CREATE PROCEDURE [dbo].[UnsubmitClaimAsApprover]
	@claimId INT,
	@approverId INT,
	@historyEmployeeId INT,
	@modifiedOn DATETIME,
	@reason NVARCHAR(4000)
AS
BEGIN
	DECLARE @splitApprovalStage BIT = 0;
	DECLARE @stage INT = 0;
	SELECT @splitApprovalStage = [splitApprovalStage], @stage = [stage] FROM [claims_base] WHERE [claimid] = @claimId;

	-- failure conditions
	IF @splitApprovalStage = 1 AND EXISTS (SELECT * FROM [savedexpenses] WHERE [claimid] = @claimId AND [tempallow] = 1 AND [itemCheckerId] <> @approverId AND [primaryitem] = 1)
	BEGIN
		-- any approved items in the claim stops the claim being unsubmitted - SPLIT OTHER APPROVER
		RETURN -1;
	END
	ELSE IF EXISTS (SELECT * FROM [savedexpenses] WHERE [claimid] = @claimId AND [tempallow] = 1 AND [primaryitem] = 1)
	BEGIN
		-- any approved items in the claim stops the claim being unsubmitted - THIS APPROVER
		RETURN -2;
	END
	ELSE IF @splitApprovalStage = 1 AND EXISTS (SELECT * FROM [savedexpenses] WHERE [claimid] = @claimId AND [returned] = 0 AND [itemCheckerId] <> @approverId AND [primaryitem] = 1)
	BEGIN
		-- all approvers at this stage, other than the unsubmitting approver, must have rejected their items
		RETURN -3;
	END

	-- reset the expense items
	DELETE FROM [returnedexpenses] WHERE [expenseid] in (SELECT [expenseid] FROM [savedexpenses] WHERE [claimid] = @claimId);
	UPDATE [savedexpenses] SET [tempallow] = 0, [returned] = 0, [itemCheckerId] = NULL WHERE [claimid] = @claimId;
	-- reset the claim
	UPDATE [claims_base] SET [datepaid] = NULL, [approved] = 0, [paid] = 0, [datesubmitted] = NULL, [status] = 0, [teamid] = NULL, [checkerid] = NULL, [stage] = 0, [submitted] = 0, [splitApprovalStage] = 0, [ModifiedOn] = @modifiedOn, [ModifiedBy] = @approverId WHERE [claimid] = @claimid;
	-- add the history entry
	INSERT INTO claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) VALUES (@claimId, GETDATE(), 'Claim unsubmitted by approver: ' + @reason, @stage, '', @historyEmployeeId);

	-- reset any advances
	UPDATE [floats] SET [settled] = 0 WHERE [floatid] IN (SELECT [floatid] FROM [float_allocations] WHERE [expenseid] IN (SELECT [expenseid] FROM [savedexpenses] WHERE [claimid] = @claimId));

	RETURN 0;
END