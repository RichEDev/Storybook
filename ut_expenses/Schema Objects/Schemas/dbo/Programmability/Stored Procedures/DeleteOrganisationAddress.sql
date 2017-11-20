CREATE PROCEDURE [dbo].[DeleteOrganisationAddress]
	@OrganisationID int,
	@AddressID int,
	@UserID int,
	@DelegateID int
AS
	DELETE FROM [dbo].[organisationAddresses]
	WHERE [OrganisationID] = @OrganisationID AND [AddressID] = @AddressID
		
	IF (@userid > 0)
	BEGIN
		exec addDeleteEntryToAuditLog @UserID, @DelegateID, 16, @OrganisationID, 'Address Deleted', null;
	END
GO