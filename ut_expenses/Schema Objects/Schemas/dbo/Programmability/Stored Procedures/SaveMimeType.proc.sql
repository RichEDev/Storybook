CREATE PROCEDURE [dbo].[SaveMimeType]

	@GlobalMimeID uniqueidentifier,
	@SubAccountID int,
	@FileExtension nvarchar(50),
	@UserID int,
	@DelegateID int
AS
	DECLARE @MimeID int;
	
BEGIN
	INSERT INTO mimeTypes (globalMimeID, subAccountID, createdOn, createdBy) 
	VALUES (@GlobalMimeID, @SubAccountID, GETUTCDATE(), @UserID)
	SET @MimeID = SCOPE_IDENTITY();

	EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 138, @MimeID, @FileExtension, @subAccountId;

	return @MimeID
END
