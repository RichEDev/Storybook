CREATE PROCEDURE [dbo].[saveCustomEntityAttribute]
@attributeid INT,
@entityid int,
@attributename nvarchar(250),
@displayname nvarchar(250),
@description nvarchar(4000),
@tooltip NVARCHAR(4000),
@mandatory BIT,
@fieldtype TINYINT,
@date DateTime,
@userid INT,
@maxlength INT,
@format TINYINT,
@defaultvalue NVARCHAR(50),
@precision TINYINT,
@workflowid int,
@isauditidentity bit,
@advicePaneltext nvarchar(4000),
@relationshipType tinyint
--@CUemployeeID INT,
--@CUdelegateID INT
AS
DECLARE @count INT;
if (@attributeid = 0)
BEGIN
	SET @count = (SELECT COUNT(*) FROM [custom_entity_attributes] WHERE entityid = @entityid AND attribute_name = @attributename);
	IF @count > 0
		RETURN -1;
		
	insert into custom_entity_attributes (entityid, attribute_name, display_name, [description], tooltip, mandatory, fieldtype, createdby, createdon, maxlength, format, defaultvalue, [precision], workflowid, is_audit_identity, advicePanelText) values (@entityid, @attributename, @displayname, @description, @tooltip, @mandatory, @fieldtype, @userid, @date, @maxlength, @format, @defaultvalue, @precision, @workflowid, @isauditidentity, @advicePanelText)
	set @attributeid = scope_identity()
	
	IF @fieldtype <> 11
		BEGIN
				DECLARE @pluralname NVARCHAR(250);
				DECLARE @sql NVARCHAR(4000);
				SELECT @pluralname = plural_name FROM custom_entities WHERE entityid = @entityid
				SET @pluralname = REPLACE(@pluralname,' ','_')
				SET @sql = 'alter TABLE [custom_' + @pluralname + '] add [att' + cast(@attributeid as nvarchar(10)) + ']';
				IF @fieldtype = 1 OR @fieldtype = 16
					begin
						SET @sql = @sql + ' nvarchar'
						IF @maxlength IS NULL
							BEGIN
								SET @sql = @sql + '(4000)';
							END
						else
							BEGIN
								SET @sql = @sql + '(' + CAST(@maxlength AS NVARCHAR(10)) + ')';
							END
					END
				ELSE IF @fieldtype = 2
					BEGIN
						SET @sql = @sql + ' int'
					end	
				ELSE IF @fieldtype = 7
					BEGIN
						SET @sql = @sql + ' decimal(18,' + CAST(@precision AS NVARCHAR(4)) + ')';
					END
				ELSE IF @fieldtype = 6
					BEGIN
						SET @sql = @sql + ' money'
					end	
				ELSE IF @fieldtype = 4 OR @fieldtype = 17
					BEGIN
						SET @sql = @sql + ' int'
					end	
				ELSE IF @fieldtype = 5
					BEGIN
						SET @sql = @sql + ' bit'
					end	
				ELSE IF @fieldtype = 3
					BEGIN
						SET @sql = @sql + ' DateTime'
					end	
				ELSE IF @fieldtype = 10
					BEGIN
						SET @sql = @Sql + ' nvarchar(max)'
					end
				ELSE IF @fieldtype = 14
					BEGIN
						SET @sql = @Sql + ' nvarchar(MAX)'
					END
				ELSE IF @fieldtype = 15
					BEGIN
						SET @sql = @sql + ' int'
					END
				ELSE IF @fieldtype = 18
					BEGIN
						SET @sql = @sql + ' nvarchar(max)'
					END
				EXECUTE sp_executesql @sql;
	
				declare @viewgroupid uniqueidentifier
				declare @level int
				set @viewgroupid = (select viewgroupid from fields where field = 'att' + cast(@attributeid as nvarchar(10)))
				set @level = (select [level] from viewgroups where viewgroupid = @viewgroupid)

				EXECUTE saveCustomAttributeReportConfig @attributeid, @viewgroupid, @level, @relationshipType;
		END
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM [custom_entity_attributes] WHERE entityid = @entityid AND attribute_name = @attributename AND attributeid <> @attributeid);
	IF @count > 0
		RETURN -1;
	update custom_entity_attributes set display_name = @displayname, [description] = @description, tooltip = @tooltip, mandatory = @mandatory, modifiedby = @userid, modifiedon = @date, maxlength = @maxlength, format = @format, [precision] = @precision, workflowid = @workflowid, is_audit_identity = @isauditidentity, advicePanelText = @advicePanelText where attributeid = @attributeid
	
end

return @attributeid
