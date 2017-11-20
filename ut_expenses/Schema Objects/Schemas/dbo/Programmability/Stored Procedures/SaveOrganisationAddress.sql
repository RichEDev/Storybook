CREATE PROCEDURE [dbo].[SaveOrganisationAddress]
	@OrganisationID int = 0,
	@AddressID int,
	@UserID int, 
	@DelegateID int
AS

IF (NOT EXISTS( SELECT * 
				FROM dbo.[organisationAddresses] 
				WHERE [OrganisationID] = @OrganisationID AND [AddressID] = @AddressID)
			   )
BEGIN -- Insert New Record
	INSERT INTO [dbo].[organisationAddresses]
	(
		[OrganisationID],
		[AddressID]
	)
	VALUES
	(
		@OrganisationID,
		@AddressID
	)
	
	if @userid > 0
	BEGIN
		exec addInsertEntryToAuditLog @UserID, @DelegateID, 16, @AddressID, @OrganisationID, null;
	END
END