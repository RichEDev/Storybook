CREATE PROCEDURE [dbo].[SearchAddresses]
@searchCriteria NVARCHAR(250),
@globalCountryId INT,
@displayEsrAddresses BIT = 0
AS
DECLARE @cleanSearchMin NVARCHAR(256) = dbo.CleanLookup(@searchCriteria);
DECLARE @cleanSearchMax NVARCHAR(256) = dbo.IncrementLookup(@cleanSearchMin, default);

DECLARE @nonAlphaLen int = PATINDEX('%[A-Za-z]%', @searchCriteria)-1;
IF @nonAlphaLen < 0 SELECT @nonAlphaLen = LEN(@searchCriteria) -- this is the case when it's all non-alpha numeric, so want outer filter to match whole string (index min and max will be '' and 'ZZZ')

DECLARE @nonAlpha NVARCHAR(256) = SUBSTRING(@searchCriteria, 1, @nonAlphaLen) + '%';

SELECT DISTINCT TOP 20 addresses.[AddressID], addresses.[Postcode], [AddressName], addresses.[Line1], [Line2], [Line3], addresses.[City], [County], [Country], 
      [Archived], [CreationMethod], [LookupDate], [SubAccountID], [GlobalIdentifier], 
      [Longitude], [Latitude], addresses.[CreatedOn], addresses.[CreatedBy], addresses.[ModifiedOn], addresses.[ModifiedBy], addresses.[AccountWideFavourite] 
FROM [dbo].[Addresses] 
left join employeeHomeAddresses on employeeHomeAddresses.AddressId = addresses.AddressID
WHERE
(
      (AddressNameLookup >= @cleanSearchMin AND AddressNameLookup < @cleanSearchMax)
      OR (Line1Lookup >= @cleanSearchMin AND Line1Lookup < @cleanSearchMax)
      OR (Line2Lookup >= @cleanSearchMin AND Line2Lookup < @cleanSearchMax)
      OR (CityLookup >= @cleanSearchMin AND CityLookup < @cleanSearchMax)
      OR (PostcodeLookup >= @cleanSearchMin AND PostcodeLookup < @cleanSearchMax)
)
AND (GlobalIdentifier = '' OR GlobalIdentifier IS NULL) 
AND ( (@displayEsrAddresses = 1 AND employeeHomeAddresses.AddressId is null ) OR CreationMethod <> 3)
AND Archived = 0 
AND Obsolete = 0 
AND (Country = @globalCountryId OR Country = 0)
AND (AddressName LIKE @nonAlpha OR addresses.Line1 LIKE @nonAlpha OR Line2 LIKE @nonAlpha OR addresses.City LIKE @nonAlpha OR addresses.Postcode LIKE @nonAlpha);

GO


