
CREATE VIEW [dbo].[help_text]
AS
SELECT     helpid, page, description, helptext, tooltipID, tooltipArea, moduleID
FROM         dbo.customised_help_text
UNION
SELECT     helpid, page, description, helptext, tooltipID, tooltipArea, moduleID
FROM         [$(metabaseExpenses)].dbo.help_text AS help_text_1
WHERE     (helpid NOT IN
                          (SELECT     helpid
                            FROM          dbo.customised_help_text AS customised_help_text_1))

