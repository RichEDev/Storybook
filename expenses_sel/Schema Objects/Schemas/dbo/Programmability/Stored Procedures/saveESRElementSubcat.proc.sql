

CREATE PROCEDURE [dbo].[saveESRElementSubcat]
	
@elementSubcatID int,
@elementID int,
@subcatID int,
@employeeID INT,
@delegateID INT

AS

BEGIN
	INSERT INTO ESRElementSubcats (elementID, subcatID) VALUES (@elementID, @subcatID)
	SET @elementSubcatID = scope_identity()

	exec addInsertEntryToAuditLog @employeeID, @delegateID, 26, @elementSubcatID, 'Element Subcat', null;
END

return @elementSubcatID





 
