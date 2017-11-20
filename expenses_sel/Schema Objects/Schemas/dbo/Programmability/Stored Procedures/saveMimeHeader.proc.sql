CREATE PROCEDURE dbo.saveMimeHeader 
(
@mimeId int, 
@fileExtension nvarchar(10), 
@mimeHeader nvarchar(150),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @mimeId = 0
BEGIN
	SET @count = (SELECT COUNT(*) FROM mime_headers WHERE fileExtension = @fileExtension);
	
	IF @count = 0
	BEGIN
		INSERT INTO mime_headers (fileExtension, mimeHeader, createdOn, createdBy)
		VALUES (@fileExtension, @mimeHeader, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Attachment Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @fileExtension + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 138, @retVal, @recordTitle, null;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM mime_headers WHERE mimeId <> @mimeId AND fileExtension = @fileExtension);
	
	IF @count = 0
	BEGIN
		DECLARE @oldfileExtension nvarchar(10);
		DECLARE @oldmimeHeader nvarchar(150);
		
		select @oldfileExtension = fileExtension, @oldmimeHeader = mimeHeader from mime_headers where mimeId = @mimeId;
		
		UPDATE mime_headers SET fileExtension = @fileExtension, mimeHeader = @mimeHeader, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE mimeId = @mimeId;
		
		SET @retVal = @mimeId;
		
		set @recordTitle = 'Attachment Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @fileExtension + ')';

		if @oldfileExtension <> @fileExtension
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 138, @mimeId, 'B9DDCD77-41A9-4751-B86D-C98D80BC4B1E', @oldfileExtension, @fileExtension, @recordtitle, null;
		if @oldmimeHeader <> @mimeHeader
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 138, @mimeId, '40769DF5-9B72-4EBD-A9DF-4890E97194F3', @oldmimeHeader, @mimeHeader, @recordtitle, null;
	END
END

RETURN @retVal;
