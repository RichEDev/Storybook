CREATE PROCEDURE [dbo].[GetAccountProperties]
@subaccountId INT
AS
SELECT subAccountID
	,stringKey
	,stringValue
	,formPostKey
	,isGlobal
FROM accountProperties
WHERE subAccountID = @subaccountId
ORDER BY stringKey
