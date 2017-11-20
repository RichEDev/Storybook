CREATE PROCEDURE [dbo].[GetCustomMimeHeaders] 

AS
BEGIN
	SELECT customMimeID, fileExtension, mimeHeader, description FROM dbo.customMimeHeaders
END
