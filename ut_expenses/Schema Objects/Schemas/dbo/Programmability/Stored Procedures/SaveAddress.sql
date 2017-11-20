CREATE PROCEDURE [dbo].[SaveAddress]
 @AddressID INT,
 @Postcode NVARCHAR(32),
 @AddressName NVARCHAR(250),
 @Line1 NVARCHAR(256),
 @Line2 NVARCHAR(256),
 @Line3 NVARCHAR(256),
 @City NVARCHAR(256),
 @County NVARCHAR(256),
 @Country INT,
 @CreationMethod INT,
 @LookupDate DATETIME = NULL,
 @SubAccountID INT,
 @GlobalIdentifier NVARCHAR(50),
 @Udprn INT,
 @Longitude NVARCHAR(20),
 @Latitude NVARCHAR(20),
 @AccountWideFavourite BIT,
 @UserID INT, 
 @DelegateID INT
AS

DECLARE @currentUser INT;
DECLARE @recordTitle NVARCHAR(2000) = COALESCE(@Line1, '') + ', ' + COALESCE(@City, '') + ', ' + COALESCE(@Postcode, '');
SET @currentUser = COALESCE(@DelegateID, @UserID);

IF (@Postcode IS NOT NULL AND LTRIM(@Postcode) <> '')
BEGIN
 -- POSTCODE PROVIDED
 
 -- Capture+ can be added even if another source has already added the same information, so filter duplicates to Capture+ addresses only
 -- or if it's not a capture+ address, search for any duplicate
 IF (@CreationMethod = 1  AND EXISTS (SELECT * FROM [dbo].[addresses] WHERE [Obsolete] = 0 AND [AddressName] = @AddressName AND [Line1] = @Line1 AND [Postcode] = @Postcode AND [Country] = @Country AND [AddressID] <> @AddressID AND [CreationMethod] = 1))
    OR
    (@CreationMethod = 3  AND EXISTS (SELECT * FROM [dbo].[addresses] WHERE [Obsolete] = 0 AND [Line1] = @Line1 AND [Postcode] = @Postcode AND [Country] = @Country AND [AddressID] <> @AddressID AND [CreationMethod] = 3))
    OR
    (@CreationMethod <> 1 AND @CreationMethod <> 3 AND EXISTS (SELECT * FROM [dbo].[addresses] WHERE [Obsolete] = 0 AND [AddressName] = @AddressName AND [Line1] = @Line1 AND [Postcode] = @Postcode AND [Country] = @Country AND [AddressID] <> @AddressID))
 BEGIN
  RETURN -1;
 END
END
ELSE
BEGIN
 -- POSTCODE NOT PROVIDED

 -- Capture+ can be added even if another source has already added the same information, so filter duplicates to Capture+ addresses only
 -- or if it's not a capture+ address, search for any duplicate
 IF (@CreationMethod = 1  AND EXISTS (SELECT * FROM [dbo].[addresses] WHERE [Obsolete] = 0 AND [AddressName] = @AddressName AND [Line1] = @Line1 AND [City] = @City AND [Country] = @Country AND [AddressID] <> @AddressID AND [CreationMethod] = 1))
    OR 
    (@CreationMethod <> 1 AND EXISTS (SELECT * FROM [dbo].[addresses] WHERE [Obsolete] = 0 AND [AddressName] = @AddressName AND [Line1] = @Line1 AND [City] = @City AND [Country] = @Country AND [AddressID] <> @AddressID)) 
 BEGIN
  RETURN -1;
 END
END

IF (@AddressID = 0)
BEGIN -- Insert New Record
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
  [Archived],
  [CreationMethod],
  [LookupDate],
  [SubAccountID],
  [GlobalIdentifier],
  [Udprn],
  [Longitude],
  [Latitude],
  [AccountWideFavourite],
  [CreatedOn],
  [CreatedBy],
  [AddressNameLookup],
  [Line1Lookup],
        [Line2Lookup],
        [CityLookup],
        [PostcodeLookup]
      
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
  0,
  @CreationMethod,
  GETUTCDATE(),
  @SubAccountID,
  @GlobalIdentifier,
  @Udprn,
  @Longitude,
  @Latitude,
  @AccountWideFavourite,
  GETUTCDATE(),
  @currentUser,
  dbo.CleanLookup(@AddressName),
  dbo.CleanLookup(@Line1),
        dbo.CleanLookup(@Line2),
        dbo.CleanLookup(@City),
        dbo.CleanLookup(@Postcode)
 )
 
 SET @AddressID = SCOPE_IDENTITY();
 
 EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 179, @AddressID, @recordTitle, @SubAccountID;
END

