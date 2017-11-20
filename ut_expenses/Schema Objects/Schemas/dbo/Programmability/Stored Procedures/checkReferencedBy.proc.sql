create procedure dbo.checkReferencedBy
(
	@currentTableID uniqueidentifier,
	@currentRecordID int
)
as
begin
	declare @retData IntPK_String;

	declare @entityName nvarchar(250);
	declare @entityID int;
	declare @attributeID int;
	declare @displayName nvarchar(250);
	declare @masterTableName nvarchar(260);
	declare @sql nvarchar(max);
	declare @sqlParams nvarchar(max) = '';
	declare @returnCount int;
	declare @fieldName nvarchar(20) = '';

	declare lp cursor for
	select customEntities.entityid, attributeid, display_name, customEntities.entity_name, customEntities.masterTableName from customEntityAttributes 
	inner join customEntities on customEntityAttributes.entityid = customEntities.entityid
	where fieldtype = 9 and relatedtable = @currentTableID and customEntities.systemview = 0
	order by customEntities.entityid
	open lp
	fetch next from lp into @entityID, @attributeID, @displayName, @entityName, @masterTableName
	while @@FETCH_STATUS = 0
	begin
		if @displayName = 'Created By' or @displayName = 'Modified By'
		begin
			set @fieldName = REPLACE(@displayName,' ', '');
		end
		else
		begin
			set @fieldName = 'att' + CAST(@attributeID as nvarchar);
		end
		
		if exists (select table_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'custom_' + @masterTableName AND COLUMN_NAME = @fieldName)
		begin
			-- execute the check sql
			set @sql = 'SELECT @recCount = COUNT(*) FROM [custom_' + @masterTableName + '] WHERE [' + @fieldName + '] = @curRecID';
			--print @sql;
			set @sqlParams = '@curRecID int, @recCount int OUTPUT';
			exec sp_executesql @sql, @sqlParams, @currentRecordID, @recCount = @returnCount output;
			--print '@returnCount = ' + cast(@returnCount as nvarchar);
			
			if @returnCount > 0
			begin
				-- must be a reference to this record, so add record to the return data
				insert into @retData values (@attributeID, @entityName + ' (' + @displayName + ')');
			end
		end
		
		fetch next from lp into @entityID, @attributeID, @displayName, @entityName, @masterTableName
	end
	close lp;
	deallocate lp;

	-- check user defined for dependencies
	declare @udfID int;
	declare @udfName nvarchar(50);
	declare @udfTableID uniqueidentifier;
	declare @udfParent nvarchar(100);

	declare udflp cursor for
	select userdefineid, attribute_name, display_name, tables.tablename, userdefined.tableid from userdefined
	inner join tables on tables.tableid = userdefined.tableid 
	where fieldtype = 9 and relatedtable = @currentTableID
	order by userdefined.tableid
	open udflp
	fetch next from udflp into @udfID, @udfName, @displayName, @masterTableName, @udfTableID
	while @@FETCH_STATUS = 0
	begin
		if @displayName = 'Created By' or @displayName = 'Modified By'
		begin
			set @fieldName = REPLACE(@displayName,' ', '');
		end
		else
		begin
			set @fieldName = 'udf' + CAST(@udfID as nvarchar);
		end		
		
		if exists (select table_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @masterTableName AND COLUMN_NAME = @fieldName)
		begin
			-- execute the check sql
			set @sql = 'SELECT @recCount = COUNT(*) FROM [' + @masterTableName + '] WHERE [' + @fieldName + '] = @curRecID';
			--print @sql;
			set @sqlParams = '@curRecID int, @recCount int OUTPUT';
			exec sp_executesql @sql, @sqlParams, @currentRecordID, @recCount = @returnCount output;
			--print '@returnCount = ' + cast(@returnCount as nvarchar);
			
			if @returnCount > 0
			begin
				-- must be a reference to this record, so add record to the return data
				select @udfParent = [description] from tables where userdefined_table = @udfTableID;
				
				insert into @retData values (@udfID, @udfParent + ' (' + @displayName + ')');
			end
		end
			
		fetch next from udflp into @udfID, @udfName, @displayName, @masterTableName, @udfTableID
	end
	close udflp;
	deallocate udflp;

	-- temporary return code to fit with current implementation. Enhancements will return the @retData contents
	if exists (select top 1 c1 from @retData)
	begin
		return -10;
	end
	else
	begin
		return 0;
	end
	--select * from @retData;
end
GO

