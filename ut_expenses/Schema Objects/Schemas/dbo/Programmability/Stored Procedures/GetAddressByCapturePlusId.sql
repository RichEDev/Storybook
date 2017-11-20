CREATE PROCEDURE [dbo].[GetAddressByCapturePlusId]
@CapturePlusId NVARCHAR(40)

AS

SELECT company, companyid AS AddressId, address1, address2, city, county, postcode, country, CapturePlusId, createdBy, isPrivateAddress AS IsPrivate
FROM companies 
WHERE CapturePlusId = @CapturePlusId;