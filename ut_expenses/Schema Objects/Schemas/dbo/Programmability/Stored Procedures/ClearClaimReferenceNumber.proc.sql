CREATE PROCEDURE [ClearClaimReferenceNumber]
	@claimId INT
AS
	UPDATE claims_base 
	SET ReferenceNumber = NULL
	WHERE claimid = @claimId
RETURN 0
