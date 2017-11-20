CREATE PROCEDURE [dbo].[SaveEmployeeWorkAddress] 
@EmployeeWorkAddressId INT, 
@employeeId INT, 
@addressId INT, 
@startDate DATETIME, 
@endDate DATETIME, 
@active BIT,
@temporary BIT,
@ESRAssignmentLocationId int,
@date DATETIME, 
@rotational BIT,
@primaryRotational BIT,
@userId INT, 
@userEmployeeId INT, 
@userDelegateId INT
AS
DECLARE @count INT;
DECLARE @recordTitle NVARCHAR(2000);

SET @recordTitle = (
  SELECT + 'Work address for ' + (
    SELECT username
    FROM employees
    WHERE employeeid = @employeeid
    )
  );

IF (@EmployeeWorkAddressId = 0)
BEGIN
 INSERT INTO EmployeeWorkAddresses (EmployeeId, AddressId, StartDate, EndDate, Active, Temporary, CreatedOn, CreatedBy, ESRAssignmentLocationId, rotational, primaryrotational)
 VALUES (@employeeId, @addressId, @startDate, @endDate, @active, @temporary, @date, @userId, @ESRAssignmentLocationId, @rotational, @primaryRotational);

 SET @EmployeeWorkAddressId = scope_identity();

 IF @userEmployeeId > 0
 BEGIN
  EXEC addInsertEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @EmployeeWorkAddressId, @recordTitle, NULL;
 END
END
ELSE
BEGIN
 DECLARE @oldAddressId INT;
 DECLARE @oldStartDate DATETIME;
 DECLARE @oldEndDate DATETIME;
 declare @oldActive bit;
 declare @oldTemporary bit;

 SELECT @oldAddressId = AddressId, @oldStartDate = StartDate, @oldEndDate = EndDate
 FROM EmployeeWorkAddresses
 WHERE EmployeeWorkAddressId = @EmployeeWorkAddressId;

 UPDATE EmployeeWorkAddresses
 SET AddressId = @addressId, StartDate = @startDate, EndDate = @endDate, Active = @active, Temporary = @temporary, ModifiedBy = @userId, ModifiedOn = @date, ESRAssignmentLocationId = @ESRAssignmentLocationId, Rotational = @rotational, PrimaryRotational = @primaryRotational
 WHERE EmployeeWorkAddressId = @EmployeeWorkAddressId

 update ESRAssignmentLocations set StartDate = @startDate where ESRAssignmentLocationId = @ESRAssignmentLocationId

 IF @userEmployeeId > 0
 BEGIN
  IF @oldAddressId <> @addressId
   EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @EmployeeWorkAddressId, 'E059C9A7-01D0-41C4-BABC-451F9010683B', @oldAddressId, @addressId, @recordTitle, NULL;

  IF @oldStartDate <> @startDate
   EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @EmployeeWorkAddressId, '57D6995D-AD9D-4DDA-A400-AA0F8A62A2AA', @oldStartDate, @startDate, @recordTitle, NULL;

  IF @oldEndDate <> @endDate
   EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @EmployeeWorkAddressId, 'F155E3D2-D47C-4ADC-88DC-FF05BD58B681', @oldEndDate, @endDate, @recordTitle, NULL;

  IF @oldActive <> @active
   EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @EmployeeWorkAddressId, '18CC1487-9A39-4600-B161-9E29B7FA8AE1', @oldactive, @active, @recordtitle, null;
  
  IF @oldTemporary <> @temporary
   EXEC addUpdateEntryToAuditLog @userEmployeeId, @userDelegateId, 25, @EmployeeWorkAddressId, '10582607-5516-41BE-9148-09D04EC0F0A9', @oldtemporary, @temporary, @recordtitle, null;

 END
END

UPDATE employees
SET modifiedon = getdate()
WHERE employeeid = @employeeId;

RETURN @EmployeeWorkAddressId
