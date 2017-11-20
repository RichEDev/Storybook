CREATE PROCEDURE [dbo].[GetTooltipByTooltipId] @tooltipId uniqueidentifier
AS
SELECT helptext
	,tooltipID
FROM dbo.help_text
WHERE tooltipID = @tooltipId;