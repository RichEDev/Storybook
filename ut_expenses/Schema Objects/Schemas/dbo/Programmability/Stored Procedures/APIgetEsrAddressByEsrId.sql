
CREATE PROCEDURE [dbo].[APIgetEsrAddressByEsrId]
 @EsrId bigint 

AS
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
 WHERE ESRPersonId = @EsrId
END