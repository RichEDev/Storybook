
CREATE FUNCTION dbo.employeeHasRole(@roleID as int, @employeeId as int, @subAccountId as int)
RETURNS bit
AS
BEGIN
	DECLARE @retVal AS bit;
	SET @retVal = 0;
	
	DECLARE @count AS INT;
	SELECT @count = count(accessRoleID) from employeeAccessRoles WHERE employeeId = @employeeId AND subAccountID = @subAccountId AND accessRoleID = @roleID;
	
	IF @count > 0
	BEGIN
		SET @retVal = 1;
	END
	
	RETURN @retVal;
END
