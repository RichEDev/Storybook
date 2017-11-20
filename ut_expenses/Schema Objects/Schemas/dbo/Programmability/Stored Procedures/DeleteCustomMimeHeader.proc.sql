CREATE PROCEDURE [dbo].[DeleteCustomMimeHeader] 
(
@mimeID uniqueidentifier, 
@employeeID INT,
@delegateID int
)
AS
DECLARE @fileExtension nvarchar(10);
DECLARE @count int;

SET @count = (SELECT count(globalMimeID) FROM mimeTypes where globalMimeID = @mimeID);

IF @count > 0
BEGIN
	return -1
END

select @fileExtension = fileExtension from customMimeHeaders where customMimeID = @mimeId;

delete from customMimeHeaders where customMimeID = @mimeID;

exec addDeleteEntryToAuditLog @employeeID, @delegateID, 159, 0, @fileExtension, null;

return 0
