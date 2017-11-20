CREATE PROCEDURE [dbo].[SaveHomeAddressFromEsrOutbound] 
	@employeeID int,
	@addressID int
AS
BEGIN

 DECLARE @date DATETIME;
 SET @date = DATEADD(D, 0, DATEDIFF(D, 0, GETUTCDATE()));

 IF EXISTS (SELECT * FROM employeeHomeAddresses WHERE AddressId = @addressID AND EmployeeId = @employeeID)
 BEGIN
	-- address is already assigned
	
	-- update endDate on other addresses where necessary
    UPDATE employeeHomeAddresses SET EndDate = @date WHERE AddressId <> @addressID and EmployeeId = @employeeID and (EndDate is null or EndDate >= @date or EndDate = '1900/01/01')
    -- update startdate and enddate for the specified address
	UPDATE employeeHomeAddresses SET StartDate = @date, EndDate = NULL WHERE AddressId = @addressID AND EmployeeId = @employeeID AND EndDate IS NOT NULL;
 
 END
 ELSE
 BEGIN
	-- address has never been assigned before
	
	-- update endDate on other addresses where necessary
    UPDATE employeeHomeAddresses SET EndDate = @date WHERE EmployeeId = @employeeID and (EndDate is null or EndDate >= @date or EndDate = '1900/01/01')
	-- insert new address assignment with a start date    
    INSERT INTO employeeHomeAddresses (EmployeeId, AddressId, StartDate, CreatedOn, CreatedBy) VALUES (@employeeID, @addressID, @date, GETUTCDATE(), @employeeID)

  END
  RETURN @@ERROR;
END