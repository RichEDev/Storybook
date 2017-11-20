CREATE PROCEDURE [dbo].[GetAddressFromFailedDuplicateCheck]
	@AddressID INT,
	@AddressName NVARCHAR(250),
	@Postcode NVARCHAR(32),
	@Line1 NVARCHAR(256),
	@City NVARCHAR(256),
	@Country INT,
	@CreationMethod INT
AS

IF (@Postcode IS NOT NULL AND LTRIM(@Postcode) <> '')
BEGIN
	-- POSTCODE PROVIDED
	IF @CreationMethod = 1
	BEGIN
		SELECT TOP 1 [a].AddressID, [a].Postcode, [a].AddressName, [a].Line1, [a].Line2, [a].Line3, [a].City, [a].County, [a].Country, [a].Archived, [a].CreationMethod, [a].LookupDate, [a].SubAccountID, [a].GlobalIdentifier, [a].Longitude, [a].Latitude, [a].CreatedOn, [a].CreatedBy, [a].ModifiedOn, [a].ModifiedBy, [a].AccountWideFavourite, [a].Line1Lookup, [a].Line2Lookup, [a].CityLookup, [a].PostcodeLookup, [a].Udprn, gc.[Country] [CountryName], gc.[Alpha3CountryCode]
				FROM [dbo].[addresses] [a]
				LEFT JOIN [global_countries] [gc] ON [a].Country = [gc].[globalcountryid]
				WHERE [a].[AddressName] = @AddressName AND [a].[Line1] = @Line1 AND [a].[Postcode] = @Postcode AND [a].[Country] = @Country AND [a].[AddressID] <> @AddressID AND [a].[CreationMethod] = 1;
	END
	ELSE
	BEGIN
		SELECT TOP 1 [a].AddressID, [a].Postcode, [a].AddressName, [a].Line1, [a].Line2, [a].Line3, [a].City, [a].County, [a].Country, [a].Archived, [a].CreationMethod, [a].LookupDate, [a].SubAccountID, [a].GlobalIdentifier, [a].Longitude, [a].Latitude, [a].CreatedOn, [a].CreatedBy, [a].ModifiedOn, [a].ModifiedBy, [a].AccountWideFavourite, [a].Line1Lookup, [a].Line2Lookup, [a].CityLookup, [a].PostcodeLookup, [a].Udprn, gc.[Country] [CountryName], gc.[Alpha3CountryCode]
				FROM [dbo].[addresses] [a]
				LEFT JOIN [global_countries] [gc] ON [a].Country = [gc].[globalcountryid]
				WHERE [a].[AddressName] = @AddressName AND [a].[Line1] = @Line1 AND [a].[Postcode] = @Postcode AND [a].[Country] = @Country AND [a].[AddressID] <> @AddressID;
	END
END
ELSE
BEGIN
	-- POSTCODE NOT PROVIDED
	IF @CreationMethod = 1
	BEGIN
		SELECT TOP 1 [a].AddressID, [a].Postcode, [a].AddressName, [a].Line1, [a].Line2, [a].Line3, [a].City, [a].County, [a].Country, [a].Archived, [a].CreationMethod, [a].LookupDate, [a].SubAccountID, [a].GlobalIdentifier, [a].Longitude, [a].Latitude, [a].CreatedOn, [a].CreatedBy, [a].ModifiedOn, [a].ModifiedBy, [a].AccountWideFavourite, [a].Line1Lookup, [a].Line2Lookup, [a].CityLookup, [a].PostcodeLookup, [a].Udprn, gc.[Country] [CountryName], gc.[Alpha3CountryCode]
				FROM [dbo].[addresses] [a]
				LEFT JOIN [global_countries] [gc] ON [a].Country = [gc].[globalcountryid]
				WHERE [a].[AddressName] = @AddressName AND [a].[Line1] = @Line1 AND [a].[City] = @City AND [a].[Country] = @Country AND [a].[AddressID] <> @AddressID AND [a].[CreationMethod] = 1
	END
	ELSE 
	BEGIN
		SELECT TOP 1 [a].AddressID, [a].Postcode, [a].AddressName, [a].Line1, [a].Line2, [a].Line3, [a].City, [a].County, [a].Country, [a].Archived, [a].CreationMethod, [a].LookupDate, [a].SubAccountID, [a].GlobalIdentifier, [a].Longitude, [a].Latitude, [a].CreatedOn, [a].CreatedBy, [a].ModifiedOn, [a].ModifiedBy, [a].AccountWideFavourite, [a].Line1Lookup, [a].Line2Lookup, [a].CityLookup, [a].PostcodeLookup, [a].Udprn, gc.[Country] [CountryName], gc.[Alpha3CountryCode]
				FROM [dbo].[addresses] [a]
				LEFT JOIN [global_countries] [gc] ON [a].Country = [gc].[globalcountryid]
				WHERE [a].[AddressName] = @AddressName AND [a].[Line1] = @Line1 AND [a].[City] = @City AND [a].[Country] = @Country AND [a].[AddressID] <> @AddressID;
	END
END