CREATE PROCEDURE [dbo].[APIdeleteEsrAddress]
  @ESRAddressId BIGINT 
 
AS
-- companyEsrAllocation
	DECLARE @addressId INT;
	SELECT @addressId = AddressId FROM AddressEsrAllocation WHERE ESRAddressID = @ESRAddressId;
-- null foreign keys
	UPDATE AddressEsrAllocation SET ESRAddressID = NULL WHERE AddressId = @addressId;

	IF EXISTS (SELECT * FROM AddressEsrAllocation WHERE AddressId = @addressId AND ESRLocationID IS NULL AND ESRAddressID IS NULL)
	BEGIN
		UPDATE addresses SET Archived = 1 WHERE AddressID = @addressId;
		DELETE FROM AddressEsrAllocation WHERE AddressId = @addressId;
	END

-- delete record
	DELETE FROM ESRAddresses WHERE ESRAddressId = @ESRAddressId