CREATE PROCEDURE [dbo].[UpdateMobileMetricData] (@DeviceId   NVARCHAR(100),
                                                 @Model      NVARCHAR(50),
                                                 @Platform   NVARCHAR(10),
                                                 @OSVersion  NVARCHAR(100),
                                                 @AppVersion NVARCHAR(100), 
					                             @EmployeeId INT)
AS
  BEGIN
      DECLARE @Id INT
      DECLARE @returnId INT

      SET @Id = (SELECT id
                 FROM   [MobileMetricData]
                 WHERE  [MobileMetricData].deviceid = @DeviceId 
		         AND    [MobileMetricData].EmployeeId = @EmployeeId)

      IF @Id IS NOT NULL
        BEGIN
            -- Device already exists so update last used column to now
            UPDATE [MobileMetricData]
            SET    lastused = Getdate(), OSVersion = @OSVersion, AppVersion = @AppVersion
            WHERE  [MobileMetricData].Id = @Id

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
                         [lastused])
            VALUES      (@DeviceId,
                         @Model,
                         @Platform,
                         @OSVersion,
                         @AppVersion,
		                 @EmployeeId,
                         Getdate())

            SET @returnId = Scope_identity()
        END

      RETURN @returnId
  END