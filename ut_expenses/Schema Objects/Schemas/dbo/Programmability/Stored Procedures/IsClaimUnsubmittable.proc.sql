CREATE PROCEDURE [dbo].[IsClaimUnsubmittable]
	@claimId int,
	@approverId int
AS
BEGIN
DECLARE @splitApprovalStage BIT = 0;
	DECLARE @stage INT = 0;
	SELECT @splitApprovalStage = [splitApprovalStage], @stage = [stage] FROM [claims_base] WHERE [claimid] = @claimId;

	-- failure conditions
	IF EXISTS (SELECT * FROM claims_base WHERE claimid = @claimId AND PayBeforeValidate = 1)
	BEGIN
		RETURN -4;
	END
	
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
	
	RETURN 0;
END