CREATE PROCEDURE [dbo].[APILookupValue]
@tableId uniqueidentifier,
@fieldId uniqueidentifier,
@lookupValue nvarchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @tableName nvarchar(250) = (select tablename from tables where tableid = @tableId);
	declare @fieldName nvarchar(100);
	declare @fieldFrom int;
	declare @isListField bit;
	declare @orderFieldName nvarchar(100);
	declare @sqlResult int;
	
	select  @fieldName = field, @orderFieldName = field, @fieldFrom = fieldFrom, @isListField = valuelist from fields where fieldid = @fieldId;
	declare @idField nvarchar(100) = (select field from fields where tableid = @tableId and idfield = 1);


	declare @sql nvarchar(500) = '';
	IF NOT (@fieldName = @idField)
	BEGIN
		if @fieldName like 'dbo.%'
		begin
			set @sql = 'select ' + quotename(@idField) + ', ' + @fieldName + ' from ' + quotename(@tableName);
		end
		else
		begin
			if @isListField = 1
			begin
				if @fieldFrom = 1
				begin
					set @sql = 'select ' + quotename(@idField) + ', [customEntityAttributeListItems].[item] from ' + quotename(@tableName) + ' left join [customEntityAttributeListItems] on [customEntityAttributeListItems].[valueid] = ' + quotename(@tableName) + '.' + quotename(@fieldName) + ' where ' + @fieldName + ' = ' + @lookupValue;
					set @orderFieldName = 'item';			
				end
				else if @fieldFrom = 2
				begin
					set @sql = 'select ' + quotename(@idField) + ', [userdefined_list_items].[item] from ' + quotename(@tableName) + ' left join [userdefined_list_items] on [userdefined_list_items].[valueid] = ' + quotename(@tableName) + '.' + quotename(@fieldName) + ' where ' + @fieldName + ' = ' + @lookupValue;
					set @orderFieldName = 'item';			
				end
				else
				begin
					set @sql = 'select ' + quotename(@idField) + ', ' + quotename(@fieldName) + ' from ' + quotename(@tableName) + ' where ' + @fieldName + ' = ' + @lookupValue;
				end
			end
			else
			begin
				set @sql = 'select ' + quotename(@idField) + ', ' + quotename(@fieldName) + ' from ' + quotename(@tableName) + ' where ' + @fieldName + ' = ' + @lookupValue;
			end
		end
	END
	ELSE
	BEGIN
		if @fieldName like 'dbo.%'
		begin
			set @sql = 'select ' +  @fieldName + ' from ' + quotename(@tableName) + ' where ' + @fieldName + ' = ' + @lookupValue;
		end
		else
		begin
			if @isListField = 1
			begin
				if @fieldFrom = 1
				begin
					set @sql = 'select ' + ' [customEntityAttributeListItems].[item] from ' + quotename(@tableName) + ' left join [customEntityAttributeListItems] on [customEntityAttributeListItems].[valueid] = ' + quotename(@tableName) + '.' + quotename(@fieldName) + ' where ' + @fieldName + ' = ' + @lookupValue;
					set @orderFieldName = 'item';			
				end
				else if @fieldFrom = 2
				begin
					set @sql = 'select ' + ' [userdefined_list_items].[item] from ' + quotename(@tableName) + ' left join [userdefined_list_items] on [userdefined_list_items].[valueid] = ' + quotename(@tableName) + '.' + quotename(@fieldName) + ' where ' + @fieldName + ' = ' + @lookupValue;
					set @orderFieldName = 'item';			
				end
				else
				begin
					set @sql = 'select ' +  quotename(@fieldName) + ' from ' + quotename(@tableName) + ' where ' + @fieldName + ' = ' + @lookupValue;
				end
			end
			else
			begin
				set @sql = 'select ' + quotename(@fieldName) + ' from ' + quotename(@tableName) + ' where ' + @fieldName + ' = ' + @lookupValue;
			end
		end
	END
	exec sp_executesql @sql;
END
