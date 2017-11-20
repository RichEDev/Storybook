CREATE PROCEDURE [dbo].[SaveFavourite]
	@FavouriteID int = 0,
	@EmployeeID int,
	@AddressID int,
	@UserID int, 
	@DelegateID int
AS

IF (@FavouriteID = 0)
BEGIN -- Insert New Record
	INSERT INTO [dbo].[favourites]
	(
		[EmployeeID],
		[AddressID]
	)
	VALUES
	(
		@EmployeeID,
		@AddressID
	)
	
	set @FavouriteID = scope_identity();
	
	if @userid > 0
	BEGIN
		exec addInsertEntryToAuditLog @UserID, @DelegateID, 38, @FavouriteID, @AddressID, null;
	END
END

ELSE --Update Existing Record
BEGIN
	-- Create a backup of existing data
	DECLARE	@oldEmployeeID nvarchar(32)
	DECLARE	@oldAddressID nvarchar(256)
	
	SELECT	@oldEmployeeID = [EmployeeID],
			@oldAddressID = [AddressID]
	FROM	[favourites]
	WHERE	[FavouriteID] = @FavouriteID;

	-- Perform the update
	UPDATE	[dbo].[favourites]
	SET		[EmployeeID] = @EmployeeID,
			[AddressID] = @AddressID
	WHERE	[FavouriteID] = @FavouriteID;
	
	-- Update the audit log
	if (@userid > 0)
	BEGIN

		if (@oldEmployeeID <> @EmployeeID)
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @FavouriteID, '7dc642bc-0421-460a-867f-62b5822ca77a', @oldEmployeeID, @EmployeeID, @FavouriteID, NULL;

		if (@oldAddressID <> @AddressID)
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @FavouriteID, '2ba5481d-219f-470c-8001-e079eae4d76e', @oldAddressID, @AddressID, @FavouriteID, NULL;

	END
END

return @FavouriteID;