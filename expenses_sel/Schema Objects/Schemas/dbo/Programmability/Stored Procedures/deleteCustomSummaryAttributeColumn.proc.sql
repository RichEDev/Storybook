
CREATE PROCEDURE dbo.deleteCustomSummaryAttributeColumn
@columnid int
AS
	delete from custom_entity_attribute_summary_columns where columnid=@columnid;
