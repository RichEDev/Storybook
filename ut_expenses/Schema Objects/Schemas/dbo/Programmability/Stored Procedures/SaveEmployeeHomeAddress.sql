CREATE PROCEDURE [dbo].[saveEmployeeHomeAddress] 
@employeeHomeAddressId INT, 
@employeeId INT, 
@addressId INT, 
@startDate DATETIME, 
@endDate DATETIME, 
@date DATETIME, 
@userId INT, 
@userEmployeeId INT, 
@userDelegateId INT
AS
DECLARE @count INT;
DECLARE @recordTitle NVARCHAR(2000);

SET @recordTitle = (
		SELECT + 'Home address for ' + (
				SELECT username
				FROM employees
				WHERE employeeid = @employeeid
				)
		);

IF (@employeeHomeAddressId = 0)
BEGIN
	INSERT INTO employeeHomeAddresses (EmployeeId, AddressId, StartDate, EndDate, CreatedOn, CreatedBy)
	VALUES (@employeeId, @addressId, @startDate, @endDate, @date, @userId);

	SET @employeeHomeAddressId = scope_identity();

	IF @userEmployeeId > 0
	BEGIN
		EXEC addInsertEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @employeeHomeAddressId, @recordTitle, NULL;
	END
END
ELSE
BEGIN
	DECLARE @oldAddressId INT;
	DECLARE @oldStartDate DATETIME;
	DECLARE @oldEndDate DATETIME;

	SELECT @oldAddressId = AddressId, @oldStartDate = StartDate, @oldEndDate = EndDate
	FROM employeeHomeAddresses
	WHERE EmployeeHomeAddressId = @employeeHomeAddressId;

	UPDATE employeeHomeAddresses
	SET AddressId = @addressId, StartDate = @startDate, EndDate = @endDate, ModifiedBy = @userId, ModifiedOn = @date
	WHERE EmployeeHomeAddressId = @employeeHomeAddressId

	IF @userEmployeeId > 0
	BEGIN
		IF @oldAddressId <> @addressId
			EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @employeeHomeAddressId, 'c3ea2dc0-3971-4e01-b6b9-30727c96bc67', @oldAddressId, @addressId, @recordTitle, NULL;

		IF @oldStartDate <> @startDate
			EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @employeeHomeAddressId, '7969db25-d2c4-478b-92a2-5b08bdec1326', @oldStartDate, @startDate, @recordTitle, NULL;

		IF @oldEndDate <> @endDate
			EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @employeeHomeAddressId, 'fea8d6a3-3daa-4d65-bc6d-9f8f47647e81', @oldEndDate, @endDate, @recordTitle, NULL;
	END
END

UPDATE employees
SET modifiedon = getdate()
WHERE employeeid = @employeeId;

RETURN @employeeHomeAddressId
