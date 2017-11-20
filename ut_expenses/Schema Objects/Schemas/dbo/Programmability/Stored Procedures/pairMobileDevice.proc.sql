CREATE PROCEDURE dbo.pairMobileDevice
@pairingKey nvarchar(30),
@serialKey nvarchar(200),
@employeeID int
AS
BEGIN
	DECLARE @count int = 0;
	SELECT @count = COUNT(mobileDeviceID) FROM mobileDevices WHERE pairingKey = @pairingKey AND deviceSerialKey = @serialKey;

	IF @count > 0
	BEGIN
		-- re-pairing existing key
		RETURN 0;
	END
	ELSE
	BEGIN
		SELECT @count = COUNT(mobileDeviceID) FROM mobileDevices WHERE pairingKey = @pairingKey AND deviceSerialKey <> @serialKey AND deviceSerialKey <> '';
	
		IF @count > 0
		BEGIN
			RETURN -1; -- MobileReturnCode for Pairing In Use
		END
	
		UPDATE mobileDevices SET deviceSerialKey=@serialKey, modifiedOn=GETUTCDATE(), modifiedBy=@employeeID WHERE employeeID=@employeeID AND pairingKey=@pairingKey;
		
		RETURN 0; -- MobileReturnCode for Successful
	END
END
