CREATE PROCEDURE [dbo].[SaveWorkAddressFromEsrOutbound] 
	@employeeID int,
	@addressID int
AS
BEGIN
	DECLARE @date DATETIME;
	SET @date = DATEADD(D, 0, DATEDIFF(D, 0, GETUTCDATE()));

	IF EXISTS (SELECT * FROM EmployeeWorkAddresses WHERE AddressId = @addressID AND EmployeeId = @employeeID)
	BEGIN
		-- address is already assigned
	
		-- update endDate on other addresses where necessary
		UPDATE EmployeeWorkAddresses SET EndDate = @date WHERE AddressId <> @addressID and EmployeeId = @employeeID and (EndDate is null or EndDate >= @date or EndDate = '1900/01/01')
		-- update startdate and enddate for the specified address
		UPDATE EmployeeWorkAddresses SET StartDate = @date, EndDate = NULL WHERE AddressId = @addressID AND EmployeeId = @employeeID AND EndDate IS NOT NULL;
		
	END
	ELSE
	BEGIN
		-- address has never been assigned before
	
		-- update endDate on other addresses where necessary
		UPDATE EmployeeWorkAddresses SET EndDate = @date WHERE EmployeeId = @employeeID and (EndDate is null or EndDate >= @date or EndDate = '1900/01/01')
		-- insert new address assignment with a start date    
	    INSERT INTO EmployeeWorkAddresses (EmployeeId, AddressId, StartDate, Active, Temporary, CreatedOn, CreatedBy) VALUES (@employeeID, @addressID, @date, 1, 0, GETUTCDATE(), @employeeID)

	END
    RETURN @@ERROR;
END