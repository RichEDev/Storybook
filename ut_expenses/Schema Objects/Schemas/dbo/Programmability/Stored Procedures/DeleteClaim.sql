CREATE PROCEDURE dbo.DeleteClaim @claimId INT
AS
BEGIN
	DELETE p
	FROM savedexpenses_journey_steps_passengers p
	WHERE EXISTS (
			SELECT *
			FROM savedexpenses
			WHERE claimid = @claimId
				AND expenseid = p.expenseid
			)

	UPDATE Receipts
	SET Deleted = 1, ClaimId = null
	WHERE ClaimId = @claimId

	DELETE
	FROM claims_base
	WHERE claimid = @claimId

	RETURN 0;
END