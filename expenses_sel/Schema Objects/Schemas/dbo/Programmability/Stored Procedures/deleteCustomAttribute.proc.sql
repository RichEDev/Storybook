CREATE PROCEDURE [dbo].[deleteCustomAttribute] (@attributeid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	DECLARE @pluralname NVARCHAR(250);
	DECLARE @entityid INT;
	DECLARE @attributename NVARCHAR(100);
	DECLARE @fieldtype TINYINT
	declare @relationshiptype tinyint
	DECLARE @relatedtableid UNIQUEIDENTIFIER;
	declare @fieldid uniqueidentifier
	DECLARE @relatedtable NVARCHAR(250);
	SELECT @entityid = entityid, @attributename = attribute_name, @fieldtype = fieldtype, @fieldid = fieldid, @relationshiptype = relationshiptype, @relatedtableid = relatedtable FROM [custom_entity_attributes] WHERE attributeid = @attributeid;
	SELECT @pluralname = plural_name FROM custom_entities WHERE entityid = @entityid;
	select @relatedtable = tablename from tables where tableid = @relatedtableid;
	
	DECLARE @sql NVARCHAR(4000)
	IF @fieldtype = 9
	BEGIN
		declare @constraint_name nvarchar(250)
		set @constraint_name = 'FK_custom_' + replace(@pluralname,' ','_') + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar(10))
		
		declare @table_name nvarchar(250)
		set @table_name = 'custom_' + replace(@pluralname,' ','_')
		
		IF EXISTS (SELECT CONSTRAINT_NAME
           FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
			WHERE TABLE_NAME = @table_name AND CONSTRAINT_NAME = @constraint_name)
		BEGIN
			SET @sql = 'ALTER TABLE [' + @table_name + '] DROP [' + @constraint_name + ']';
			EXECUTE sp_executesql @sql
		END

		if @relationshiptype = 2
		begin
			declare @sysviewname nvarchar(300)
			
			set @sysviewname = (select tablename from tables where tableid = @relatedtableid);
			delete from custom_joinbreakdown where jointableid in (select jointableid from custom_jointables where tableid = @relatedtableid);
			delete from custom_jointables where tableid = @relatedtableid;
			delete from custom_viewgroups where viewgroupid = (select distinct viewgroupid from custom_fields where tableid = @relatedtableid);
			delete from custom_fields where tableid = @relatedtableid;
			delete from custom_entities where tableid = @relatedtableid;

			IF OBJECT_ID (@sysviewname, 'view') IS NOT NULL
			begin
				set @sql = 'drop view [' + @sysviewname + ']';
				exec sp_executesql @sql;
			end

			SET @sql = 'alter table [' + @relatedtable + '] drop column [att' + cast(@attributeid as nvarchar(10)) + ']';
			EXECUTE sp_executesql @sql;
			
			-- if a 1:n relationship field has been deleted, this can invalidate the systemviews - so refresh each (can't tell which will be affected!)
			declare @viewName nvarchar(250);
			declare @systemViewName nvarchar(250);

			declare loop_cursor cursor for
			select plural_name from custom_entities where systemview=1
			open loop_cursor
			fetch next from loop_cursor into @systemViewName
			while @@FETCH_STATUS = 0
			begin
				set @viewName = 'custom_' + replace(@systemViewName, ' ', '_');
				
				set @sql = 'exec sp_refreshview ' + char(39) + @viewName + char(39);
				
				if exists (select * from sys.views where type = 'V' and name = @viewName)
				begin
					exec sp_executesql @sql
				end
					
				fetch next from loop_cursor into @systemViewName
			end
			close loop_cursor
			deallocate loop_cursor
		end
		ELSE
		BEGIN
			SET @sql = 'alter table [' + @table_name + '] drop column [att' + cast(@attributeid as nvarchar(10)) + ']';
			EXECUTE sp_executesql @sql;
		END
	END
	ELSE
	BEGIN
		SET @sql = 'alter table [' + @table_name + '] drop column [att' + cast(@attributeid as nvarchar(10)) + ']';
		EXECUTE sp_executesql @sql;
	END
	
	delete from custom_entity_attribute_list_items where attributeid = @attributeid;

	delete from custom_entity_form_fields where attributeid = @attributeid;
	delete from custom_entity_view_fields where fieldid = (select fieldid from custom_entity_attributes where attributeid = @attributeid);
	DELETE FROM [custom_entity_attributes] WHERE attributeid = @attributeid;
END
