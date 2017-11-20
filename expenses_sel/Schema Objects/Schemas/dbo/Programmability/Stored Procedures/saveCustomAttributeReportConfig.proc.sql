CREATE procedure [dbo].[saveCustomAttributeReportConfig]
@attributeid int,
@parent_viewgroup uniqueidentifier,
@level int,
@relationshiptype tinyint
as
begin
declare @count int

if @attributeid > 0
begin
	-- dynamically create custom_jointables entry
	declare @jointableid int
	declare @joinbreakdownid int
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
			declare jt_loop cursor for
			SELECT     dbo.custom_entity_attributes.attributeid + 10000 AS jointableid, dbo.custom_entity_attributes.aliasTableID AS tableid, 
								  dbo.custom_entities.tableid AS basetable, dbo.custom_entities.plural_name + ' to ' + dbo.tables.tablename AS description, 
								  dbo.custom_entities.modifiedon AS amendedon
			FROM         dbo.custom_entities INNER JOIN
								  dbo.custom_entity_attributes ON dbo.custom_entity_attributes.entityid = dbo.custom_entities.entityid INNER JOIN
								  dbo.tables ON dbo.tables.tableid = dbo.custom_entity_attributes.relatedtable
			WHERE     (dbo.custom_entity_attributes.fieldtype = 9) and (dbo.custom_entity_attributes.attributeid = @attributeid)

			open jt_loop
			fetch next from jt_loop into @jointableid, @tableid, @basetable, @desc, @amendedon
			while @@fetch_status = 0
			begin
				set @count = (select count(jointableid) from custom_jointables where basetableid = @basetable and tableid = @tableid)

				if @count = 0
				begin
					insert into custom_jointables (jointableid, tableid, basetableid, description, amendedon)
					values (@jointableid, @tableid, @basetable, @desc, @amendedon)
				end
				fetch next from jt_loop into @jointableid, @tableid, @basetable, @desc, @amendedon
			end
			close jt_loop
			deallocate jt_loop

			-- dynamically create custom_joinbreakdown entry
			declare jb_loop cursor for
			SELECT     dbo.custom_entity_attributes.attributeid + 10000 AS joinbreakdownid, dbo.custom_entity_attributes.attributeid + 10000 AS jointableid, CAST(1 AS tinyint)
								   AS [order], dbo.custom_entity_attributes.aliasTableID AS tableid, dbo.custom_entities.tableid AS sourcetable, dbo.tables.primarykey AS joinkey, 
								  dbo.custom_entities.modifiedon AS amendedon, dbo.custom_entity_attributes.fieldid AS destinationkey
			FROM         dbo.custom_entities INNER JOIN
								  dbo.custom_entity_attributes ON dbo.custom_entity_attributes.entityid = dbo.custom_entities.entityid INNER JOIN
								  dbo.tables ON dbo.tables.tableid = dbo.custom_entity_attributes.relatedtable
			WHERE     (dbo.custom_entity_attributes.relationshiptype = 1) OR
								  (dbo.custom_entity_attributes.relationshiptype IS NULL)
			open jb_loop
			fetch next from jb_loop into @joinbreakdownid, @jointableid, @order, @tableid, @sourcetable, @joinkey, @amendedon, @destinationkey
			while @@FETCH_STATUS = 0
			begin
				set @count = (select count(joinbreakdownid) from custom_joinbreakdown where joinbreakdownid = @joinbreakdownid)

				if @count = 0
				begin
					insert into custom_joinbreakdown (joinbreakdownid, jointableid, [order], tableid, sourcetable, joinkey, destinationkey, amendedon)
					values (@joinbreakdownid, @jointableid, @order, @tableid, @sourcetable, @joinkey, @destinationkey, @amendedon)
				end

				fetch next from jb_loop into @joinbreakdownid, @jointableid, @order, @tableid, @sourcetable, @joinkey, @amendedon, @destinationkey
			end
			close jb_loop
		END
	ELSE
		BEGIN
			declare jt_loop cursor for
			SELECT     dbo.custom_entity_attributes.attributeid + 10000 AS jointableid, dbo.custom_entity_attributes.relatedtable AS tableid, 
								  dbo.custom_entities.tableid AS basetable, dbo.custom_entities.plural_name + ' to ' + dbo.tables.tablename AS description, 
								  dbo.custom_entities.modifiedon AS amendedon
			FROM         dbo.custom_entities INNER JOIN
								  dbo.custom_entity_attributes ON dbo.custom_entity_attributes.entityid = dbo.custom_entities.entityid INNER JOIN
								  dbo.tables ON dbo.tables.tableid = dbo.custom_entity_attributes.relatedtable
			WHERE     (dbo.custom_entity_attributes.fieldtype = 9) and (dbo.custom_entity_attributes.attributeid = @attributeid)

			open jt_loop
			fetch next from jt_loop into @jointableid, @tableid, @basetable, @desc, @amendedon
			while @@fetch_status = 0
			begin
				set @count = (select count(jointableid) from custom_jointables where basetableid = @basetable and tableid = @tableid)

				if @count = 0
				begin
					insert into custom_jointables (jointableid, tableid, basetableid, description, amendedon)
					values (@jointableid, @tableid, @basetable, @desc, @amendedon)
				end
				fetch next from jt_loop into @jointableid, @tableid, @basetable, @desc, @amendedon
			end
			close jt_loop
			deallocate jt_loop

			-- dynamically create custom_joinbreakdown entry
			declare jb_loop cursor for
			
			SELECT     custom_entity_attributes_1.attributeid + 10000 AS joinbreakdownid, custom_entity_attributes_1.attributeid + 10000 AS jointableid, CAST(1 AS tinyint) 
								  AS [order], custom_entity_attributes_1.relatedtable AS tableid, custom_entities_1.tableid AS sourcetable, tables_1.primarykey AS joinkey, 
								  custom_entities_1.modifiedon AS amendedon,
									  (SELECT     fieldid
										FROM          dbo.custom_entity_attributes
										WHERE      (entityid = custom_entities_1.entityid) AND (is_key_field = 1)) AS destinationkey
			FROM         dbo.custom_entities AS custom_entities_1 INNER JOIN
								  dbo.custom_entity_attributes AS custom_entity_attributes_1 ON custom_entity_attributes_1.entityid = custom_entities_1.entityid INNER JOIN
								  dbo.tables AS tables_1 ON tables_1.tableid = custom_entity_attributes_1.relatedtable
			WHERE     (custom_entity_attributes_1.fieldtype = 9) AND (custom_entity_attributes_1.relationshiptype = 2) AND (custom_entity_attributes_1.attributeid = @attributeid)

			open jb_loop
			fetch next from jb_loop into @joinbreakdownid, @jointableid, @order, @tableid, @sourcetable, @joinkey, @amendedon, @destinationkey
			while @@FETCH_STATUS = 0
			begin
				set @count = (select count(joinbreakdownid) from custom_joinbreakdown where joinbreakdownid = @joinbreakdownid)

				if @count = 0
				begin
					insert into custom_joinbreakdown (joinbreakdownid, jointableid, [order], tableid, sourcetable, joinkey, destinationkey, amendedon)
					values (@joinbreakdownid, @jointableid, @order, @tableid, @sourcetable, @joinkey, @destinationkey, @amendedon)
				end

				fetch next from jb_loop into @joinbreakdownid, @jointableid, @order, @tableid, @sourcetable, @joinkey, @amendedon, @destinationkey
			end
			close jb_loop
			deallocate jb_loop
		END
end

-- dynamically create custom_viewgroups entry
declare @entity_name nvarchar(250)
declare @viewgroupid uniqueidentifier

declare vg_loop cursor for
SELECT     entity_name, tableid AS viewgroupid
FROM         dbo.custom_entities

open vg_loop
fetch next from vg_loop into @entity_name, @viewgroupid
while @@FETCH_STATUS = 0
begin
	set @count = (select count(viewgroupid) from custom_viewgroups where viewgroupid = @viewgroupid)

	if @count = 0
	begin
		if @parent_viewgroup is not null
		begin
			insert into custom_viewgroups (entity_name, [level], viewgroupid,parentid)
			values (@entity_name, @level, @viewgroupid, @parent_viewgroup)
		end
		else
		begin
			insert into custom_viewgroups (entity_name, [level], viewgroupid)
			values (@entity_name, @level, @viewgroupid)
		end
	end
	fetch next from vg_loop into @entity_name, @viewgroupid
end
close vg_loop
deallocate vg_loop
end
