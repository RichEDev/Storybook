CREATE PROCEDURE [dbo].[DeleteEmployeeHomeAddress] 
	@employeeHomeAddressId INT
	,@userEmployeeId INT
	,@userDelegateId INT
AS
DECLARE @title1 NVARCHAR(500);

SELECT @title1 = username
FROM employees
WHERE employeeid = (
		SELECT employeeid
		FROM employeeHomeLocations
		WHERE employeelocationid = @employeeHomeAddressId
		);

DECLARE @recordTitle NVARCHAR(2000);

SET @recordTitle = (
		SELECT + 'Home address for ' + @title1
		);

DELETE
FROM employeeHomeAddresses
WHERE employeeHomeAddressId = @employeeHomeAddressId;

EXEC addDeleteEntryToAuditLog @userEmployeeId
	,@userDelegateId
	,25
	,@employeeHomeAddressId
	,@recordTitle
	,NULL;

RETURN
