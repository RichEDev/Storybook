CREATE PROCEDURE [dbo].[saveCustomEntityRelationship]
@attributeid INT,
@entityid int,
@displayname nvarchar(250),
@description nvarchar(4000),
@tooltip NVARCHAR(4000),
@date DateTime,
@userid INT,
@mandatory BIT,
@builtIn BIT,
@relatedtableid UNIQUEIDENTIFIER,
@relationshiptype TINYINT,
@viewid INT,
@relatedentity int,
@aliasTableID uniqueidentifier,
@displayFieldID uniqueidentifier,
@maxRows int
AS
DECLARE @count INT;
DECLARE @isAdd BIT;
DECLARE @otmBaseTableID UNIQUEIDENTIFIER = null;
DECLARE @otmTargetTableID UNIQUEIDENTIFIER = null;

IF (@attributeid = 0)
begin

	SET @count = (SELECT COUNT(*) FROM [customEntityAttributes] WHERE entityid = @entityid AND display_name = @displayname);
	IF @count > 0
	BEGIN
		RETURN -1;
	END
	
	SELECT @otmBaseTableID = tableid FROM customEntities WHERE entityid = @entityid;
	SELECT @otmTargetTableID = tableid FROM customEntities WHERE entityid = @relatedentity;
	IF EXISTS (SELECT * FROM dbo.FindFirstOneToManyPath(@otmTargetTableID, @otmBaseTableID))
	BEGIN
		RETURN -2;
	END
	
	insert into [customEntityAttributes] (entityid, display_name, [description], tooltip, mandatory, builtIn, fieldtype, createdby, createdon, relatedtable, viewid, relationshiptype, related_entity, aliasTableID, relationshipDisplayField, maxRows, CacheExpiry) values (@entityid, @displayname, @description, @tooltip, @mandatory, 9, @userid, @date, @relatedtableid, @viewid, @relationshiptype, @relatedentity, @aliasTableID, @displayFieldID, @maxRows, GETUTCDATE())
	set @attributeid = scope_identity()
	SET @isAdd = 1
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM [customEntityAttributes] WHERE entityid = @entityid AND display_name = @displayname AND attributeid <> @attributeid);
	IF @count > 0
		RETURN -1;

	DECLARE @OldBuiltIn BIT;
	SELECT @OldBuiltIn = BuiltIn FROM [customEntityAttributes] WHERE attributeid = @attributeid;
	IF (@OldBuiltIn = 1 AND @builtIn = 0)
	BEGIN
		SET @builtIn = 1;
	END

	update [customEntityAttributes] set display_name = @displayname, [description] = @description, tooltip = @tooltip, mandatory = @mandatory, modifiedby = @userid, modifiedon = @date, viewid = @viewid, relationshiptype = @relationshiptype, related_entity = @relatedentity, relatedtable = @relatedtableid, aliasTableID = @aliasTableID, relationshipDisplayField = @displayFieldID, maxRows = @maxRows, CacheExpiry = GETUTCDATE() where attributeid = @attributeid
	update customFields set [description] = @displayname, comment = @description, mandatory = @mandatory, amendedon = getdate() where field = 'att' + cast(@attributeid as nvarchar);
	SET @isAdd = 0
end

IF @isAdd = 1 and @attributeid > 0 --OR @attributename = 'CreatedBy' OR @attributename = 'ModifiedBy'
	BEGIN
		DECLARE @masterTableName NVARCHAR(250);
		declare @relatedtable nvarchar(250);
		DECLARE @sql NVARCHAR(4000);
		DECLARE @keyfield NVARCHAR(500);
		
		SELECT @masterTableName = masterTableName FROM [customEntities] WHERE entityid = @entityid
		
		IF @relationshiptype = 1 --Many to one
			BEGIN
				select @relatedtable = tablename from tables where tableid = @relatedtableid;
				SELECT @keyfield = field FROM fields INNER JOIN tables ON tables.primarykey = fields.fieldid WHERE tables.tableid = @relatedtableid;
		
				SET @sql = 'alter TABLE [custom_' + @masterTableName + '] add [att' + cast(@attributeid as nvarchar(10)) + '] int';
				EXECUTE sp_executesql @sql;
		
				SET @sql = 'ALTER TABLE [custom_' + @masterTableName + ']  WITH CHECK ADD  CONSTRAINT [FK_custom_' + @masterTableName + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar(10)) + '] FOREIGN KEY ([att' + cast(@attributeid as nvarchar(10)) + ']) REFERENCES [dbo].[' + @relatedtable + '] ([' + @keyfield + '])';
				EXECUTE sp_executesql @sql;

				-- Only call report config for n:1 as saveCustomEntitySystemView makes amended call for relationship type 2 (1:n)
				EXECUTE saveCustomAttributeReportConfig @attributeid, null, 0, @relationshiptype;
			END
		ELSE
			BEGIN
				select @relatedtable = tablename from tables where tableid = @relatedtableid;
				SELECT @keyfield = field FROM fields INNER JOIN tables ON tables.primarykey = fields.fieldid WHERE tables.tablename = 'custom_' + @masterTableName;
		
				SET @sql = 'alter TABLE [' + @relatedtable + '] add [att' + cast(@attributeid as nvarchar(10)) + '] int';
				EXECUTE sp_executesql @sql;
		
				SET @sql = 'ALTER TABLE [' + @relatedtable + ']  WITH CHECK ADD  CONSTRAINT [FK_custom_' + @masterTableName + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar(10)) + '] FOREIGN KEY ([att' + cast(@attributeid as nvarchar(10)) + ']) REFERENCES [dbo].[custom_' + @masterTableName + '] ([' + @keyfield + '])';
				EXECUTE sp_executesql @sql;
			END
	END
return @attributeid
