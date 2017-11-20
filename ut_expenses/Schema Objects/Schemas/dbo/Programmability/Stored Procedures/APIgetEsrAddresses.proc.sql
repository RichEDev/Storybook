CREATE PROCEDURE [dbo].[APIgetEsrAddresses]
	@ESRAddressId bigint 

AS
BEGIN
	IF @ESRAddressId = 0
	BEGIN
		SELECT [ESRAddressId]
		  ,[ESRPersonId]
		  ,[AddressType]
		  ,[AddressStyle]
		  ,[PrimaryFlag]
		  ,[AddressLine1]
		  ,[AddressLine2]
		  ,[AddressLine3]
		  ,[AddressTown]
		  ,[AddressCounty]
		  ,[AddressPostcode]
		  ,[AddressCountry]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[ESRLastUpdate]
	  FROM [dbo].[ESRAddresses]
	END
ELSE
	BEGIN
		SELECT [ESRAddressId]
		  ,[ESRPersonId]
		  ,[AddressType]
		  ,[AddressStyle]
		  ,[PrimaryFlag]
		  ,[AddressLine1]
		  ,[AddressLine2]
		  ,[AddressLine3]
		  ,[AddressTown]
		  ,[AddressCounty]
		  ,[AddressPostcode]
		  ,[AddressCountry]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[ESRLastUpdate]
	  FROM [dbo].[ESRAddresses]
	  WHERE ESRAddressId = @ESRAddressId
	END
END