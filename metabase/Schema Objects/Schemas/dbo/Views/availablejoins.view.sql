CREATE VIEW [dbo].[availablejoins]
AS
SELECT     dbo.jointables_base.description, dbo.tables_base.tablename, dbo.fields_base.field, sourcetables.tablename AS sourcetable, dbo.tables_base.jointype, 
                      dbo.joinbreakdown_base.[order], sourcetables.tableid AS sourcetableid, dbo.tables_base.tableid, dbo.jointables_base.basetableid, dbo.joinbreakdown_base.jointableid, dbo.[joinbreakdown_base].destinationkey, destinationfields.field AS destinationfield
FROM         dbo.jointables_base INNER JOIN
                      dbo.joinbreakdown_base ON dbo.jointables_base.jointableid = dbo.joinbreakdown_base.jointableid INNER JOIN
                      dbo.tables_base ON dbo.joinbreakdown_base.tableid = dbo.tables_base.tableid LEFT OUTER JOIN
                      dbo.tables_base sourcetables ON sourcetables.tableid = dbo.joinbreakdown_base.sourcetable INNER JOIN
                      dbo.fields_base ON dbo.fields_base.fieldid = dbo.joinbreakdown_base.joinkey
					LEFT JOIN dbo.fields_base AS destinationfields ON destinationfields.fieldid = joinbreakdown_base.destinationkey