ELSE --Update Existing Record
BEGIN
 -- Create a backup of existing data
 DECLARE @oldPostcode NVARCHAR(32);
 DECLARE @oldAddressName NVARCHAR(250);
 DECLARE @oldLine1 NVARCHAR(256);
 DECLARE @oldLine2 NVARCHAR(256);
 DECLARE @oldLine3 NVARCHAR(256);
 DECLARE @oldCity NVARCHAR(256);
 DECLARE @oldCounty NVARCHAR(256);
 DECLARE @oldCountry NVARCHAR(256);
 DECLARE @oldCreationMethod INT;
 DECLARE @oldLookupDate DATETIME;
 DECLARE @oldSubAccountID INT;
 DECLARE @oldGlobalIdentifier NVARCHAR(50);
 DECLARE @oldUdprn INT;
 DECLARE @oldLongitude NVARCHAR(20);
 DECLARE @oldLatitude NVARCHAR(20);
 DECLARE @oldAccountWideFavourite bit;
 
 -- Create a backup of existing data
 SELECT @oldPostcode = [Postcode],
   @oldAddressName = [AddressName],
   @oldLine1 = [Line1],
   @oldLine2 = [Line2],
   @oldLine3 = [Line3],
   @oldCity = [City],
   @oldCounty = [County],
   @oldCountry = [Country],
   @oldCreationMethod = [CreationMethod],
   @oldLookupDate = [LookupDate],
   @oldSubAccountID = [SubAccountID],
   @oldGlobalIdentifier = [GlobalIdentifier],
   @oldUdprn = [Udprn],
   @oldLongitude = [Longitude],
   @oldLatitude = [Latitude],
   @oldAccountWideFavourite = [AccountWideFavourite]
 FROM [addresses]
 WHERE [AddressID] = @AddressID;
 
 -- Perform the update
 UPDATE [dbo].[addresses]
 SET  [Postcode] = @Postcode,
   [AddressName] = @AddressName,
   [Line1] = @Line1,
   [Line2] = @Line2,
   [Line3] = @Line3,
   [City] = @City,
   [County] = @County,
   [Country] = @Country,
   [CreationMethod] = @CreationMethod,
   [LookupDate] = COALESCE(@LookupDate, [LookupDate]),
   [SubAccountID] = @SubAccountID,
   [GlobalIdentifier] = @GlobalIdentifier,
   [Udprn] = @Udprn,
   [Longitude] = @Longitude,
   [Latitude] = @Latitude,
   [AccountWideFavourite] = @AccountWideFavourite,
   [ModifiedOn] = GETUTCDATE(),
   [ModifiedBy] = @currentUser,
   [AddressNameLookup] = dbo.CleanLookup(@AddressName),
   [Line1Lookup] = dbo.CleanLookup(@Line1),
            [Line2Lookup] = dbo.CleanLookup(@Line2),
            [CityLookup] = dbo.CleanLookup(@City),
            [PostcodeLookup] = dbo.CleanLookup(@Postcode)

 WHERE [AddressID] = @AddressID;
 
 SELECT @LookupDate = [LookupDate] FROM [addresses] WHERE [AddressID] = @AddressID;

 -- Update the audit log
  
 IF (@oldPostcode <> @Postcode)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '5a999c9f-440e-4d65-8b3c-187ba69e0234', @oldPostcode, @Postcode, @recordTitle, @SubAccountID;
 
 IF (@oldAddressName <> @AddressName)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '85DB77A3-6BC0-494F-8085-C35E96B042BE', @oldAddressName, @AddressName, @recordTitle, @SubAccountID;

 IF (@oldLine1 <> @Line1)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '25597581-f3c5-40f0-b7bc-53fb67ac1a0e', @oldLine1, @Line1, @recordTitle, @SubAccountID;
  
 IF (@oldLine2 <> @Line2)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '2fd99466-554c-43db-93de-878b17b0421f', @oldLine2, @Line2, @recordTitle, @SubAccountID;
  
 IF (@oldLine3 <> @Line3)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, 'e38eb767-8a40-4da1-bf1e-0f8f65c3a146', @oldLine3, @Line3, @recordTitle, @SubAccountID;
 
 IF (@oldCity <> @City)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, 'b90920fe-bea1-4948-a26b-cb997136c9f4', @oldCity, @City, @recordTitle, @SubAccountID;
 
 IF (@oldCounty <> @County)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '4b2d3322-4651-4c22-9fb9-a5c806038d20', @oldCounty, @County, @recordTitle, @SubAccountID;
 
 IF (@oldCountry <> @Country)
 BEGIN
  declare @countryname nvarchar(100)
 declare @oldcountryname nvarchar(100)
 select @countryname = country from global_countries where globalcountryid = @Country
 select @oldcountryname = country from global_countries where globalcountryid = @oldCountry
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '52524292-4a74-4405-94ad-f8f680984ad7', @oldcountryname, @countryname, @recordTitle, @SubAccountID;
 END
 IF (@oldCreationMethod <> @CreationMethod)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '459a25ff-fe05-4c43-9809-4272bfa9bd6e', @oldCreationMethod, @CreationMethod, @recordTitle, @SubAccountID;
  
 IF (@oldLookupDate <> @LookupDate)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, 'da1be4ad-ddd4-41e5-b3b0-ad563bed15a8', @oldLookupDate, @LookupDate, @recordTitle, @SubAccountID;
  
 IF (@oldSubAccountID <> @SubAccountID)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, 'b4b12641-4016-4769-b1b6-35bd364198ba', @oldSubAccountID, @SubAccountID, @recordTitle, @SubAccountID;
  
 IF (@oldGlobalIdentifier <> @GlobalIdentifier)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '67aa1884-bf83-47c0-b345-227c127e5f88', @oldGlobalIdentifier, @GlobalIdentifier, @recordTitle, @SubAccountID;
   
    IF (@oldUdprn <> @Udprn)
     EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '4FCF85FA-9BF5-466C-993C-D916CEE4781C', @oldUdprn, @Udprn, @recordTitle, @SubAccountID;

 IF (@oldLongitude <> @Longitude)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '5efb14e8-eb9d-4096-aceb-0ef99d48e545', @oldLongitude, @Longitude, @recordTitle, @SubAccountID;
  
 IF (@oldLatitude <> @Latitude)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '88b0e48c-cb5b-4ab2-b138-e7dcf90a3ef3', @oldLatitude, @Latitude, @recordTitle, @SubAccountID;

 IF (@oldAccountWideFavourite <> @AccountWideFavourite)
  EXEC addUpdateEntryToAuditLog @userid, @delegateID, 179, @AddressID, '489CE238-7975-452E-A93C-7BBDF371726C', @oldAccountWideFavourite, @AccountWideFavourite, @recordTitle, @SubAccountID;
END
RETURN @AddressID;