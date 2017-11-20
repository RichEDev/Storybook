IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo][ApiSearchAddresses]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ApiSearchAddresses]
GO

CREATE PROCEDURE [dbo].[ApiSearchAddresses]
@searchCriteria NVARCHAR(250),
@globalCountryId INT,
@displayEsrAddresses BIT = 0
AS
DECLARE @cleanSearchMin NVARCHAR(256) = dbo.CleanLookup(@searchCriteria);
DECLARE @cleanSearchMax NVARCHAR(256) = dbo.IncrementLookup(@cleanSearchMin, default);

DECLARE @nonAlphaLen int = PATINDEX('%[A-Za-z]%', @searchCriteria)-1;
IF @nonAlphaLen < 0 SELECT @nonAlphaLen = LEN(@searchCriteria) -- this is the case when it's all non-alpha numeric, so want outer filter to match whole string (index min and max will be '' and 'ZZZ')

DECLARE @nonAlpha NVARCHAR(256) = SUBSTRING(@searchCriteria, 1, @nonAlphaLen) + '%';

SELECT DISTINCT [AddressID], [Postcode], [AddressName], [Line1], [Line2], [Line3], [City], [County], [Country], 
      [Archived], [CreationMethod], [LookupDate], [SubAccountID], [GlobalIdentifier], 
      [Longitude], [Latitude], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy] 
FROM [dbo].[Addresses] 
WHERE
(
      (AddressNameLookup >= @cleanSearchMin AND AddressNameLookup < @cleanSearchMax)
      OR (Line1Lookup >= @cleanSearchMin AND Line1Lookup < @cleanSearchMax)
      OR (Line2Lookup >= @cleanSearchMin AND Line2Lookup < @cleanSearchMax)
      OR (CityLookup >= @cleanSearchMin AND CityLookup < @cleanSearchMax)
      OR (PostcodeLookup >= @cleanSearchMin AND PostcodeLookup < @cleanSearchMax)
)
AND (GlobalIdentifier = '' OR GlobalIdentifier IS NULL) 
AND (@displayEsrAddresses = 1 OR CreationMethod <> 3)
AND Archived = 0 
AND Obsolete = 0 
AND (Country = @globalCountryId OR Country = 0)
AND (AddressName LIKE @nonAlpha OR Line1 LIKE @nonAlpha OR Line2 LIKE @nonAlpha OR City LIKE @nonAlpha OR Postcode LIKE @nonAlpha);
