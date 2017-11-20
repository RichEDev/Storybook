
CREATE VIEW [dbo].[jointables]
AS
SELECT     jointableid, tableid, basetableid, description, amendedon
FROM         [$(metabaseExpenses)].dbo.jointables_base
UNION ALL
SELECT     jointableid, tableid, basetableid, description, amendedon
FROM         dbo.custom_jointables

