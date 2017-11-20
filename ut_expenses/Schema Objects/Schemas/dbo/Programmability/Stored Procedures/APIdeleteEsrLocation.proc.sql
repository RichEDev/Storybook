CREATE procedure [dbo].[APIdeleteEsrLocation]
	@ESRLocationId BIGINT
AS
BEGIN
-- companyEsrAllocation
	DECLARE @addressId INT;
	SELECT @addressId = AddressId FROM AddressEsrAllocation WHERE ESRLocationID = @ESRLocationId;
-- FOREIGN KEYS
	UPDATE ESROrganisations SET ESRLocationId = NULL WHERE ESRLocationId = @ESRLocationId;

	UPDATE AddressEsrAllocation SET ESRLocationID = NULL WHERE AddressId = @addressId;

	IF EXISTS (SELECT * FROM AddressEsrAllocation WHERE AddressId = @addressId AND ESRLocationID IS NULL AND ESRAddressID IS NULL)
	BEGIN
		UPDATE addresses SET Archived = 1 WHERE AddressId = @addressId;
		DELETE FROM AddressEsrAllocation WHERE AddressId = @addressId;
	END

	UPDATE esr_assignments SET ESRLocationId = NULL WHERE ESRLocationId = @ESRLocationId;
	UPDATE ESRAssignmentLocations SET DeletedDateTime = getdate() WHERE ESRLocationId = @ESRLocationId;
-- DELETE
	DELETE FROM ESRLocations WHERE ESRLocationId = @ESRLocationId;
END