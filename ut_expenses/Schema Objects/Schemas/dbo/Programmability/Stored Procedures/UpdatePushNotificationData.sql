CREATE PROCEDURE [dbo].[UpdatePushNotificationData] (@MobileDeviceId   INT,
                                                 @Registered   BIT,
                                                 @PushChannel  NVARCHAR(max),
                                                 @RegistrationId NVARCHAR(100),
												 @RegisteredTag NVARCHAR(50))
AS
BEGIN
	
	IF @Registered = 1
	BEGIN
			UPDATE [MobileMetricData]
			SET	AllowNotifications = @Registered,
				Registered = @Registered,
				PushChannel = @PushChannel,
				RegistrationId = @RegistrationId,
				RegisteredTag=@RegisteredTag,
				RegisteredOn = GETDATE()
			WHERE Id =@MobileDeviceId;

	END
	ELSE
	BEGIN
			UPDATE [MobileMetricData]
			SET	AllowNotifications = @Registered,
				Registered = @Registered
			WHERE Id =@MobileDeviceId;
	END


	/* Return details based on Return Id */	
	SELECT	
		Id, EmployeeId, AllowNotifications, Registered, PushChannel, RegistrationId, RegisteredTag
	FROM [MobileMetricData] 
	WHERE 
		Id = @MobileDeviceId;
  END
  GO