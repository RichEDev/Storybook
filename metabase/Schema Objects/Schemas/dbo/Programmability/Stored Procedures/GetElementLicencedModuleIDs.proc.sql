
CREATE PROCEDURE [dbo].[GetElementLicencedModuleIDs]
	@elementID int,
	@accountID int
AS
BEGIN
	SELECT moduleElementBase.moduleID FROM accountsLicencedElements 
		INNER JOIN moduleElementBase ON accountsLicencedElements.elementID = moduleElementBase.elementID 
		WHERE accountsLicencedElements.elementID = @elementID AND accountsLicencedElements.accountID = @accountID
END
