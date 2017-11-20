
CREATE VIEW [dbo].[joinbreakdown]
AS
SELECT     joinbreakdownid, jointableid, [order], tableid, sourcetable, joinkey, amendedon, destinationkey
FROM         [$(metabaseExpenses)].dbo.joinbreakdown_base
UNION ALL
SELECT     joinbreakdownid, jointableid, [order], tableid, sourcetable, joinkey, amendedon, destinationkey
FROM         dbo.custom_joinbreakdown

