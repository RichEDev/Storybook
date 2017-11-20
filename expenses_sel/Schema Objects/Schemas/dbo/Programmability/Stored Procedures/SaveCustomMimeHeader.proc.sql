
CREATE PROCEDURE [dbo].[SaveCustomMimeHeader] 
(
@mimeId uniqueidentifier, 
@fileExtension nvarchar(50), 
@mimeHeader nvarchar(150),
@description nvarchar(500),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;

DECLARE @recordTitle nvarchar(2000);

--Exists globally
SET @count = (SELECT COUNT(*) FROM [$(metabaseExpenses)].dbo.mime_headers WHERE fileExtension = @fileExtension);
IF @count > 0
BEGIN
	return -1
END

SET @count = (SELECT COUNT(*) FROM customMimeHeaders WHERE customMimeID = @mimeId);
IF @count = 0
BEGIN

	BEGIN
		--Exists Locally
		SET @count = (SELECT COUNT(*) FROM customMimeHeaders WHERE fileExtension = @fileExtension);
		
		IF @count > 0
		BEGIN
			return -2
		END
		
		INSERT INTO customMimeHeaders (customMimeID, fileExtension, mimeHeader, [description], createdOn, createdBy)
		VALUES (@mimeID, @fileExtension, @mimeHeader, @description, getutcdate(), @employeeId);
		
		set @recordTitle = 'Custom Attachment Type ID: ' + CAST(@mimeId AS nvarchar(50)) + ' (' + @fileExtension + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 159, 0, @recordTitle, null;
		
	END
END
ELSE
BEGIN
	--Exists Locally
	SET @count = (SELECT COUNT(*) FROM customMimeHeaders WHERE customMimeID <> @mimeId AND fileExtension = @fileExtension);
	
	IF @count > 0
	BEGIN
		return -2
	END

	DECLARE @oldfileExtension nvarchar(10);
	DECLARE @oldmimeHeader nvarchar(150);
	DECLARE @olddescription nvarchar(500);
	
	select @oldfileExtension = fileExtension, @oldmimeHeader = mimeHeader, @olddescription = [description] from customMimeHeaders where customMimeID = @mimeId;
	
	UPDATE customMimeHeaders SET fileExtension = @fileExtension, mimeHeader = @mimeHeader, [description] = @description, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE customMimeID = @mimeId;
	
	set @recordTitle = 'Custom Attachment Type ID: ' + CAST(@mimeId AS nvarchar(50)) + ' (' + @fileExtension + ')';

	if @oldfileExtension <> @fileExtension
		exec addUpdateEntryToAuditLog @employeeId, @delegateID, 159, 0, '56d4c3b1-1445-4f08-bdbd-405098f54b44', @oldfileExtension, @fileExtension, @recordtitle, null;
	if @oldmimeHeader <> @mimeHeader
		exec addUpdateEntryToAuditLog @employeeId, @delegateID, 159, 0, 'fa427b10-e063-4438-bc90-84f075105138', @oldmimeHeader, @mimeHeader, @recordtitle, null;
	if @olddescription <> @description
		exec addUpdateEntryToAuditLog @employeeId, @delegateID, 159, 0, '37349c8c-efee-4a44-83b5-233bac5d3d95', @olddescription, @description, @recordtitle, null;
END

return 0

