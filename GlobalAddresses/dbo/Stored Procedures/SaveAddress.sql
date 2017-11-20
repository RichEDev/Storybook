
CREATE PROCEDURE [dbo].[SaveAddress]
	@Postcode NVARCHAR(32),
	@AddressName NVARCHAR(250),
	@Line1 NVARCHAR(256),
	@Line2 NVARCHAR(256),
	@Line3 NVARCHAR(256),
	@City NVARCHAR(256),
	@County NVARCHAR(256),
	@Country INT,
	@LookupDate DATETIME = NULL,
	@ProviderIdentifier NVARCHAR(50),
	@Udprn INT,
	@Longitude NVARCHAR(20),
	@Latitude NVARCHAR(20)
AS

DECLARE @AddressID INT;

IF (@ProviderIdentifier IS NULL 
	OR LTRIM(@ProviderIdentifier) = '' 
	OR EXISTS (SELECT * FROM addresses WHERE ProviderIdentifier = @ProviderIdentifier))
BEGIN
	RETURN -1;
END

	INSERT INTO [dbo].[addresses]
	(
		[Postcode],
		[AddressName],
		[Line1],
		[Line2],
		[Line3],
		[City],
		[County],
		[Country],
		[LookupDate],
		[ProviderIdentifier],
		[Udprn],
		[Longitude],
		[Latitude],
		[CreatedOn]
	)
	VALUES
	(
		@Postcode,
		@AddressName,
		@Line1,
		@Line2,
		@Line3,
		@City,
		@County,
		@Country,
		GETUTCDATE(),
		@ProviderIdentifier,
		@Udprn,
		@Longitude,
		@Latitude,
		GETUTCDATE()	
	)
	
	SET @AddressID = SCOPE_IDENTITY();

RETURN @AddressID;


