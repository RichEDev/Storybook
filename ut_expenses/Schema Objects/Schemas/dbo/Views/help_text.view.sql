CREATE VIEW dbo.help_text
AS
SELECT     page, description, helptext, tooltipID, tooltipArea, moduleID
FROM         dbo.customised_help_text
UNION
SELECT     page, description, helptext, tooltipID, tooltipArea, moduleID
FROM         [$(targetMetabase)].dbo.help_text AS help_text_1
WHERE     (tooltipID NOT IN
                          (SELECT     tooltipID
                            FROM          dbo.customised_help_text AS customised_help_text_1))

