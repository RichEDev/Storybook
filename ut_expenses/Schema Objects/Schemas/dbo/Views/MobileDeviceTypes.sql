CREATE VIEW [dbo].[MobileDeviceTypes]
	AS SELECT mobileDeviceTypeID, model FROM [$(targetMetabase)].dbo.mobileDeviceTypes 