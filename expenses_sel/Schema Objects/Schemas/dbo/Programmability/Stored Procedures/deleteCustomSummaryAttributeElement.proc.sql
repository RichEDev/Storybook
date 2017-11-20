
CREATE PROCEDURE dbo.deleteCustomSummaryAttributeElement
@summary_attributeid int
AS
	delete from custom_entity_attribute_summary where summary_attributeid = @summary_attributeid;
