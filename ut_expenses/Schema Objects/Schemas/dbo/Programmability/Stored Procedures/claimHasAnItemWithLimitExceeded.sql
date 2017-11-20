CREATE PROCEDURE [dbo].[claimHasAnItemWithLimitExceeded] @claimId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @hasLimitExceeded BIT

	IF (
			SELECT count(savedexpensesFlags.expenseid)
			FROM savedexpensesFlags
			INNER JOIN savedexpenses ON savedexpenses.expenseid = savedexpensesFlags.expenseid
			WHERE savedexpenses.claimid = @claimId
				AND (
					savedexpensesFlags.flagtype = 2
					OR savedexpensesFlags.flagType = 3
					)
			) = 0
		SET @hasLimitExceeded = 0
	ELSE
		SET @hasLimitExceeded = 1

	RETURN @hasLimitExceeded
END
