CREATE PROCEDURE [dbo].[APIdeleteAddress]
	@addressid int
AS
	IF EXISTS (SELECT * FROM [AddressEsrAllocation] WHERE [AddressId] = @addressid AND (ESRLocationID IS NOT NULL OR ESRAddressID IS NOT NULL))
	BEGIN
		UPDATE [addresses] SET [Archived] = 1 WHERE [AddressID] = @addressid;
	END
	ELSE
	BEGIN
		DELETE FROM [addresses] WHERE [AddressID] = @addressid;
	END
	RETURN 0;