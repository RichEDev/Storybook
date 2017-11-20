﻿CREATE PROCEDURE [dbo].[APIgetAddresses]
	@AddressID INT
AS
BEGIN
	IF @AddressID = 0
		BEGIN
			SELECT [addresses].[AddressID]
				,[addresses].[Archived]
				,[addresses].[AddressName]
				,[addresses].[Line1]
				,[addresses].[Line2]
				,[addresses].[Line3]
				,[addresses].[City]
				,[addresses].[County]
				,[addresses].[Postcode]
				,[addresses].[Country]
				,[addresses].[Latitude]
				,[addresses].[Longitude]
				,[addresses].[CreationMethod]
				,[addresses].[GlobalIdentifier]
				,[addresses].[Udprn]
				,[addresses].[LookupDate]
				,[addresses].[SubAccountID]
				,[addresses].[AccountWideFavourite]
				,[AddressEsrAllocation].[ESRLocationID]
				,[AddressEsrAllocation].[ESRAddressID]
				,[addresses].[CreatedOn]
				,[addresses].[CreatedBy]
				,[addresses].[ModifiedOn]
				,[addresses].[ModifiedBy]
			FROM [dbo].[addresses]
			LEFT JOIN [AddressEsrAllocation] ON [AddressEsrAllocation].[AddressId] = [addresses].[AddressID]
		END
	ELSE
		BEGIN
			SELECT [addresses].[AddressID]
				,[addresses].[Archived]
				,[addresses].[AddressName]
				,[addresses].[Line1]
				,[addresses].[Line2]
				,[addresses].[Line3]
				,[addresses].[City]
				,[addresses].[County]
				,[addresses].[Postcode]
				,[addresses].[Country]
				,[addresses].[Latitude]
				,[addresses].[Longitude]
				,[addresses].[CreationMethod]
				,[addresses].[GlobalIdentifier]
				,[addresses].[Udprn]
				,[addresses].[LookupDate]
				,[addresses].[SubAccountID]
				,[addresses].[AccountWideFavourite]
				,[AddressEsrAllocation].[ESRLocationID]
				,[AddressEsrAllocation].[ESRAddressID]
				,[addresses].[CreatedOn]
				,[addresses].[CreatedBy]
				,[addresses].[ModifiedOn]
				,[addresses].[ModifiedBy]
			FROM [dbo].[addresses]
			LEFT JOIN [AddressEsrAllocation] ON [AddressEsrAllocation].[AddressId] = [addresses].[AddressID]
			WHERE [addresses].[AddressID] = @AddressID
		 END
END	
RETURN 0