CREATE PROCEDURE [dbo].[SetMobileDeviceStatus] (
	@DeviceId NVARCHAR(100)
	,@AllowNotifications BIT
	,@EmployeeId INT
	,@Active BIT
	)
AS
BEGIN
	UPDATE mobilemetricdata
	SET allownotifications = @AllowNotifications, active = @Active
	WHERE deviceid = @DeviceId AND employeeid = @EmployeeId

	IF @Active = 1
	BEGIN
		--set other employees registered to the device to inactive
		UPDATE mobilemetricdata
		SET active = 0
		WHERE deviceid = @DeviceId
			AND employeeid <> @EmployeeId
	END
END