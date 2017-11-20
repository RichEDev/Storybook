
CREATE PROCEDURE dbo.deleteCustomOneToManySummaryAttributeColumn
@columnid int
AS
	delete from [customEntityAttributeSummaryColumns] where columnid=@columnid;
