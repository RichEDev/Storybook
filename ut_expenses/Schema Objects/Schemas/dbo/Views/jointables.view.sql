CREATE VIEW dbo.jointables
AS
SELECT     jointableid, tableid, basetableid, description, amendedon, CAST(0 AS INT) AS joinFrom
FROM         [$(targetMetabase)].dbo.jointables_base
UNION ALL
SELECT     jointableid, tableid, basetableid, description, amendedon, CAST(1 AS INT) AS joinFrom
FROM         dbo.[customJoinTables]
GO