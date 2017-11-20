-- work item ID: 46406
-- sequence: 6
CREATE PROCEDURE [dbo].[mobileDeviceTypeOsReader]
AS
BEGIN
      SELECT mobileDeviceOSTypeId, mobileInstallFrom, mobileImage FROM mobileDeviceOSTypes
END
