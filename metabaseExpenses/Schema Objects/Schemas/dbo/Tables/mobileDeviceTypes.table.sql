CREATE TABLE [dbo].[mobileDeviceTypes] (
    [mobileDeviceTypeID] INT           IDENTITY (12, 1) NOT NULL,
    [model]              NVARCHAR (50) NOT NULL,
    [mobileDeviceOSType] INT           NULL,
    CONSTRAINT [PK_mobileDeviceTypes] PRIMARY KEY CLUSTERED ([mobileDeviceTypeID] ASC),
    FOREIGN KEY ([mobileDeviceOSType]) REFERENCES [dbo].[mobileDeviceOSTypes] ([mobileDeviceOSTypeId])
);



