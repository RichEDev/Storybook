CREATE PROCEDURE [dbo].[getRelationshipValues]
@relatedTable uniqueidentifier,
@displayField uniqueidentifier,
@subAccountId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @tableName nvarchar(250) = (select tablename from tables where tableid = @relatedTable);
	declare @fieldName nvarchar(100);
	declare @fieldFrom int;
	declare @isListField bit;
	declare @orderFieldName nvarchar(100);
	
	select  @fieldName = field, @orderFieldName = field, @fieldFrom = fieldFrom, @isListField = valuelist from fields where fieldid = @displayField;

	declare @idField nvarchar(100) = (select field from fields where tableid = @relatedTable and idfield = 1);

	declare @sql nvarchar(500) = '';
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
				set @sql = 'select ' + quotename(@idField) + ', [customEntityAttributeListItems].[item] from ' + quotename(@tableName) + ' left join [customEntityAttributeListItems] on [customEntityAttributeListItems].[valueid] = ' + quotename(@tableName) + '.' + quotename(@fieldName);
				set @orderFieldName = 'item';			
			end
			else if @fieldFrom = 2
			begin
				set @sql = 'select ' + quotename(@idField) + ', [userdefined_list_items].[item] from ' + quotename(@tableName) + ' left join [userdefined_list_items] on [userdefined_list_items].[valueid] = ' + quotename(@tableName) + '.' + quotename(@fieldName);
				set @orderFieldName = 'item';			
			end
			else
			begin
				set @sql = 'select ' + quotename(@idField) + ', ' + quotename(@fieldName) + ' from ' + quotename(@tableName);
			end
		end
		else
		begin
			set @sql = 'select ' + quotename(@idField) + ', ' + quotename(@fieldName) + ' from ' + quotename(@tableName);
		end
	end

	if exists (select column_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @tableName and COLUMN_NAME = 'subAccountId')
	begin
		set @sql = @sql + ' WHERE subAccountId = @subAccId OR subAccountId IS NULL';
	end
	
	set @sql = @sql + ' ORDER BY ' + @orderFieldName;

	declare @params nvarchar(100) = '@subAccId int';
	exec sp_executesql @sql, @params, @subAccountId;
END