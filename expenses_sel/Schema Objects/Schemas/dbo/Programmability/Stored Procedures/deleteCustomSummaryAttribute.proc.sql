
CREATE PROCEDURE dbo.deleteCustomSummaryAttribute
@attributeid int,
@CUemployeeID int,
@CUdelegateID int
AS
	delete from custom_entity_attribute_summary_columns where attributeid = @attributeid;
	delete from custom_entity_attribute_summary where attributeid = @attributeid;
	delete from custom_entity_attributes where attributeid = @attributeid;	
