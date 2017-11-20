
CREATE VIEW [dbo].[Elements]
AS
	select 0 as elementType, elementID, elementFriendlyName from [$(targetMetabase)].dbo.elementsBase
	union
	select 1 as elementType, entityid, entity_name as elementFriendlyName from customentities
	union
	select 2 as elementType, viewid, view_name  as elementFriendlyName from customEntityViews
	union
	select 3 as elementType, formid, form_name  as elementFriendlyName from customEntityForms