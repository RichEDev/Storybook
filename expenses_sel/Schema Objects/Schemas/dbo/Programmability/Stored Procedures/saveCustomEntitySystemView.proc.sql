CREATE PROCEDURE [dbo].[saveCustomEntitySystemView]
@entityid int,
@parententityid int,
@viewname nvarchar(250),
@donorpluralname nvarchar(250),
@relativeattributeid int,
@entityname nvarchar(250),
@pluralname NVARCHAR(250),
@description nvarchar(4000),
@enableattachments BIT,
@enableAudiences bit,
@allowdocmergeaccess bit,
@date DateTime,
@userid int
AS

DECLARE @count INT;
DECLARE @viewentityid int;
DECLARE @ParentEntityName nvarchar(250);

	SET @ParentEntityName = (SELECT entity_name from custom_entities where entityid = @parententityid);
	
	insert into custom_entities (entity_name, plural_name, [description], createdby, createdon, enableAttachments, enableAudiences, allowdocmergeaccess, systemview, systemview_derivedentityid, systemview_entityid) values (@viewname, @pluralname, @description, @userid, @date, @enableattachments, @enableAudiences, @allowdocmergeaccess, cast(1 as bit), @parententityid, @entityid);
	set @viewentityid = scope_identity();
	
	declare @parentTable nvarchar(250);
	declare @parentField nvarchar(10);
	declare @joinField nvarchar(250);
	declare @sysviewEntityId int;
	
	select @sysviewEntityId = entityid, @parentTable = parent_pluralname, @parentField = parent_fieldname, @joinField = related_fieldname from custom_entity_relationships where attributeid = @relativeattributeid;

	update custom_entities set systemview_entityid = @sysviewEntityId where entityid = @viewentityid;
	
	
	DECLARE @sql NVARCHAR(4000)
	SET @sql =  'CREATE VIEW [dbo].[custom_' + replace(@viewname,' ', '_') + '] AS ' +
	'SELECT [custom_' + replace(@donorpluralname,' ','_') + '].* FROM dbo.[custom_' + replace(@donorpluralname,' ','_') + ']' +
	' INNER JOIN dbo.[custom_' +  + replace(@parentTable,' ', '_') + '] ON dbo.[custom_' + replace(@parentTable,' ','_') + '].' + @parentField + ' = dbo.[custom_' + replace(@donorpluralname,' ','_') + '].' + @joinField;

	EXECUTE sp_executesql @sql;

	-- create field entries in the custom_fields
	declare @donortableid uniqueidentifier;
	set @donortableid = (select tableid from tables where tablename = @donorpluralname);
	declare @viewtableid uniqueidentifier;
	set @viewtableid = (select tableid from custom_entities where entityid = @viewentityid);

	declare @f nvarchar(250);
	declare @ft nvarchar(10);
	declare @d nvarchar(250);
	declare @id bit;
	declare @gl bit;
	declare @w int;
	declare cf_loop cursor for
	select field, fieldtype, [description], idfield, genlist, width from fields where fieldid in (select fieldid from custom_entity_attributes where entityid = @parententityid)
	open cf_loop
	fetch next from cf_loop into @f, @ft, @d, @id, @gl, @w
	while @@FETCH_STATUS = 0
	begin
		if not exists (select [fieldid] from custom_fields where field = @f and tableid = @viewtableid)
		begin
			insert into custom_fields (field, fieldtype, [description], comment, normalview, idfield, viewgroupid_old, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, useforlookup, workflowupdate, workflowsearch, [length], tableid, viewgroupid, relabel)
			values (@f, @ft, @d, @d, 0, @id, 1, @gl, @w, 0, 0, 0, 1, 0, getutcdate(), 0, 0, 0, 0, @viewtableid, @viewtableid, 0)
		end
		fetch next from cf_loop into @f, @ft, @d, @id, @gl, @w
	end
	close cf_loop
	deallocate cf_loop

	declare @vg_level int
	set @vg_level = (select [level] from custom_viewgroups where entity_name = @ParentEntityName) + 1
	declare @vg_parentid uniqueidentifier
	set @vg_parentid = (select viewgroupid from custom_viewgroups where entity_name = @ParentEntityName)
	EXECUTE saveCustomAttributeReportConfig @relativeattributeid, @vg_parentid, @vg_level, 2

	-- Update relatedtable in custom entity view definition
	-- update custom_entity_attributes set relatedtable = @viewtableid where attributeid = @relativeattributeid
		
return @entityid
