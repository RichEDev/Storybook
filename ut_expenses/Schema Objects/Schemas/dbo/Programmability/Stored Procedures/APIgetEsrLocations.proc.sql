CREATE PROCEDURE [dbo].[APIgetEsrLocations]
	@ESRLocationId bigint 
AS
BEGIN
IF @ESRLocationId = 0
	BEGIN
		SELECT [ESRLocationId]
		  ,[LocationCode]
		  ,[Description]
		  ,[InactiveDate]
		  ,[AddressLine1]
		  ,[AddressLine2]
		  ,[AddressLine3]
		  ,[Town]
		  ,[County]
		  ,[Postcode]
		  ,[Country]
		  ,[Telephone]
		  ,[Fax]
		  ,[PayslipDeliveryPoint]
		  ,[SiteCode]
		  ,[WelshLocationTranslation]
		  ,[WelshAddress1]
		  ,[WelshAddress2]
		  ,[WelshAddress3]
		  ,[WelshTownTranslation]
		  ,[ESRLastUpdate]
	  FROM [dbo].[ESRLocations]
	END
ELSE
	BEGIN
		SELECT [ESRLocationId]
		  ,[LocationCode]
		  ,[Description]
		  ,[InactiveDate]
		  ,[AddressLine1]
		  ,[AddressLine2]
		  ,[AddressLine3]
		  ,[Town]
		  ,[County]
		  ,[Postcode]
		  ,[Country]
		  ,[Telephone]
		  ,[Fax]
		  ,[PayslipDeliveryPoint]
		  ,[SiteCode]
		  ,[WelshLocationTranslation]
		  ,[WelshAddress1]
		  ,[WelshAddress2]
		  ,[WelshAddress3]
		  ,[WelshTownTranslation]
		  ,[ESRLastUpdate]
	  FROM [dbo].[ESRLocations]
	  WHERE @ESRLocationId = ESRLocationId
	END
END
