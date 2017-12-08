CREATE PROCEDURE [dbo].[UpdateMobileMetricData] (@DeviceId   NVARCHAR(100),
                                                 @Model      NVARCHAR(50),
                                                 @Platform   NVARCHAR(10),
                                                 @OSVersion  NVARCHAR(100),
                                                 @AppVersion NVARCHAR(100), 
												 @EmployeeId INT)
AS
BEGIN
      DECLARE @Id AS INT
      DECLARE @returnId AS INT

      SET @Id = (SELECT id
                 FROM   [MobileMetricData]
                 WHERE  [MobileMetricData].deviceid = @DeviceId 
				 AND    [MobileMetricData].EmployeeId = @EmployeeId)

      IF @Id IS NOT NULL
        BEGIN
            -- Device already exists so update last used column to now
                UPDATE [MobileMetricData]
				SET    lastused = Getdate(), OSVersion = @OSVersion, AppVersion = @AppVersion, Active= 1
				WHERE  [MobileMetricData].Id = @Id;

			-- Set other employees registered to the device to inactive
				UPDATE mobilemetricdata
				SET active = 0
				WHERE deviceid = @DeviceId
				AND employeeid <> @EmployeeId;

            SET @returnId = @Id
        END
      ELSE
        BEGIN
            INSERT INTO [dbo].[MobileMetricData]
                        ([deviceid],
                         [model],
                         [platform],
                         [osversion],
                         [appversion],
						 [EmployeeId],
                         [lastused],
						 [Active])
            VALUES      (@DeviceId,
                         @Model,
                         @Platform,
                         @OSVersion,
                         @AppVersion,
						 @EmployeeId,
                         Getdate(),
						 1);

            SET @returnId = Scope_identity()
        END

	/* Return details based on Return Id */	
	SELECT	
		Id, EmployeeId, AllowNotifications, Registered, PushChannel, RegistrationId,RegisteredTag 
	FROM [MobileMetricData] 
	WHERE 
		Id = @returnId;
  END
  GO