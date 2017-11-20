CREATE PROCEDURE [dbo].[saveCustomAttributeReportConfig]
@attributeid int,
@parent_viewgroup uniqueidentifier,
@level int,
@relationshiptype tinyint
as
begin
 declare @count int = 0;

 if @attributeid > 0
 begin
  -- dynamically create custom_jointables entry
  declare @jointableid uniqueidentifier
  declare @joinbreakdownid uniqueidentifier
  declare @order tinyint
  declare @tableid uniqueidentifier
  declare @basetable uniqueidentifier
  declare @sourcetable uniqueidentifier
  declare @joinkey uniqueidentifier
  declare @destinationkey uniqueidentifier
  declare @desc nvarchar(250)
  declare @amendedon datetime
 
  IF @relationshiptype = 1 --Many to one
   BEGIN
    SELECT     @tableid = dbo.[customEntityAttributes].relatedtable, @basetable = dbo.[customEntities].tableid, 
      @desc = dbo.[customEntities].masterTableName + ' to ' + dbo.tables.tablename,  @amendedon = dbo.[customEntities].modifiedon
    FROM         dbo.[customEntities] INNER JOIN
           dbo.[customEntityAttributes] ON dbo.[customEntityAttributes].entityid = dbo.[customEntities].entityid INNER JOIN
           dbo.tables ON dbo.tables.tableid = dbo.[customEntityAttributes].relatedtable
    WHERE     (dbo.[customEntityAttributes].fieldtype = 9) and (dbo.[customEntityAttributes].attributeid = @attributeid)

    SET @jointableid = NEWID();
	
	IF NOT EXISTS (SELECT jointableid FROM [customJoinTables] WHERE basetableid = @basetable AND tableid = @tableid)
	BEGIN
		INSERT INTO [customJoinTables] (jointableid, tableid, basetableid, [description], amendedon)
		VALUES (@jointableid, @tableid, @basetable, @desc, @amendedon);
	END

    SELECT     @order = CAST(1 AS tinyint), @tableid = dbo.[customEntityAttributes].relatedtable, @sourcetable = dbo.[customEntities].tableid, 
         @joinkey = dbo.tables.primarykey,  @amendedon = dbo.[customEntities].modifiedon, @destinationkey = dbo.[customEntityAttributes].fieldid
    FROM       dbo.[customEntities] INNER JOIN
           dbo.[customEntityAttributes] ON dbo.[customEntityAttributes].entityid = dbo.[customEntities].entityid INNER JOIN
           dbo.tables ON dbo.tables.tableid = dbo.[customEntityAttributes].relatedtable
    WHERE     (dbo.[customEntityAttributes].relationshiptype = 1 OR
           dbo.[customEntityAttributes].relationshiptype IS NULL) AND ([customEntityAttributes].attributeid = @attributeid)

	IF NOT EXISTS (SELECT joinbreakdownid FROM [customJoinBreakdown] WHERE joinbreakdownid = @joinbreakdownid)
	BEGIN
		INSERT INTO [customJoinBreakdown] (jointableid, [order], tableid, sourcetable, joinkey, destinationkey, amendedon)
		VALUES (@jointableid, @order, @tableid, @sourcetable, @joinkey, @destinationkey, @amendedon)
	END
   END
  ELSE IF @relationshiptype = 2
   BEGIN
    SET @tableid = (select tableid from customEntities where systemview_derivedentityid = (select related_entity from customEntityAttributes where attributeid = @attributeid) and systemview_entityid = (select entityid from customEntities where tableid = @parent_viewgroup));

    SELECT     @basetable = dbo.[customEntities].tableid, @desc = dbo.[customEntities].masterTableName + ' to ' + dbo.tables.tablename, @amendedon = dbo.[customEntities].modifiedon
    FROM         dbo.[customEntities] INNER JOIN
           dbo.[customEntityAttributes] ON dbo.[customEntityAttributes].entityid = dbo.[customEntities].entityid INNER JOIN
           dbo.tables ON dbo.tables.tableid = @tableid
    WHERE     (dbo.[customEntityAttributes].fieldtype = 9) and (dbo.[customEntityAttributes].attributeid = @attributeid);

    SET @jointableid = NEWID();
	IF NOT EXISTS (SELECT jointableid FROM [customJoinTables] WHERE basetableid = @basetable AND tableid = @tableid)
	BEGIN
		INSERT INTO [customJoinTables] (jointableid, tableid, basetableid, [description], amendedon)
		VALUES (@jointableid, @tableid, @basetable, @desc, @amendedon)
	END

    SELECT     @order = CAST(1 AS tinyint), @sourcetable = custom_entities_1.tableid, 
     @joinkey = custom_entity_attributes_1.fieldid,  @amendedon = custom_entities_1.modifiedon, @destinationkey = 
            (SELECT     fieldid
           FROM          dbo.[customEntityAttributes]
           WHERE      (entityid = custom_entities_1.entityid) AND (is_key_field = 1))
    FROM         dbo.[customEntities] AS custom_entities_1 INNER JOIN
           dbo.[customEntityAttributes] AS custom_entity_attributes_1 ON custom_entity_attributes_1.entityid = custom_entities_1.entityid INNER JOIN
           dbo.tables AS tables_1 ON tables_1.tableid = @tableid
    WHERE     (custom_entity_attributes_1.fieldtype = 9) AND (custom_entity_attributes_1.attributeid = @attributeid)

	IF NOT EXISTS (SELECT joinbreakdownid FROM [customJoinBreakdown] WHERE joinbreakdownid = @joinbreakdownid)
	BEGIN
		INSERT INTO [customJoinBreakdown] (jointableid, [order], tableid, sourcetable, joinkey, destinationkey, amendedon)
		VALUES (@jointableid, @order, @tableid, @sourcetable, @joinkey, @destinationkey, @amendedon)
	END
   END
  end

 -- dynamically create custom_viewgroups entry
 declare @entity_name nvarchar(250)
 declare @viewgroupid uniqueidentifier
 declare @entityid int;
 if @relationshiptype = 2
 begin
  set @entityid = (select entityid from customEntities where systemview_derivedentityid = (select related_entity from customEntityAttributes where attributeid = @attributeid) and systemview_entityid = (select entityid from customEntities where tableid = @parent_viewgroup));
 end
 else
 begin
  set @entityid = (select entityid from customEntityAttributes where attributeid = @attributeid);
 end

 if @level is null or @level = 0
 begin
  set @level = 1;
 end

 declare vg_loop cursor for
 SELECT     entity_name, tableid AS viewgroupid
 FROM         dbo.[customEntities] where entityid = @entityid

 open vg_loop
 fetch next from vg_loop into @entity_name, @viewgroupid
 while @@FETCH_STATUS = 0
 begin
  set @count = (select count(viewgroupid) from [customViewGroups] where viewgroupid = @viewgroupid)

  if @count = 0
  begin
   if @level > 1
   begin
    insert into [customViewGroups] (entity_name, [level], viewgroupid, parentid, amendedon)
    values (@entity_name, @level, @viewgroupid, @parent_viewgroup, getdate())
   end
   else
   begin
    insert into [customViewGroups] (entity_name, [level], viewgroupid, amendedon)
    values (@entity_name, @level, @viewgroupid, getdate())
   end
  end
  fetch next from vg_loop into @entity_name, @viewgroupid
 end
 close vg_loop
 deallocate vg_loop
end