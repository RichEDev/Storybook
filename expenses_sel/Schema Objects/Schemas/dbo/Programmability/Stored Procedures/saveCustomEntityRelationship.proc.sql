CREATE PROCEDURE [dbo].[saveCustomEntityRelationship]
	@attributeid INT,
@entityid int,
@attributename nvarchar(250),
@displayname nvarchar(250),
@description nvarchar(4000),
@tooltip NVARCHAR(4000),
@date DateTime,
@userid INT,
@mandatory BIT,
@relatedtableid UNIQUEIDENTIFIER,
@relationshiptype TINYINT,
@formid INT,
@viewid INT,
@relatedentity int,
@aliasTableID uniqueidentifier--,
--@CUemployeeID INT,
--@CUdelegateID INT
AS
DECLARE @count INT;
DECLARE @isAdd BIT;

IF (@attributeid = 0)
begin
	SET @count = (SELECT COUNT(*) FROM [custom_entity_attributes] WHERE entityid = @entityid AND attribute_name = @attributename);
	IF @count > 0
		RETURN -1;
		
	insert into custom_entity_attributes (entityid, attribute_name, display_name, [description], tooltip, mandatory, fieldtype, createdby, createdon, relatedtable, formid, viewid, relationshiptype, related_entity, aliasTableID) values (@entityid, @attributename, @displayname, @description, @tooltip, @mandatory, 9, @userid, @date, @relatedtableid, @formid, @viewid, @relationshiptype, @relatedentity, @aliasTableID)
	set @attributeid = scope_identity()
	SET @isAdd = 1
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM [custom_entity_attributes] WHERE entityid = @entityid AND attribute_name = @attributename AND attributeid <> @attributeid);
	IF @count > 0
		RETURN -1;
	update custom_entity_attributes set display_name = @displayname, [description] = @description, tooltip = @tooltip, mandatory = @mandatory, modifiedby = @userid, modifiedon = @date, formid = @formid, viewid = @viewid, relationshiptype = @relationshiptype, related_entity = @relatedentity, relatedtable = @relatedtableid, aliasTableID = @aliasTableID where attributeid = @attributeid
	SET @isAdd = 0
end

IF @isAdd = 1 OR @attributename = 'CreatedBy' OR @attributename = 'ModifiedBy'
	BEGIN
		DECLARE @pluralname NVARCHAR(250);
		declare @relatedtable nvarchar(250);
		DECLARE @sql NVARCHAR(4000);
		DECLARE @keyfield NVARCHAR(500);
		
		IF @relationshiptype = 1 --Many to one
			BEGIN
				SELECT @pluralname = plural_name FROM custom_entities WHERE entityid = @entityid
				SET @pluralname = REPLACE(@pluralname,' ','_')
				select @relatedtable = tablename from tables where tableid = @relatedtableid;
				SELECT @keyfield = field FROM fields INNER JOIN tables ON tables.primarykey = fields.fieldid WHERE tables.tableid = @relatedtableid;
		
				SET @sql = 'alter TABLE [custom_' + @pluralname + '] add [att' + cast(@attributeid as nvarchar(10)) + '] int';
				EXECUTE sp_executesql @sql;
		
				SET @sql = 'ALTER TABLE [custom_' + @pluralname + ']  WITH CHECK ADD  CONSTRAINT [FK_custom_' + @pluralname + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar(10)) + '] FOREIGN KEY ([att' + cast(@attributeid as nvarchar(10)) + ']) REFERENCES [dbo].[' + @relatedtable + '] ([' + @keyfield + '])';
				EXECUTE sp_executesql @sql;
			END
		ELSE
			BEGIN
				SELECT @pluralname = plural_name FROM custom_entities WHERE entityid = @entityid
				SET @pluralname = REPLACE(@pluralname,' ','_')
				select @relatedtable = tablename from tables where tableid = @relatedtableid;
				SELECT @keyfield = field FROM fields INNER JOIN tables ON tables.primarykey = fields.fieldid WHERE tables.tablename = @pluralname;
		
				SET @sql = 'alter TABLE [' + @relatedtable + '] add [att' + cast(@attributeid as nvarchar(10)) + '] int';
				EXECUTE sp_executesql @sql;
		
				SET @sql = 'ALTER TABLE [' + @relatedtable + ']  WITH CHECK ADD  CONSTRAINT [FK_custom_' + @pluralname + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar(10)) + '] FOREIGN KEY ([att' + cast(@attributeid as nvarchar(10)) + ']) REFERENCES [dbo].[' + @pluralname + '] ([' + @keyfield + '])';
				EXECUTE sp_executesql @sql;
			END

		EXECUTE saveCustomAttributeReportConfig @attributeid, null, 0, @relationshiptype;
	END
return @attributeid
