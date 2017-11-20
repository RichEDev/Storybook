CREATE PROCEDURE [dbo].[ArchiveMimeType]
	@MimeID int,
	@SubAccountID int,
	@FileExtension nvarchar(50),
	@UserID int,
	@DelegateID int
AS
BEGIN
	DECLARE @Archived bit;
	
	SELECT @Archived = Archived FROM mimeTypes WHERE mimeID = @MimeID

	IF @Archived = 0
	BEGIN
		UPDATE mimeTypes SET archived = 1, modifiedOn = GETUTCDATE(), modifiedBy = @UserID WHERE mimeID = @MimeID
		exec addUpdateEntryToAuditLog @UserID, @DelegateID, 138, @MimeID, '456c9fe6-34cb-41d4-91ce-3fb34df17bca', 0, 1, @FileExtension, @SubAccountID;
	END
	ELSE
	BEGIN
		UPDATE mimeTypes SET archived = 0, modifiedOn = GETUTCDATE(), modifiedBy = @UserID WHERE mimeID = @MimeID
		exec addUpdateEntryToAuditLog @UserID, @DelegateID, 138, @MimeID, '456c9fe6-34cb-41d4-91ce-3fb34df17bca', 1, 0, @FileExtension, @SubAccountID;
	END
END
