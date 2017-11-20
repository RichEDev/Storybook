CREATE PROCEDURE [dbo].[EditEnvelopeBatch]
	@envelopes EnvelopeBatchEdit readonly
AS
BEGIN
	SET CONCAT_NULL_YIELDS_NULL OFF;
	UPDATE t
	SET 
		t.AccountId = (CASE WHEN e.AccountId IS NOT NULL THEN e.AccountId ELSE t.AccountId END),
		t.ClaimId = (CASE WHEN e.ClaimId IS NOT NULL THEN e.ClaimId ELSE t.ClaimId END), 
		t.EnvelopeNumber = (CASE WHEN t.EnvelopeNumber IS NOT NULL THEN e.EnvelopeNumber ELSE t.EnvelopeNumber END), 
		t.CRN = (CASE WHEN e.CRN IS NOT NULL THEN e.CRN ELSE t.CRN END), 
		t.EnvelopeStatus = (CASE WHEN e.EnvelopeStatus IS NOT NULL THEN e.EnvelopeStatus ELSE t.EnvelopeStatus END), 
		t.EnvelopeType = (CASE WHEN e.EnvelopeType IS NOT NULL THEN e.EnvelopeType ELSE t.EnvelopeType END), 
		t.DateIssuedToClaimant = (CASE WHEN e.DateIssuedToClaimant IS NOT NULL THEN e.DateIssuedToClaimant ELSE t.DateIssuedToClaimant END), 
		t.DateAssignedToClaim = (CASE WHEN e.DateAssignedToClaim IS NOT NULL THEN e.DateAssignedToClaim ELSE t.DateAssignedToClaim END), 
		t.DateReceived = (CASE WHEN e.DateReceived IS NOT NULL THEN e.DateReceived ELSE t.DateReceived END),
		t.DateAttachCompleted = (CASE WHEN e.dateAttachCompleted IS NOT NULL THEN e.dateAttachCompleted ELSE t.DateAttachCompleted END),
		t.DateDestroyed = (CASE WHEN e.dateDestroyed IS NOT NULL THEN e.dateDestroyed ELSE t.DateDestroyed END),
		t.OverpaymentCharge = (CASE WHEN e.OverpaymentCharge IS NOT NULL THEN e.OverpaymentCharge ELSE t.OverpaymentCharge END), 
		t.PhysicalStateProofUrl = (CASE WHEN e.PhysicalStateProofUrl IS NOT NULL THEN e.PhysicalStateProofUrl ELSE t.PhysicalStateProofUrl END), 
		t.LastModifiedBy = (CASE WHEN e.LastModifiedBy IS NOT NULL THEN e.LastModifiedBy ELSE t.LastModifiedBy END),
		t.DeclaredLostInPost = (CASE WHEN e.DeclaredLostInPost IS NOT NULL THEN e.DeclaredLostInPost ELSE t.DeclaredLostInPost END)
	FROM @envelopes e
	INNER JOIN [dbo].[Envelopes] t
	ON t.EnvelopeId = e.EnvelopeId;
	SET CONCAT_NULL_YIELDS_NULL ON;
END