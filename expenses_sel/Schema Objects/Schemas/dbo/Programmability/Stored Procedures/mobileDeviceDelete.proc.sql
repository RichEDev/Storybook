CREATE PROCEDURE [dbo].[mobileDeviceDelete] (@mobileDeviceID INT) 
AS 
BEGIN
	DELETE FROM [dbo].[mobileDevices] WHERE [dbo].[mobileDevices].[mobileDeviceID]=@mobileDeviceID;
	return @@ROWCOUNT
END
