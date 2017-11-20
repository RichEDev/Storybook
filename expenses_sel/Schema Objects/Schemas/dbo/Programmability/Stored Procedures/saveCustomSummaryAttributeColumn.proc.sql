
CREATE PROCEDURE dbo.saveCustomSummaryAttributeColumn
@columnid int,
@attributeid int,
@column_fieldname nvarchar(100),
@alternate_header nvarchar(150),
@width int,
@order int,
@filterVal nvarchar(100),
@default_sort bit
AS
DECLARE @count int;
DECLARE @retID int;
SET @retID = 0;

IF @columnid > 0
BEGIN
	SET @count = (select count(columnid) from custom_entity_attribute_summary_columns where attributeid = @attributeid and column_fieldname = @column_fieldname and columnid <> @columnid);
	
	if @count = 0
	begin
		update custom_entity_attribute_summary_columns set default_sort=@default_sort, column_fieldname = @column_fieldname, alternate_header=@alternate_header, width=@width, [order]=@order, filterVal=@filterVal
		where columnid=@columnid;

		set @retID = @columnid;		
	end
END
ELSE
BEGIN
	SET @count = (select count(columnid) from custom_entity_attribute_summary_columns where attributeid = @attributeid and column_fieldname = @column_fieldname);
	
	if @count = 0
	begin
		insert into custom_entity_attribute_summary_columns (attributeid, column_fieldname, alternate_header, width, [order], default_sort, filterVal)
		values (@attributeid, @column_fieldname, @alternate_header, @width, @order, @default_sort, @filterVal);
		
		set @retID = SCOPE_IDENTITY();	
	end
END

RETURN @retID
