ALTER TABLE [dbo].[mobileDeviceTypes]
	ADD CONSTRAINT [FK_mobileDeviceTypes_mobileDeviceOSTypes] FOREIGN KEY ([mobileDeviceOSType]) REFERENCES [dbo].[mobileDeviceOSTypes] ([mobileDeviceOSTypeId]);