CREATE PROCEDURE [dbo].[DeleteEmployeeWorkAddress] 
@employeeWorkAddressId INT, 
@userEmployeeId INT, 
@userDelegateId INT
AS

begin tran;

DECLARE @title1 NVARCHAR(500);

SELECT @title1 = username
FROM employees
WHERE employeeid = (
  SELECT employeeid
  FROM EmployeeWorkAddresses
  WHERE EmployeeWorkAddressId = @employeeWorkAddressId
  );

DECLARE @recordTitle NVARCHAR(2000);

SET @recordTitle = (
  SELECT + 'Work address for ' + @title1
  );

delete from
	ESRAssignmentLocations
where
	ESRAssignmentLocationId in
	(
		select
			ESRAssignmentLocationId
		from
			EmployeeWorkAddresses
		where
			EmployeeWorkAddressId = @employeeWorkAddressId
	);

DELETE
FROM EmployeeWorkAddresses
WHERE EmployeeWorkAddressId = @employeeWorkAddressId;

EXEC addDeleteEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @employeeWorkAddressId, @recordTitle, NULL;

commit tran;

RETURN

