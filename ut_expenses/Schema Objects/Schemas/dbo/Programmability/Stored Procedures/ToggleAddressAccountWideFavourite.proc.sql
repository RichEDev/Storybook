CREATE PROCEDURE [dbo].[ToggleAddressAccountWideFavourite]
	@AddressID INT,
	@UserID INT,
	@DelegateID INT
AS

DECLARE @fave BIT;
DECLARE @newFave BIT;
DECLARE @recordTitle NVARCHAR(2000) = NULL;
SELECT @recordTitle = [Line1] + ', ' + [City] + ', ' + [Postcode], @fave = [AccountWideFavourite] FROM [dbo].[addresses] WHERE [AddressID] = @AddressID;

IF @recordTitle IS NOT NULL
BEGIN
	SELECT @newFave = @fave ^ 1;
	 
	-- Perform the update
	UPDATE	[dbo].[addresses]
		SET		[AccountWideFavourite] = @newFave,
				[ModifiedOn] = GETUTCDATE(),
				[ModifiedBy] = COALESCE(@DelegateID, @UserID)
		WHERE	[AddressID] = @AddressID;
	
	-- Update the audit log
	EXEC addUpdateEntryToAuditLog @UserID, @DelegateID, 38, @AddressID, '489CE238-7975-452E-A93C-7BBDF371726C', @fave, @newFave, @recordTitle, NULL;
	
	RETURN 1;
END

RETURN -99;