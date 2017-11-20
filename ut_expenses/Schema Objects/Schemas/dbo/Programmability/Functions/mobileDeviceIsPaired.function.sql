CREATE FUNCTION dbo.mobileDeviceIsPaired (@mobileDeviceID INT) 
RETURNS BIT
BEGIN
	IF EXISTS (SELECT mobileDeviceID FROM mobileDevices WHERE mobileDeviceID = @mobileDeviceID AND (deviceSerialKey IS NULL OR LTRIM(RTRIM(deviceSerialKey)) = ''))
		RETURN 0;
	
	RETURN 1;
END