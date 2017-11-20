-- work item ID: 46404
-- sequence: 4
CREATE PROCEDURE [dbo].[MobileDeviceTypeReader] 
AS
BEGIN
      select [dbo].[mobileDeviceTypes].[mobileDeviceTypeID],[dbo].[mobileDeviceTypes].[model], [dbo].[mobileDeviceTypes].mobileDeviceOSType FROM mobileDeviceTypes
END