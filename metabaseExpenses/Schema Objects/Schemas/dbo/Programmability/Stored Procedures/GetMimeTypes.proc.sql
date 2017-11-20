
CREATE PROCEDURE [dbo].[GetMimeTypes] 

AS
BEGIN
	select mimeID, fileExtension, mimeHeader, [description] from dbo.mime_headers
END
