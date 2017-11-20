CREATE PROCEDURE [dbo].[ArchiveAddress]
 @AddressID int = 0,
 @UserID int, 
 @DelegateID int
AS

DECLARE @count INT;

IF (@AddressID > 0)
BEGIN
 -- Get and flip the current archived status
 DECLARE @Archived bit
 DECLARE @NewArchived bit
 SELECT @Archived = [Archived] from [dbo].[addresses] where [AddressID] = @AddressID;
 SELECT @NewArchived = @Archived ^ 1;
 
 DECLARE @RecordTitle NVARCHAR(2000);
 SET @RecordTitle = (SELECT [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [dbo].[addresses] WHERE [AddressID] = @AddressID);
 
 -- Perform the update
	UPDATE	[dbo].[addresses]
	SET		[Archived] = @NewArchived,
			[ModifiedOn] = GETUTCDATE(),
			[ModifiedBy] = CASE WHEN @DelegateID IS NULL THEN @UserID ELSE @DelegateID END
	WHERE	[AddressID] = @AddressID;
 
 -- Update the audit log
 exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @AddressID, '404b7297-ff10-45a1-8cf1-ea15ca0df949', @Archived, @NewArchived, @RecordTitle, NULL;
 
 RETURN 1;
END
ELSE
BEGIN
 RETURN -99;
END