CREATE VIEW dbo.treeviewCommonFields
AS
SELECT dbo.[customEntities].tableid, dbo.[customEntityAttributes].fieldid
FROM  dbo.[customEntityAttributes] INNER JOIN
               dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
