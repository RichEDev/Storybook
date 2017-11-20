
CREATE PROCEDURE [dbo].[GetElementLicencedModuleIDs]
	@moduleID tinyint,
	@accountID int
AS
BEGIN
	SELECT moduleElementBase.elementID FROM accountsLicencedElements 
		INNER JOIN moduleElementBase ON accountsLicencedElements.elementID = moduleElementBase.elementID 
		WHERE moduleElementBase.moduleID = @moduleID AND accountsLicencedElements.accountID = @accountID
END
