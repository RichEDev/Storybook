


CREATE PROCEDURE [dbo].[deleteESRElementSubcats]
	
@elementID int,
@employeeID INT,
@delegateID INT

AS
BEGIN
	DELETE FROM ESRElementSubcats WHERE elementID = @elementID
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 26, @elementID, 'All Element Subcats', null;
END


