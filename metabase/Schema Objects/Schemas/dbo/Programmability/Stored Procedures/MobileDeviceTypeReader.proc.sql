
CREATE PROCEDURE [dbo].[MobileDeviceTypeReader] 
AS
BEGIN
	select [dbo].[mobileDeviceTypes].[mobileDeviceTypeID],[dbo].[mobileDeviceTypes].[model] FROM mobileDeviceTypes
END
