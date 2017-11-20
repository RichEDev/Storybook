﻿CREATE TABLE [dbo].[MobileMetricData]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [DeviceId] NVARCHAR(255) NULL, 
    [Model] NVARCHAR(100) NULL, 
    [Platform] NVARCHAR(10) NULL, 
    [OSVersion] NVARCHAR(10) NULL, 
    [AppVersion] NVARCHAR(100) NULL, 
    [LastUsed] NVARCHAR(100) NOT NULL, 
    [EmployeeId] INT NULL, 
    [Active] BIT NULL, 
    [AllowNotifications] BIT NULL 
)
