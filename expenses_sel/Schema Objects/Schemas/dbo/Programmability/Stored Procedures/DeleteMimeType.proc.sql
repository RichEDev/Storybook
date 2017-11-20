CREATE PROCEDURE [dbo].[DeleteMimeType]
	@MimeID int,
	@SubAccountID int,
	@FileExtension nvarchar(50),
	@UserID int,
	@DelegateID int
AS
	DECLARE @Count int;
BEGIN
	-- ####Check car attachments
	SET @Count = (SELECT count(mimeID) FROM cars_attachments WHERE mimeID = @MimeID)

	IF @Count > 0
	BEGIN
		return -1
	END
	
	-- ####Check employee attachments
	SET @Count = (SELECT count(mimeID) FROM employee_attachments WHERE mimeID = @MimeID)

	IF @Count > 0
	BEGIN
		return -2
	END
	
	-- ####Check email template attachments
	SET @Count = (SELECT count(mimeID) FROM emailTemplate_attachments WHERE mimeID = @MimeID)

	IF @Count > 0
	BEGIN
		return -3
	END
	
	-- ####Check custom entity attachments
	DECLARE @tablename nvarchar(250);
	DECLARE @name nvarchar(250);
	DECLARE @parmDefinition nvarchar(50);
	DECLARE @sql nvarchar(MAX);
	
	DECLARE loop_cursor CURSOR FOR
	SELECT plural_name FROM custom_entities
	OPEN loop_cursor
	FETCH NEXT FROM loop_cursor INTO @name
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @tablename = 'custom_' + replace(@name,' ', '_') + '_attachments'
		IF exists (SELECT * FROM information_schema.tables WHERE table_name = @tablename)
		BEGIN
			SET @sql = 'SET @returnCount = (SELECT count(mimeID) FROM ' + @tablename + ' WHERE mimeID = @MimeTypeID)'
			SET @parmDefinition = '@MimeTypeID int, @returnCount int OUTPUT'
			EXECUTE sp_executesql @sql, @parmDefinition, @MimeTypeID = @MimeID, @returnCount = @Count OUTPUT
	
			IF @Count > 0
			BEGIN
				return -4
			END
		END

		FETCH NEXT FROM loop_cursor INTO @name		
	END
				
	CLOSE loop_cursor
	DEALLOCATE loop_cursor
	
	DELETE FROM mimeTypes WHERE mimeID = @MimeID;
	
	EXEC addDeleteEntryToAuditLog @UserID, @DelegateID, 138, @MimeID, @FileExtension, @SubAccountID;
	
	RETURN 0
END
