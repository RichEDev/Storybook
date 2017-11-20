
CREATE VIEW [dbo].[logErrorReasons]
AS
SELECT logReasonID, reasonType, reason, createdon, modifiedon
FROM  [$(metabaseExpenses)].dbo.logErrorReasons

