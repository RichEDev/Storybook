CREATE PROCEDURE [dbo].[saveCustomSummaryAttributeColumn]
@columnid int,
@attributeid int,
@columnAttributeId int,
@alternate_header nvarchar(150),
@width int,
@order int,
@filterVal nvarchar(100),
@default_sort bit,
@displayFieldId uniqueidentifier,
@joinViaId int
AS
DECLARE @count int;
DECLARE @retID int;
SET @retID = 0;

IF @columnid > 0
BEGIN
	SET @count = (select count(columnid) from [customEntityAttributeSummaryColumns] where attributeid = @attributeid and columnAttributeId = @columnAttributeId and columnid <> @columnid);
	
	if @count = 0
	begin
		update [customEntityAttributeSummaryColumns] set default_sort=@default_sort, columnAttributeId = @columnAttributeId, alternate_header=@alternate_header, width=@width, [order]=@order, filterVal=@filterVal, displayFieldId = @displayFieldId, joinViaID = @joinViaId
		where columnid=@columnid;

		set @retID = @columnid;		
	end
END
ELSE
BEGIN
	SET @count = (select count(columnid) from [customEntityAttributeSummaryColumns] where attributeid = @attributeid and columnAttributeId = @columnAttributeId);
	
	if @count = 0
	begin
		insert into [customEntityAttributeSummaryColumns] (attributeid, columnAttributeId, alternate_header, width, [order], default_sort, filterVal, displayFieldId, joinViaID)
		values (@attributeid, @columnAttributeId, @alternate_header, @width, @order, @default_sort, @filterVal, @displayFieldId, @joinViaId);
		
		set @retID = SCOPE_IDENTITY();
	end
END

RETURN @retID;