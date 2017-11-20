CREATE PROCEDURE [dbo].[GetLicencedElementsByAccountId]
	@accountId int
AS
SELECT
	moduleID, moduleElementBase.elementID
FROM
	moduleElementBase
		join accountsLicencedElements on moduleElementBase.elementID = accountsLicencedElements.elementID
									and accountsLicencedElements.accountID = @accountId
ORDER by
	moduleID;
	
RETURN 0
