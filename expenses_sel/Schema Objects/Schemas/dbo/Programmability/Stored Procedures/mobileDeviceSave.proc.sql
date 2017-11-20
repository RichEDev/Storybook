CREATE PROCEDURE [dbo].[mobileDeviceSave] (@mobileDeviceID INT, @employeeID INT, @deviceTypeID INT, @deviceName NVARCHAR(100), @pairingKey NVARCHAR(30), @deviceSerialKey NVARCHAR(200))
AS
BEGIN
	DECLARE @duplicateCheck INT;
	DECLARE @newMobileDeviceID INT;
	SET @duplicateCheck = (SELECT COUNT(*) FROM mobileDevices WHERE deviceName=@deviceName AND mobileDeviceID <> @mobileDeviceID);

	IF @duplicateCheck = 0
	BEGIN

		IF @mobileDeviceID = 0
		BEGIN
			INSERT INTO mobileDevices (employeeID, deviceTypeID, deviceName, pairingKey, deviceSerialKey) VALUES (@employeeID, @deviceTypeID, @deviceName, @pairingKey, @deviceSerialKey);
			SET @newMobileDeviceID = @@IDENTITY;
		END
		ELSE
		BEGIN
			UPDATE mobileDevices SET employeeID=@employeeID, deviceTypeID=@deviceTypeID, deviceName=@deviceName, pairingKey=@pairingKey, deviceSerialKey=@deviceSerialKey WHERE mobileDeviceID=@mobileDeviceID	
			SET @newMobileDeviceID = @mobileDeviceID;
		END
	END
	ELSE
	BEGIN
		SET @newMobileDeviceID = -1;	
	END

	RETURN @newMobileDeviceID;
END
