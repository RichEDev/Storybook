
CREATE VIEW [dbo].[reports_allowedtables]
AS
SELECT     basetableid, tableid
FROM         [$(metabaseExpenses)].dbo.reports_allowedtables_base
UNION ALL
SELECT DISTINCT dbo.custom_entities.tableid, dbo.custom_entity_attributes.relatedtable
FROM         dbo.custom_entities INNER JOIN
                      dbo.custom_entity_attributes ON dbo.custom_entity_attributes.entityid = dbo.custom_entities.entityid
WHERE     (dbo.custom_entity_attributes.fieldtype = 9)
UNION all
SELECT dbo.custom_entities.tableid, dbo.custom_entities.tableid
FROM         dbo.custom_entities 


