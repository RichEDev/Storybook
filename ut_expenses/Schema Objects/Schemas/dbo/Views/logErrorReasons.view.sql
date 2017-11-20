CREATE VIEW dbo.logErrorReasons
AS
SELECT     logReasonID, reasonType, reason, createdon, modifiedon
FROM         [$(targetMetabase)].dbo.logErrorReasons AS logErrorReasons_1

