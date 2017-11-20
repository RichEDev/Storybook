CREATE PROCEDURE [dbo].[getAutoCompleteOptions]
@maxRows int,
@matchTableID uniqueidentifier,
@displayFieldID uniqueidentifier,
@matchFields GuidPK READONLY,
@matchText nvarchar(20)
AS
declare @matchFieldID uniqueidentifier;
declare @sql nvarchar(4000) = 'SELECT TOP ' + CAST(@maxRows as nvarchar) + ' ';
declare @joins AS TABLE
(
	joinToTableName nvarchar(100) NOT NULL,
	joinFromTableName nvarchar(100) NOT NULL,
	joinFromFieldName nvarchar(100) NOT NULL,
	joinAliasNumber int NOT NULL,
	joinString nvarchar(500) NOT NULL
);
declare @fieldName nvarchar(50);
declare @fieldType nvarchar(2);
declare @tableName nvarchar(50);
declare @isValueList bit;
declare @joinTableName nvarchar(100);
declare @joinNumber int = 0;
declare @fieldFrom int;

set @tableName = (select tablename from tables where tableid = @matchTableID);

-- Add ID field
set @fieldName = (select field from fields where fieldid = (select primarykey from tables where tableid = @matchTableID));

if @fieldName is not null 
begin
	set @sql = @sql + QUOTENAME(@fieldName) + ' AS [ID], ';
end
else
begin
	set @sql = @sql + '-1 AS [ID], ';
end

-- Add display field
select @fieldName = field, @fieldType = fieldtype, @isValueList = valuelist, @fieldFrom = fieldFrom from fields where fieldid = @displayFieldID;

if @fieldName is not null 
begin
	if @fieldType = 'D'
		set @sql = @sql + 'CONVERT(nvarchar,' + @fieldName + ', 102)';
	else if @fieldType = 'N' and @isValueList = 1
	begin
		SELECT @joinTableName = CASE @fieldFrom
			WHEN 1 THEN 'customEntityAttributeListItems'
			WHEN 2 THEN 'userdefined_list_items'
			ELSE ''
			END;
						
		if @joinTableName <> ''
		begin
			if not exists (select @joinTableName from @joins where joinToTableName = @joinTableName and joinFromTableName = @tableName and joinFromFieldName = @fieldName)
			begin
				set @joinNumber = @joinNumber + 1;
				insert into @joins VALUES (@joinTableName, @tableName, @fieldName, @joinNumber, 'left join ' + QUOTENAME(@joinTableName) + ' AS ' + QUOTENAME(@joinTableName + cast(@joinNumber as nvarchar)) + ' on ' + QUOTENAME(@tableName) + '.' + QUOTENAME(@fieldName) + ' = ' + QUOTENAME(@joinTableName + cast(@joinNumber as nvarchar)) + '.[valueid]');
				
				set @sql = @sql + QUOTENAME(@joinTableName + cast(@joinNumber as nvarchar)) + '.[item]';
				
			end
		end
	end
	else
		set @sql = @sql + @fieldName;
	
	set @sql = @sql + ' AS [Display] ';
end
else
begin
	set @sql = @sql + '''< Display Field Error >'' AS [Display] ';
end

set @sql = @sql + 'FROM ' + QUOTENAME(@tableName) + ' ';

-- apply WHERE clause
declare @joiner nvarchar(4) = '';
declare @where nvarchar(MAX) = '';

if (select count(ID) from @matchFields) > 0
begin
	set @where = 'WHERE ';
	
	declare lp cursor for
	select ID from @matchFields
	open lp
	fetch next from lp into @matchFieldID
	while @@FETCH_STATUS = 0
	begin
		select @fieldName = field, @fieldType = fieldtype, @isValueList = valuelist, @fieldFrom = fieldFrom from fields where fieldid = @matchFieldID;
		if @fieldName is not null
		begin
			if @fieldType = 'N' AND @isValueList = 1
			begin
				SELECT @joinTableName = CASE @fieldFrom
					WHEN 1 THEN 'customEntityAttributeListItems'
					WHEN 2 THEN 'userdefined_list_items'
					ELSE ''
					END;
						
				if @joinTableName <> ''
				begin
					if not exists (select @joinTableName from @joins where joinToTableName = @joinTableName and joinFromTableName = @tableName and joinFromFieldName = @fieldName)
					begin
						set @joinNumber = @joinNumber + 1;
						insert into @joins VALUES (@joinTableName, @tableName, @fieldName, @joinNumber, 'left join ' + QUOTENAME(@joinTableName) + ' AS ' + QUOTENAME(@joinTableName + cast(@joinNumber as nvarchar)) + ' on ' + QUOTENAME(@tableName) + '.' + QUOTENAME(@fieldName) + ' = ' + QUOTENAME(@joinTableName + cast(@joinNumber as nvarchar)) + '.[valueid]');
						
						set @where = @where + @joiner + QUOTENAME(@joinTableName + cast(@joinNumber as nvarchar)) + '.[item] LIKE @wildcard';
					end
					else
					begin
						set @where = @where + @joiner + QUOTENAME(@joinTableName + (select cast(joinAliasNumber as nvarchar) from @joins where joinToTableName = @joinTableName and joinFromTableName = @tableName and joinFromFieldName = @fieldName)) + '.[item] LIKE @wildcard';
					end
				end
				
			end
			else
			begin
				set @where = @where + @joiner + @fieldName + ' LIKE @wildcard'
			end
			
			set @joiner = ' OR ';
		end
	
		fetch next from lp into @matchFieldID	
	end
	close lp;
	deallocate lp;
end


-- add any joins
if (select count(*) from @joins) > 0
begin
	declare @str nvarchar(500);
	
	declare lp cursor for
	select joinString from @joins
	open lp
	fetch next from lp into @str
	while @@FETCH_STATUS = 0
	begin
		set @sql = @sql + @str + ' ';
		
		fetch next from lp into @str
	end
	close lp
	deallocate lp;
end

set @sql = @sql + @where;

declare @retVals TABLE (recID int, recText nvarchar(150));
declare @sqlParams nvarchar(250) = '@wildcard nvarchar(MAX)';
exec sp_executesql @sql, @sqlParams, @wildcard = @matchText