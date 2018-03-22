CREATE PROCEDURE [dbo].[saveCustomEntityAttribute]
@attributeid INT,
@entityid int,
@displayname nvarchar(250),
@description nvarchar(4000),
@tooltip NVARCHAR(4000),
@mandatory BIT,
@displayInMobile BIT,
@fieldtype TINYINT,
@date DateTime,
@userid INT,
@maxlength INT,
@format TINYINT,
@defaultvalue NVARCHAR(50),
@precision TINYINT,
@workflowid int,
@isauditidentity bit,
@commentText nvarchar(4000),
@relationshipType tinyint,
@related_entityid int,
@isunique bit,
@populateExistingWithDefault bit,
@triggerAttributeId int,
@triggerJoinViaId int,
@triggerDisplayFieldId uniqueidentifier,
@relatedtable uniqueidentifier,
@boolAttribute bit = 0,
@builtIn bit,
@encrypted bit
AS
DECLARE @count INT;
DECLARE @masterTableName nvarchar(250) = (SELECT masterTableName FROM [customEntities] WHERE entityid = @entityid);

IF @isauditidentity = 1
BEGIN
	update [customEntityAttributes] set is_audit_identity = 0 WHERE entityid = @entityid;
END

if (@attributeid = 0)
BEGIN
	SET @count = (SELECT COUNT(*) FROM [customEntityAttributes] WHERE entityid = @entityid AND display_name = @displayname);
	IF @count > 0
		RETURN -1;
		
 insert into [customEntityAttributes] (entityid, display_name, [description], tooltip, mandatory, DisplayInMobile, fieldtype, createdby, createdon, maxlength, format, defaultvalue, [precision], workflowid, is_audit_identity, advicePanelText, related_entity, is_unique, TriggerAttributeId, TriggerJoinViaId, TriggerDisplayFieldId, CacheExpiry, relatedtable, BoolAttribute, [encrypted]) values (@entityid, @displayname, @description, @tooltip, @mandatory, @displayInMobile, @fieldtype, @userid, @date, @maxlength, @format, @defaultvalue, @precision, @workflowid, @isauditidentity, @commentText, @related_entityid, @isunique, @triggerAttributeId, @triggerJoinViaId, @triggerDisplayFieldId, GETUTCDATE(), @relatedtable, @boolAttribute, @encrypted)
	set @attributeid = scope_identity()
	
	IF @fieldtype not in (11, 19, 21)
		BEGIN
			DECLARE @sql NVARCHAR(4000);
			SET @sql = 'alter TABLE [custom_' + @masterTableName + '] add [att' + cast(@attributeid as nvarchar(10)) + ']';
  IF @fieldtype = 1 OR @fieldtype = 16 OR @fieldtype = 23
			begin
				SET @sql = @sql + ' nvarchar'
    
    IF @fieldtype = 23
    BEGIN
     SET @maxlength = 255;
    END
    
				if @maxlength is null
				begin
					set @maxlength = 4000;
				end
				
				SET @sql = @sql + '(' + CAST(@maxlength AS NVARCHAR(10)) + ')';
			END
		ELSE IF @fieldtype = 2 OR @fieldtype = 4 OR @fieldtype = 15 OR @fieldtype = 17
			BEGIN
				SET @sql = @sql + ' int';
			end	
		ELSE IF @fieldtype = 7
			BEGIN
				SET @sql = @sql + ' decimal(18,' + CAST(@precision AS NVARCHAR(4)) + ')';
			END
		ELSE IF @fieldtype = 6
			BEGIN
				SET @sql = @sql + ' money';
			end	
		ELSE IF @fieldtype = 5
			BEGIN
				SET @sql = @sql + ' bit';
			end	
		ELSE IF @fieldtype = 3
			BEGIN
				SET @sql = @sql + ' DateTime';
			end	
		ELSE IF @fieldtype = 10 OR @fieldtype = 18
			BEGIN
				SET @sql = @sql + ' nvarchar(max)';
			end
  ELSE IF @fieldtype = 22 
  BEGIN
    SET @sql = @sql + ' uniqueidentifier';
   end

		EXECUTE sp_executesql @sql;

		-- If the user has chosen to populate existing records for this entity with the new tickbox attributes value
		IF @fieldtype = 5 AND @populateExistingWithDefault = 1
		BEGIN
			SET @sql = '';

			IF @defaultvalue = 'Yes'
			BEGIN
				SET @sql = 'UPDATE [custom_' + @masterTableName + '] SET [att' + cast(@attributeid as nvarchar(10)) + '] = 1 WHERE [att' + cast(@attributeid as nvarchar(10)) + '] IS NULL;';
			END
			ELSE IF @defaultvalue = 'No'
			BEGIN
				SET @sql = 'UPDATE [custom_' + @masterTableName + '] SET [att' + cast(@attributeid as nvarchar(10)) + '] = 0 WHERE [att' + cast(@attributeid as nvarchar(10)) + '] IS NULL;';
			END

			EXECUTE sp_executesql @sql;
		END


		declare @viewgroupid uniqueidentifier
		declare @level int
		set @viewgroupid = (select viewgroupid from fields where field = 'att' + cast(@attributeid as nvarchar(10)))
		set @level = (select [level] from viewgroups where viewgroupid = @viewgroupid)

		EXECUTE saveCustomAttributeReportConfig @attributeid, null, @level, @relationshipType;

		EXECUTE dbo.refreshSystemViews @entityid;

		if @fieldtype not in (9,11,19,21)
		begin
			-- insert new field entry into customFields for any systemview that is derived from the current entity
			declare @fieldTypeChar nvarchar(2) = (select dbo.getFieldType(@fieldtype, @format));
			declare @sysviewfieldname nvarchar(20) = 'att' + cast(@attributeid as nvarchar);
			declare @sysviewtableid uniqueidentifier;
			declare @valuelist bit = (CASE @fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END);
   declare @isForeignKey bit =  CAST(0 AS BIT);
			if @maxlength is null
			begin
				if @fieldtype = 1 or @fieldtype = 16
				begin
					set @maxlength = 4000;
				end
				else
				begin
					set @maxlength = 50;
				end
			end

			declare lp cursor for
			select tableid from [customEntities] where systemview=1 and systemview_derivedentityid = @entityid
			open lp
			fetch next from lp into @sysviewtableid
			while @@FETCH_STATUS = 0
			begin
				if not exists (select fieldid from customFields where tableid = @sysviewtableid and field = @sysviewfieldname)
				begin
     insert into customFields (field, fieldtype, description, comment, width, allowimport, tableid, viewgroupid, valuelist, IsForeignKey)
     values (@sysviewfieldname, @fieldTypeChar, @displayname, @description, @maxlength, 1, @sysviewtableid, @sysviewtableid, @valuelist, @isForeignKey);
				end

				fetch next from lp into @sysviewtableid
			end
			close lp;
			deallocate lp;
		end
	END
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM [customEntityAttributes] WHERE entityid = @entityid AND display_name = @displayname AND attributeid <> @attributeid);
	IF @count > 0
		RETURN -1;

	DECLARE @oldBuiltIn BIT;
	SELECT @oldBuiltIn = BuiltIn FROM [customEntityAttributes] WHERE attributeid = @attributeid;
	
	-- do not allow the built-in value to go back to false
	IF @oldBuiltIn = 1 AND @builtIn = 0
		SET @builtIn = 1

	update [customEntityAttributes] set display_name = @displayname, [description] = @description, tooltip = @tooltip, mandatory = @mandatory, DisplayInMobile = @displayInMobile, modifiedby = @userid, modifiedon = @date, maxlength = @maxlength, format = @format, defaultvalue = @defaultvalue, workflowid = @workflowid, is_audit_identity = @isauditidentity, advicePanelText = @commentText, related_entity = @related_entityid, is_unique = @isunique, TriggerAttributeId = @triggerAttributeId, TriggerJoinViaId = @triggerJoinViaId, TriggerDisplayFieldId = @triggerDisplayFieldId, CacheExpiry = GETUTCDATE(), BoolAttribute = @boolAttribute, [Encrypted] = @encrypted where attributeid = @attributeid;
		
	update customFields set [description] = @displayname, comment = @description, mandatory = @mandatory, amendedon = getdate(), [length] = ISNULL(@maxlength, 0), width = ISNULL(@maxlength, 50) where field = 'att' + cast(@attributeid as nvarchar);
end

return @attributeid