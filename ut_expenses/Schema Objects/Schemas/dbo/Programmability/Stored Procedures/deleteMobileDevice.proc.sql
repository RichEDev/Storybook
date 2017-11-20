CREATE PROCEDURE [dbo].[deleteMobileDevice] (@mobileDeviceID INT, @CUemployeeID int, @CUdelegateID int) 
AS 
BEGIN
	DECLARE @rowCount int;
	DECLARE @devName nvarchar(100);
	DECLARE @recordTitle nvarchar(200);
	SELECT @recordTitle = dbo.getEmployeeFullName(employeeid) + ' Mobile Devices' from employees where employeeid = @CUemployeeID
	SELECT @devName = deviceName FROM mobileDevices WHERE mobileDeviceID = @mobileDeviceID;
	
	DELETE FROM [dbo].[mobileDevices] WHERE [dbo].[mobileDevices].[mobileDeviceID]=@mobileDeviceID;
	
	EXEC addDeleteEntryWithValueToAuditLog @CUemployeeID, @CUdelegateID, 165, @mobileDeviceID, @recordTitle, @devName, null;
	SET @rowCount = @@ROWCOUNT;
	
	return @rowCount;
END
