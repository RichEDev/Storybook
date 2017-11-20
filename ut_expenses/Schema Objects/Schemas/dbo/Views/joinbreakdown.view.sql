CREATE VIEW dbo.joinbreakdown
AS
SELECT     joinbreakdownid, jointableid, [order], tableid, sourcetable, joinkey, amendedon, destinationkey, CAST(0 AS INT) AS joinbreakdownFrom
FROM         [$(targetMetabase)].dbo.joinbreakdown_base
UNION ALL
SELECT     joinbreakdownid, jointableid, [order], tableid, sourcetable, joinkey, amendedon, destinationkey, CAST(1 AS INT) AS joinbreakdownFrom
FROM         dbo.[customJoinBreakdown]
GO