CREATE PROCEDURE [dbo].[saveMobileDevice] 
(
@mobileDeviceID INT, 
@employeeID INT, 
@deviceTypeID INT, 
@deviceName NVARCHAR(100), 
@pairingKey NVARCHAR(30), 
@deviceSerialKey NVARCHAR(200),
@requestorEmployeeId int
)
AS
BEGIN
	DECLARE @duplicateCheck INT;
	DECLARE @newMobileDeviceID INT;
	SET @duplicateCheck = (SELECT COUNT(*) FROM mobileDevices WHERE deviceName=@deviceName AND mobileDeviceID <> @mobileDeviceID AND employeeID = @employeeID);

	IF @duplicateCheck = 0
	BEGIN
		IF @mobileDeviceID = 0
		BEGIN
			INSERT INTO mobileDevices (employeeID, deviceTypeID, deviceName, pairingKey, deviceSerialKey, createdOn, createdBy) VALUES (@employeeID, @deviceTypeID, @deviceName, @pairingKey, @deviceSerialKey, GETUTCDATE(), @requestorEmployeeId);
			SET @newMobileDeviceID = @@IDENTITY;
			if @newMobileDeviceID > 0
			begin
				EXEC addInsertEntryToAuditLog @requestorEmployeeId, null, 165, @newMobileDeviceID, @deviceName, null;
			end
		END
		ELSE
		BEGIN
			declare @oldDeviceName nvarchar(100);
			declare @employeeName nvarchar(100);
			select @employeeName = firstname + ' ' + surname + ' Mobile Devices' from employees where employeeid = @employeeID;
			select @oldDeviceName = deviceName from mobileDevices where mobileDeviceID = @mobileDeviceID;
			
			UPDATE mobileDevices SET employeeID=@employeeID, deviceTypeID=@deviceTypeID, deviceName=@deviceName, pairingKey=@pairingKey, deviceSerialKey=@deviceSerialKey, modifiedOn=GETUTCDATE(), modifiedBy=@requestorEmployeeId WHERE mobileDeviceID=@mobileDeviceID;
			SET @newMobileDeviceID = @mobileDeviceID;
		
			if @oldDeviceName <> @deviceName
			begin
				exec addUpdateEntryToAuditLog @requestorEmployeeId, null, 165, @mobileDeviceID, 'A08EBDA1-250C-4497-B791-D02D9E937A8A', @oldDeviceName, @deviceName, @employeeName, null;
			end
		END
	END
	ELSE
	BEGIN
		SET @newMobileDeviceID = -1;
	END

	RETURN @newMobileDeviceID;
END
