
CREATE PROCEDURE dbo.deleteCustomOneToManySummaryAttributeElement
@summary_attributeid int
AS
	delete from [customEntityAttributeSummary] where summary_attributeid = @summary_attributeid;
