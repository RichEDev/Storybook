
CREATE VIEW [dbo].[globalMimeTypes]
AS
SELECT mimeID, fileExtension, mimeHeader, description
FROM      [$(metabaseExpenses)].dbo.mime_headers AS globalMimeTypes
UNION ALL
SELECT customMimeID, fileExtension, mimeHeader, description
 FROM customMimeHeaders

