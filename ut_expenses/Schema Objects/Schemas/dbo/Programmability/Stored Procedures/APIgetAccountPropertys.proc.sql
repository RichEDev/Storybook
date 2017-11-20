--This stored procedure is getting used by ESRv2 so be careful before modifying it
CREATE PROCEDURE [dbo].[APIgetAccountPropertys]
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