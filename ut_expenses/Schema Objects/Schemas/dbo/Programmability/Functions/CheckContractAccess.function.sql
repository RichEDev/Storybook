CREATE FUNCTION [dbo].[CheckContractAccess] (@userID INT, @contractID INT, @subAccountID INT)  
RETURNS int AS  
BEGIN
DECLARE @AccessCount INT

SET @AccessCount = (
	SELECT COUNT(*) AS [AccessCount] FROM [contract_audience] WHERE [contractId] = @contractID AND 
	(
		([audienceType] = 0 AND [accessId] = @userID) 
		OR 
		([audienceType] = 1 AND [accessId] IN 
			(SELECT [teamid] FROM [teams] WHERE @userID IN
				(SELECT [employeeId] FROM [teamemps] WHERE [accessId] = [teamid])
			)
		)
                
                /****** contract accessible to Framework super user ******/
		OR
		 (select count(*) from accessRoleElementDetails where viewAccess='true' and elementId=188 and roleID in (select accessRoleId from employeeAccessRoles where employeeId = @userID and subaccountid = @subAccountID))>=1
	)
)

IF(@AccessCount = 0)
BEGIN
	-- Check that any restrictions exist
	DECLARE @tmpCount INT
	SET @tmpCount = (SELECT COUNT(*) FROM [contract_audience] WHERE [contractId] = @contractID)
	IF(@tmpCount = 0)
	BEGIN
		-- no restrictions exist, so permit
		SET @AccessCount = 999
	END
END

RETURN(@AccessCount) 
END