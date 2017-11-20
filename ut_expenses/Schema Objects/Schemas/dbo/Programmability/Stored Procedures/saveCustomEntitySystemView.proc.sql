CREATE PROCEDURE [dbo].[saveCustomEntitySystemView]
@entityid int,
@parententityid int,
@systemViewEntityName nvarchar(250),
@relativeattributeid int,
@entityname nvarchar(250),
@pluralname NVARCHAR(250),
@description nvarchar(4000),
@audienceViewType tinyint,
@allowdocmergeaccess bit,
@date DateTime,
@userid int,
@enablePopupWindow BIT
AS

	DECLARE @count INT;
	DECLARE @ParentEntityName NVARCHAR(250) = '';
	DECLARE @parentMasterTableName NVARCHAR(250) = '';

	SELECT @ParentEntityName = entity_name, @parentMasterTableName = masterTableName FROM [customEntities] WHERE entityid = @parententityid;
	
	insert into [customEntities] (entity_name, plural_name, [description], createdby, createdon, enableAttachments, AudienceViewType, allowdocmergeaccess, systemview, systemview_derivedentityid, systemview_entityid, masterTableName, CacheExpiry, enablePopupWindow) values (@systemViewEntityName, @pluralname, @description, @userid, @date, @audienceViewType, @audienceViewType, @allowdocmergeaccess, cast(1 as bit), @parententityid, @entityid, @systemViewEntityName, GETUTCDATE(), @enablePopupWindow);
	set @entityid = scope_identity();
	
	DECLARE @masterTableName NVARCHAR(250);
	declare @parentField nvarchar(10);
	declare @joinField nvarchar(250);
	declare @sysviewEntityId int;
	DECLARE @viewMasterTableName NVARCHAR(250) = '';
	
	select @sysviewEntityId = entityid, @masterTableName = parent_mastertablename, @parentField = parent_fieldname, @joinField = related_fieldname from [customEntityRelationships] where attributeid = @relativeattributeid;

	SET @viewMasterTableName = (SELECT @masterTableName + '_to_' + @parentMasterTableName);
	UPDATE [customEntities] SET systemview_entityid = @sysviewEntityId, masterTableName = @viewMasterTableName, CacheExpiry = GETUTCDATE() WHERE entityid = @entityid;
	
	
	DECLARE @sql NVARCHAR(4000);
	SET @sql =  'CREATE VIEW [dbo].[custom_' + @masterTableName + '_to_' + @parentMasterTableName + '] AS ' +
	'SELECT [custom_' + @parentMasterTableName + '].* FROM dbo.[custom_' + @parentMasterTableName + ']' +
	' INNER JOIN dbo.[custom_' + @masterTableName + '] ON dbo.[custom_' + @masterTableName + '].' + @parentField + ' = dbo.[custom_' + @parentMasterTableName + '].' + @joinField;

	EXECUTE sp_executesql @sql;

	-- create field entries in the custom_fields
	declare @viewtableid uniqueidentifier;
	set @viewtableid = (select tableid from [customEntities] where entityid = @entityid);

	declare @f nvarchar(250);
	declare @ft nvarchar(10);
	declare @d nvarchar(250);
	declare @id bit;
	declare @gl bit;
	declare @w int;
	declare @ifk bit;
	declare @rt uniqueidentifier;
	declare @vlist bit;

	declare cf_loop cursor for
	select field, fieldtype, [description], idfield, genlist, width, isforeignkey, relatedtable, valuelist from fields where fieldid in (select fieldid from [customEntityAttributes] where entityid = @parententityid)
	open cf_loop
	fetch next from cf_loop into @f, @ft, @d, @id, @gl, @w, @ifk, @rt, @vlist
	while @@FETCH_STATUS = 0
	begin
		if not exists (select [fieldid] from [customFields] where field = @f and tableid = @viewtableid)
		begin
			insert into [customFields] (field, fieldtype, [description], comment, normalview, idfield, viewgroupid_old, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, useforlookup, workflowupdate, workflowsearch, [length], tableid, viewgroupid, relabel, isforeignkey, relatedtable)
			values (@f, @ft, @d, @d, 0, @id, 1, @gl, @w, 0, 0, @vlist, 1, 0, getutcdate(), 0, 0, 0, 0, @viewtableid, @viewtableid, 0, @ifk, @rt)
		end
		fetch next from cf_loop into @f, @ft, @d, @id, @gl, @w, @ifk, @rt, @vlist
	end
	close cf_loop
	deallocate cf_loop

	declare @vg_level int
	set @vg_level = isnull((select [level] from [customViewGroups] where viewgroupid = (select tableid from customEntities where masterTableName = @masterTableName)),0) + 1
	declare @vg_parentid uniqueidentifier
	set @vg_parentid = (select viewgroupid from [customViewGroups] where viewgroupid = (select tableid from customEntities where masterTableName = @masterTableName))
	EXECUTE saveCustomAttributeReportConfig @relativeattributeid, @vg_parentid, @vg_level, 2

	-- Update relatedtable in custom entity view definition
	-- update customEntityAttributes set relatedtable = @viewtableid where attributeid = @relativeattributeid
		
return @entityid
