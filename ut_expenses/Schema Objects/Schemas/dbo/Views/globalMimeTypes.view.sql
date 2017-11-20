CREATE VIEW dbo.globalMimeTypes
AS
SELECT     mimeID, fileExtension, mimeHeader, description
FROM         [$(targetMetabase)].dbo.mime_headers AS globalMimeTypes
UNION ALL
SELECT     customMimeID, fileExtension, mimeHeader, description
FROM         customMimeHeaders

