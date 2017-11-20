﻿CREATE PROCEDURE [dbo].[saveHomeLocationFromESROutbound] 
 @employeeID int,
 @newDate datetime,
 @locationID int,
 @createdby int
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 DECLARE @count INT;
 
 SET @count = (SELECT COUNT(1) FROM employeeHomeLocations WHERE locationID = @locationID AND employeeID=@employeeID);
  
 IF @count > 0
  RETURN -1;

 DECLARE @date DATETIME;

 SET @date = DATEADD(D, 0, DATEDIFF(D, 0, GETDATE()));
 
 SET @newDate = DATEADD(D, 0, DATEDIFF(D, 0, @newDate));

    UPDATE employeeHomeLocations SET endDate = @newDate WHERE employeeID = @employeeID and (endDate is null or endDate >= @date or endDate = '1900/01/01')
    
    INSERT INTO employeeHomeLocations (employeeID, locationID, startDate, createdOn, createdBy) VALUES (@employeeID, @locationID, @date, GETDATE(), @createdby)

    RETURN @@ERROR;
    
END