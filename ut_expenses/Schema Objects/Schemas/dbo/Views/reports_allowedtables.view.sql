CREATE VIEW dbo.reports_allowedtables
AS
SELECT     basetableid, tableid
FROM         [$(targetMetabase)].dbo.reports_allowedtables_base
UNION ALL
SELECT DISTINCT dbo.[customEntities].tableid, dbo.[customEntityAttributes].relatedtable
FROM         dbo.[customEntities] INNER JOIN
                      dbo.[customEntityAttributes] ON dbo.[customEntityAttributes].entityid = dbo.[customEntities].entityid
WHERE     (dbo.[customEntityAttributes].fieldtype = 9)
UNION ALL
SELECT     dbo.[customEntities].tableid, dbo.[customEntities].tableid
FROM         dbo.[customEntities] 


