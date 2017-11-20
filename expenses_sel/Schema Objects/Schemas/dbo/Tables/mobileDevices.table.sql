CREATE TABLE [dbo].[mobileDevices] (
    [mobileDeviceID]  INT            IDENTITY (1, 1) NOT NULL,
    [employeeID]      INT            NOT NULL,
    [deviceTypeID]    INT            NOT NULL,
    [deviceName]      NVARCHAR (100) NOT NULL,
    [pairingKey]      NVARCHAR (30)  NULL,
    [deviceSerialKey] NVARCHAR (200) NULL
);

