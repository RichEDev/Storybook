CREATE PROCEDURE [dbo].[ApproveClaim]
	@claimid int,
	@modifiedon datetime,
	@userid int,
	@payBeforeValidate bit
AS
	update savedexpenses set tempallow = 1, returned = 0 where claimid = @claimid
                
    update claims_base set approved = 1, status = 6, ModifiedOn = @modifiedon, ModifiedBy = @userid, PayBeforeValidate = @payBeforeValidate where claimid = @claimid
                
RETURN 0
