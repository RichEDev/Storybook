-- this method only updates the fields that you pass without overwriting everything will null.
CREATE PROCEDURE [dbo].[EditEnvelope]
	@envelopeId int,
	@accountId int = null,
	@claimId int = null,
	@envelopeNumber nvarchar(10) = null,
	@crn nvarchar(12) = null,
	@envelopeStatus tinyint,
	@envelopeType int,
	@dateIssuedToClaimant DateTime = null,
	@dateAssignedToClaim DateTime = null,
	@dateReceived DateTime = null,
	@dateAttachCompleted DateTime = null,
	@dateDestroyed DateTime = null,
	@overpaymentCharge decimal(16,2) = null,
	@physicalStateProofUrl nvarchar(100) = null,
	@lastModifiedBy int,
	@declaredLostInPost bit
AS
BEGIN
	SET CONCAT_NULL_YIELDS_NULL OFF;
	UPDATE [dbo].[Envelopes]
	SET 
		AccountId = (CASE WHEN @accountId IS NOT NULL THEN @accountId ELSE AccountId END),
		ClaimId = (CASE WHEN @claimId IS NOT NULL THEN @claimId ELSE ClaimId END), 
		EnvelopeNumber = (CASE WHEN @envelopeNumber IS NOT NULL THEN @envelopeNumber ELSE EnvelopeNumber END), 
		CRN = (CASE WHEN @crn IS NOT NULL THEN @crn ELSE CRN END), 
		EnvelopeStatus = (CASE WHEN @envelopeStatus IS NOT NULL THEN @envelopeStatus ELSE EnvelopeStatus END), 
		EnvelopeType = (CASE WHEN @envelopeType IS NOT NULL THEN @envelopeType ELSE EnvelopeType END), 
		DateIssuedToClaimant = (CASE WHEN @dateIssuedToClaimant IS NOT NULL THEN @dateIssuedToClaimant ELSE DateIssuedToClaimant END), 
		DateAssignedToClaim = (CASE WHEN @dateAssignedToClaim IS NOT NULL THEN @dateAssignedToClaim ELSE DateAssignedToClaim END), 
		DateReceived = (CASE WHEN @dateReceived IS NOT NULL THEN @dateReceived ELSE DateReceived END), 
		DateAttachCompleted = (CASE WHEN @dateAttachCompleted IS NOT NULL THEN @dateAttachCompleted ELSE DateAttachCompleted END),
		DateDestroyed = (CASE WHEN @dateDestroyed IS NOT NULL THEN @dateDestroyed ELSE DateDestroyed END),
		OverpaymentCharge = (CASE WHEN @overpaymentCharge IS NOT NULL THEN @overpaymentCharge ELSE OverpaymentCharge END), 
		PhysicalStateProofUrl = (CASE WHEN @physicalStateProofUrl IS NOT NULL THEN @physicalStateProofUrl ELSE PhysicalStateProofUrl END), 
		LastModifiedBy = (CASE WHEN @lastModifiedBy IS NOT NULL THEN @lastModifiedBy ELSE LastModifiedBy END),
		DeclaredLostInPost = (CASE WHEN @declaredLostInPost IS NOT NULL THEN @declaredLostInPost ELSE DeclaredLostInPost END)
	WHERE EnvelopeId = @envelopeId;
	SET CONCAT_NULL_YIELDS_NULL ON;
END