
CREATE PROCEDURE dbo.saveCustomSummaryAttributeElement
@summary_attributeid int,
@attributeid int,
@otm_attributeid int,
@order tinyint,
@CUemployeeID int,
@CUdelegateID int
AS
DECLARE @count int;
DECLARE @retID int;
SET @retID = 0;

IF @summary_attributeid > 0
BEGIN
	SET @count = (select count(summary_attributeid) from [customEntityAttributeSummary] where attributeid = @attributeid and otm_attributeid = @otm_attributeid and summary_attributeid <> @summary_attributeid);
	if @count = 0
	begin
		update [customEntityAttributeSummary] set [order]=@order
		where summary_attributeid = @summary_attributeid;
		
		set @retID = @summary_attributeid;
	end
END
ELSE
BEGIN
	SET @count = (select count(summary_attributeid) from [customEntityAttributeSummary] where attributeid = @attributeid and otm_attributeid = @otm_attributeid);
	
	if @count = 0
	begin
		insert into [customEntityAttributeSummary] (attributeid, otm_attributeid, [order])
		values (@attributeid, @otm_attributeid, @order);
		
		set @retID = SCOPE_IDENTITY();
	end
END

RETURN @retID;
