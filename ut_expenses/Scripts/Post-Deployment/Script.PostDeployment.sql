/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
UPDATE customEntities SET masterTableName = plural_name WHERE masterTableName IS NULL;
GO

UPDATE [$(targetMetabase)].dbo.tables_base SET tablename = 'customTables' WHERE tablename = 'custom_tables';
UPDATE [$(targetMetabase)].dbo.tables_base SET tablename = 'customFields' WHERE tablename = 'custom_fields';
UPDATE [$(targetMetabase)].dbo.tables_base SET tablename = 'customEntities' WHERE tablename = 'custom_entities';
UPDATE [$(targetMetabase)].dbo.tables_base SET tablename = 'customEntityAttributes' WHERE tablename = 'custom_entity_attributes';
UPDATE [$(targetMetabase)].dbo.tables_base SET tablename = 'customEntityViews' WHERE tablename = 'custom_entity_views';
UPDATE [$(targetMetabase)].dbo.tables_base SET tablename = 'customEntityForms' WHERE tablename = 'custom_entity_forms';
GO

DECLARE @customTableID UNIQUEIDENTIFIER = (SELECT tableid FROM [$(targetMetabase)].dbo.tables_base WHERE tablename = 'customTables');
DECLARE @keyfield UNIQUEIDENTIFIER;
DECLARE @primarykeyfield UNIQUEIDENTIFIER;
SELECT @keyfield = fieldid FROM [$(targetMetabase)].dbo.fields_base WHERE tableid = @customTableID AND field = 'tablename';
SELECT @primarykeyfield = fieldid FROM [$(targetMetabase)].dbo.fields_base WHERE tableid = @customTableID AND field = 'tableid';

UPDATE [$(targetMetabase)].dbo.tables_base SET keyfield = @keyfield, primarykey = @primarykeyfield WHERE tableid = @customTableID;
GO

declare @desc nvarchar(4000);
declare @attId int;
declare @entityId int;
declare @sql nvarchar(4000);
declare @exesql nvarchar(4000);

set @sql = 'if exists (select column_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = ''*TABLE*'' AND COLUMN_NAME = ''*COLUMN*'') begin alter table [*TABLE*] drop column [*COLUMN*]; end';

declare attCursor cursor for
select attributeid, entityid, [description] from customEntityAttributes where fieldtype = 14
open attCursor
fetch next from attCursor into @attId, @entityId, @desc
while @@FETCH_STATUS = 0
begin
	update customEntityAttributes set advicePanelText = @desc, fieldtype = 19, modifiedon = GETUTCDATE() where attributeid = @attId;
	
	-- need to drop old label field as comment type doesn't have a physical field
	set @exesql = REPLACE(@sql, '*TABLE*', (select 'custom_' + masterTableName from customEntities where entityid = @entityId));
	set @exesql = REPLACE(@exesql, '*COLUMN*', 'att' + CAST(@attId as nvarchar));
	
	print @exesql;
	exec sp_executesql @exesql;
	
	fetch next from attCursor into @attId, @entityId, @desc
end
close attCursor
deallocate attCursor;
GO

update userdefined set format = 8 where fieldtype = 4 and format is null;

update customEntityAttributes set format = 8 where fieldtype = 4 and format is null;
GO

-- update any occurrences of relationship text boxes in user defined fields or customEntityAttributes
declare @relTableID uniqueidentifier;
declare @udfId int;
declare @employeeMatchFields GuidPK;
insert into @employeeMatchFields values 
('B5DD3E91-DA63-4E9B-B933-FEC64831D920'),
('1C45B860-DDAA-47DA-9EEC-981F59CCE795'),
('0F951C3E-29D1-49F0-AC13-4CFCABF21FDA');

declare @employeeDisplayField uniqueidentifier = 'B5DD3E91-DA63-4E9B-B933-FEC64831D920';

declare @companiesMatchFields GuidPK;
insert into @companiesMatchFields values 
('F31F204A-E714-4F0B-9B3D-7170DB58A30B'),
('258118D7-9B57-4C16-A99A-B7D07EDC9A54'),
('B84C65C7-7140-4930-B90E-CA5AD9374017'),
('9086176A-DFB9-4151-94DC-EE345F0A594B');
declare @companiesDisplayField uniqueidentifier = 'F31F204A-E714-4F0B-9B3D-7170DB58A30B';

declare lp cursor for
select userdefineid, relatedTable from userdefined where fieldtype = 12
open lp
fetch next from lp into @udfId, @relTableID
while @@FETCH_STATUS = 0
begin
	if @relTableID = 'D0335558-1EF5-449B-87CD-3E6BBF126205' -- companies
	begin
		update userdefined set fieldtype = 9, maxRows = 15, displayField = @companiesDisplayField where userdefineid = @udfId;
		
		if not exists (select 1 from userdefinedMatchFields where userdefineid = @udfId)
		begin
			insert into userdefinedMatchFields select @udfId, ID from @companiesMatchFields
		end
	end
		
	if @relTableID = '618DB425-F430-4660-9525-EBAB444ED754' -- employees
	begin
		update userdefined set fieldtype = 9, maxRows = 15, displayField = @employeeDisplayField where userdefineid = @udfId;
		
		if not exists (select 1 from userdefinedMatchFields where userdefineid = @udfId)
		begin	
			insert into userdefinedMatchFields select @udfId, ID from @employeeMatchFields
		end
	end
	
	fetch next from lp into @udfId, @relTableID
end
close lp
deallocate lp

declare @attId int;

declare lp cursor for
select attributeid, relatedTable from customEntityAttributes where fieldtype = 12
open lp
fetch next from lp into @attId, @relTableID
while @@FETCH_STATUS = 0
begin
	if @relTableID = 'D0335558-1EF5-449B-87CD-3E6BBF126205' -- companies
	begin
		update customEntityAttributes set fieldtype=9, relationshiptype=1, maxRows = 15, relationshipDisplayField = @companiesDisplayField where attributeid = @attId;
		
		if not exists (select 1 from customEntityAttributeMatchFields where attributeId = @attId)
		begin
			insert into customEntityAttributeMatchFields select @attId, ID from @companiesMatchFields
		end
	end
	
	if @relTableID = '618DB425-F430-4660-9525-EBAB444ED754' -- employees
	begin
		update customEntityAttributes set fieldtype=9, relationshiptype=1, maxRows = 15, relationshipDisplayField = @employeeDisplayField where attributeid = @attId;
		
		if not exists (select 1 from customEntityAttributeMatchFields where attributeId = @attId)
		begin
			insert into customEntityAttributeMatchFields select @attId, ID from @employeeMatchFields
		end
	end
	
	fetch next from lp into @attId, @relTableID
end
close lp
deallocate lp

declare @ceaTableID uniqueidentifier = (select tableid from [$(targetMetabase)].dbo.tables_base where tablename = 'customEntityAttributes');
if not exists (select fieldid from [$(targetMetabase)].dbo.fields_base where tableid = @ceaTableID and field = 'system_attribute')
begin
	insert into [$(targetMetabase)].dbo.fields_base (fieldid, field, fieldtype, description, amendedon, tableid)
	values ('1B83975C-F9AD-46F5-8380-DE3EB05AA53F', 'system_attribute', 'X', 'Identified if field is generated by the system not a user', GETDATE(), @ceaTableID)
end


update customEntityAttributes set system_attribute = 0 where system_attribute is null

update customEntityAttributes set system_attribute = 1 where lower(display_name) in ('id', 'created by', 'created on', 'modified on', 'modified by', 'greenlight currency')
