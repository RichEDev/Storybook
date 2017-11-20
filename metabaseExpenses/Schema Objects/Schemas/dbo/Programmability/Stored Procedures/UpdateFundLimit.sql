-- Procedure to update new fund limit for the expedite customer by its id's.
CREATE PROCEDURE [dbo].UpdateFundLimit 
	@accountID int,
    @amount decimal

AS
BEGIN
	IF EXISTS (SELECT * FROM dbo.registeredusers WHERE accountid  = @accountID)
		BEGIN
			UPDATE registeredusers SET fundlimit=@amount WHERE accountid = @accountID
			SELECT fundlimit FROM registeredusers WHERE accountid=@accountID
		END
END
