
CREATE VIEW [dbo].[viewgroups]
AS
SELECT     groupname, [level], amendedon, viewgroupid, parentid, alias, relabel_param, CAST(0 AS BIT) AS ViewGroupFrom
FROM         [$(targetMetabase)].dbo.viewgroups_base
UNION
SELECT     entity_name, [level], amendedon, viewgroupid, parentid, alias, null as relabel_param, CAST(1 AS BIT) AS ViewGroupFrom
FROM         dbo.[customViewGroups]

