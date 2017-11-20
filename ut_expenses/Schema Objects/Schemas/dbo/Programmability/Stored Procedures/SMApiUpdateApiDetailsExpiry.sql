CREATE PROCEDURE [dbo].[SMApiUpdateApiDetailsExpiry]
	@apiDetailsId INT,
	@expiryTime DATETIME
AS
BEGIN
	UPDATE ApiDetails SET ExpiryTime = @expiryTime
	WHERE ApiDetailsId = @apiDetailsId
END

