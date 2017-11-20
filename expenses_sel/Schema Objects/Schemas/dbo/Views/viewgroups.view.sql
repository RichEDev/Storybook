
CREATE VIEW [dbo].[viewgroups]
AS
SELECT     groupname, [level], amendedon, viewgroupid, parentid, alias
FROM         [$(metabaseExpenses)].dbo.viewgroups_base
UNION
SELECT     entity_name, [level], amendedon, viewgroupid, parentid, alias
FROM         dbo.custom_viewgroups

