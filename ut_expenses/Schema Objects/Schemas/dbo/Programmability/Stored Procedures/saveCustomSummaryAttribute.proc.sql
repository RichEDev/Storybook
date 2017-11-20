CREATE PROCEDURE [dbo].[saveCustomSummaryAttribute]
@attributeid int,
@entityid int,
@display_name nvarchar(250),
@description nvarchar(4000),
@related_entityid int,
@CUemployeeID int,
@CUdelegateID int
AS
DECLARE @count int;
DECLARE @retID int;
DECLARE @recordTitle nvarchar(500);
SET @retID = 0;

IF @attributeid > 0
BEGIN
	SET @count = (select COUNT(attributeid) from [customEntityAttributes] where display_name = @display_name and entityid = @entityid and attributeid <> @attributeid);
	
	if @count = 0
	begin
		declare @old_display_name nvarchar(250);
		declare @old_description nvarchar(4000);
		declare @old_related_entityid int;
		declare @masterTableName nvarchar(250);
		
		set @masterTableName = (select masterTableName from [customEntities] where entityid = @entityid);
		
		select @old_display_name = display_name, @old_description = description, @old_related_entityid = related_entity
		from [customEntityAttributes] where attributeid = @attributeid;
		
		update [customEntityAttributes] set display_name=@display_name, description=@description, related_entity=@related_entityid, modifiedon=GETUTCDATE(), modifiedby=@CUemployeeID, CacheExpiry = GETUTCDATE()
		where attributeid=@attributeid;
		
		set @recordTitle = 'Summary Attribute ID: ' + CAST(@attributeid AS nvarchar(20)) + ' (' + @display_name + ')';
		
		-- Create attribute on the database so sql doesn't fail. This is a dummy field and no data is stored in it
		declare @sql nvarchar(2000);
		set @sql = 'if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = ''custom_' +  @masterTableName + ''' AND COLUMN_NAME = ''att' + CAST(@attributeid as nvarchar) + ''') BEGIN ALTER TABLE dbo.[custom_' +  @masterTableName + '] ADD [att' + CAST(@attributeid as nvarchar) + '] int NULL END';
		exec sp_executesql @sql;
		
		if @old_display_name <> @display_name
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @attributeid, '7140CC2B-F60D-4503-AA3E-F9C40A3EBC53', @old_display_name, @display_name, @recordtitle, null;
			
		if @old_description <> @description
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @attributeid, 'FCFBA8DE-F850-46BA-8E1E-9ABAD9C6AE1B', @old_description, @description, @recordtitle, null;
			
		SET @retID = @attributeid;
	end
END
ELSE
BEGIN
	SET @count = (select COUNT(attributeid) from [customEntityAttributes] where display_name = @display_name and entityid = @entityid);
	
	if @count = 0
	begin
		insert into [customEntityAttributes] (entityid, display_name, description, fieldtype, related_entity, createdon, createdby, CacheExpiry)
		values (@entityid, @display_name, @description, 15, @related_entityid, GETUTCDATE(), @CUemployeeID, GETUTCDATE());
		
		SET @retID = SCOPE_IDENTITY();
		
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @retID, @recordTitle, null;
	end
END

RETURN @retID
