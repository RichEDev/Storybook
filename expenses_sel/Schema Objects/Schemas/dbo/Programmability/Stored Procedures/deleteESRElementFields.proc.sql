



CREATE PROCEDURE [dbo].[deleteESRElementFields]
	
@elementID int,
@employeeID INT,
@delegateID INT

AS
BEGIN
	DELETE FROM ESRElementFields WHERE elementID = @elementID
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 26, @elementID, 'All Element Fields', null;	
END


