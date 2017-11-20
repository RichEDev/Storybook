CREATE VIEW dbo.treeviewCommonFields
AS
SELECT dbo.custom_entities.tableid, dbo.custom_entity_attributes.fieldid
FROM  dbo.custom_entity_attributes INNER JOIN
               dbo.custom_entities ON dbo.custom_entities.entityid = dbo.custom_entity_attributes.entityid
