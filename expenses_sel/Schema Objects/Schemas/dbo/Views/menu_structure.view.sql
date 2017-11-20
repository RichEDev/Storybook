
CREATE VIEW [dbo].[menu_structure]
AS
SELECT     menuid, menu_name, parentid, CAST (0 AS BIT) AS custom
FROM         [$(metabaseExpenses)].dbo.menu_structure_base
UNION ALL
SELECT     menuid, menu_name, parentid, CAST (1 AS BIT) AS custom
FROM         dbo.custom_menu_structure

