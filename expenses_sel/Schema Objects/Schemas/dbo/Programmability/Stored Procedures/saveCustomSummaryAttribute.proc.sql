CREATE PROCEDURE [dbo].[saveCustomSummaryAttribute]
@attributeid int,
@entityid int,
@attribute_name nvarchar(250),
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
	SET @count = (select COUNT(attributeid) from custom_entity_attributes where attribute_name = @attribute_name and entityid = @entityid and attributeid <> @attributeid);
	
	if @count = 0
	begin
		declare @old_display_name nvarchar(250);
		declare @old_attribute_name nvarchar(250);
		declare @old_description nvarchar(4000);
		declare @old_related_entityid int;
		declare @pluralname nvarchar(1000);
		
		set @pluralname = (select plural_name from custom_entities where entityid = @entityid);
		
		select @old_attribute_name = attribute_name, @old_display_name = display_name, @old_description = description, @old_related_entityid = related_entity
		from custom_entity_attributes where attributeid = @attributeid;
		
		update custom_entity_attributes set attribute_name = @attribute_name, display_name=@display_name, description=@description, related_entity=@related_entityid, modifiedon=GETUTCDATE(), modifiedby=@CUemployeeID
		where attributeid=@attributeid;
		
		set @recordTitle = 'Summary Attribute ID: ' + CAST(@attributeid AS nvarchar(20)) + ' (' + @attribute_name + ')';
		
		-- Create attribute on the database so sql doesn't fail. This is a dummy field and no data is stored in it
		declare @sql nvarchar(2000);
		set @sql = 'if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = ''custom_' +  replace(@pluralname,' ', '_') + ''' AND COLUMN_NAME = ''att' + CAST(@attributeid as nvarchar) + ''') BEGIN ALTER TABLE dbo.[custom_' +  replace(@pluralname,' ', '_') + '] ADD [att' + CAST(@attributeid as nvarchar) + '] int NULL END';
		exec sp_executesql @sql;
		
		if @old_attribute_name <> @attribute_name
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @attributeid, 'F9A05E27-5CDC-4050-91D9-D4C3BD828C91', @old_attribute_name, @attribute_name, @recordtitle, null;
			
		if @old_display_name <> @display_name
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @attributeid, '7140CC2B-F60D-4503-AA3E-F9C40A3EBC53', @old_display_name, @display_name, @recordtitle, null;
			
		if @old_description <> @description
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @attributeid, 'FCFBA8DE-F850-46BA-8E1E-9ABAD9C6AE1B', @old_description, @description, @recordtitle, null;
			
		SET @retID = @attributeid;
	end
END
ELSE
BEGIN
	SET @count = (select COUNT(attributeid) from custom_entity_attributes where attribute_name = @attribute_name and entityid = @entityid);
	
	if @count = 0
	begin
		insert into custom_entity_attributes (entityid, attribute_name, display_name, description, fieldtype, related_entity, createdon, createdby)
		values (@entityid, @attribute_name, @display_name, @description, 15, @related_entityid, GETUTCDATE(), @CUemployeeID);
		
		SET @retID = SCOPE_IDENTITY();
		
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @retID, @recordTitle, null;
	end
END

RETURN @retID
