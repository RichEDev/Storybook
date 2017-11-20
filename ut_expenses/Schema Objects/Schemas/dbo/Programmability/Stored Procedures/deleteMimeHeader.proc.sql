CREATE PROCEDURE dbo.deleteMimeHeader 
(
@mimeId INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @fileExtension nvarchar(10);

select @fileExtension = fileExtension from mime_headers where mimeId = @mimeId;

delete from mime_headers where mimeId = @mimeId;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 138, @mimeId, @fileExtension, null;
