CREATE PROCEDURE [dbo].[ApiBatchSaveAddress] @list ApiBatchSaveAddressType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @AddressID INT
	DECLARE @Archived BIT
	DECLARE @AddressName NVARCHAR(250)
	DECLARE @Line1 NVARCHAR(256)
	DECLARE @Line2 NVARCHAR(256)
	DECLARE @Line3 NVARCHAR(256)
	DECLARE @City NVARCHAR(256)
	DECLARE @County NVARCHAR(256)
	DECLARE @Postcode NVARCHAR(32)
	DECLARE @Country INT
	DECLARE @GlobalIdentifier NVARCHAR(50)
	DECLARE @CreationMethod INT
	DECLARE @Udprn INT
	DECLARE @AccountWideFavourite BIT
	DECLARE @LookupDate DATETIME
	DECLARE @SubAccountID INT
	DECLARE @ESRLocationID BIGINT
	DECLARE @ESRAddressID BIGINT
	DECLARE @Latitude NVARCHAR(20)
	DECLARE @Longitude NVARCHAR(20)
	DECLARE @CreatedOn DATETIME
	DECLARE @CreatedBy INT
	DECLARE @ModifiedOn DATETIME
	DECLARE @ModifiedBy INT
	DECLARE @EsrAddressType INT
	DECLARE @tempTable TABLE (
	   EsrId BIGINT,
	   EsrAddressType int
	)
	
	DECLARE @loopTable TABLE (
		tmpId BIGINT
		,EsrId BIGINT
		,EsrAddressType int
		)
	
	DECLARE @zeroAddressIds TABLE (
		EsrId BIGINT
		,EsrAddressType int  
		,AddressId int
		)

     INSERT @tempTable
	   SELECT EsrAddressId, 0 FROM @list WHERE ESRAddressID IS NOT NULL
     INSERT @tempTable
	   SELECT EsrLocationId, 1 FROM @list WHERE ESRLocationID IS NOT NULL AND ESRAddressID IS NULL
	       
	INSERT @loopTable
	SELECT ROW_NUMBER() OVER (
			ORDER BY EsrId
			)
		,EsrId
		,EsrAddressType
	FROM @tempTable
	
	SELECT @count = count(*)
	FROM @loopTable

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @ESRAddressID = (
				SELECT TOP 1 EsrId
				FROM @loopTable
				WHERE tmpId = @index
				AND EsrAddressType = 0
				);
		SET @ESRLocationID = (
				SELECT TOP 1 EsrId
				FROM @loopTable
				WHERE tmpId = @index
				AND EsrAddressType = 1		
		);
		SET @EsrAddressType = (SELECT EsrAddressType FROM @loopTable WHERE tmpId = @index)
		
		SELECT TOP 1 @AddressID = AddressID
			,@Archived = Archived
			,@AddressName = AddressName
			,@Line1 = Line1
			,@Line2 = Line2
			,@Line3 = Line3
			,@City = City
			,@County = County
			,@Postcode = Postcode
			,@Country = Country
			,@GlobalIdentifier = GlobalIdentifier
			,@CreationMethod = CreationMethod
			,@Udprn = Udprn
			,@AccountWideFavourite = AccountWideFavourite
			,@LookupDate = LookupDate
			,@SubAccountID = SubAccountID
			,@ESRLocationID = ESRLocationID
			,@ESRAddressID = ESRAddressID
			,@Latitude = Latitude
			,@Longitude = Longitude
			,@CreatedOn = CreatedOn
			,@CreatedBy = CreatedBy
			,@ModifiedOn = ModifiedOn
			,@ModifiedBy = ModifiedBy
		FROM @list
		WHERE ESRAddressID = @ESRAddressID
		OR ESRLocationID = @ESRLocationID

		SET @AddressName = COALESCE(@AddressName, '');

	  
		IF @AddressID = 0
			AND EXISTS (
				SELECT [AddressID]
				FROM [dbo].[addresses]
				WHERE AddressName = @AddressName
					AND Line1 = @Line1
					AND Postcode = @Postcode
					AND CreationMethod = @CreationMethod
				)
		BEGIN
			SELECT TOP 1 @AddressID = [AddressID]
			FROM [dbo].[addresses]
			WHERE AddressName = @AddressName
				AND Line1 = @Line1
				AND Postcode = @Postcode
				AND CreationMethod = @CreationMethod;
		END

		IF @AddressID = 0
		BEGIN
			INSERT INTO [dbo].[addresses] (
				[Archived]
				,[AddressName]
				,[Line1]
				,[Line2]
				,[Line3]
				,[City]
				,[County]
				,[Postcode]
				,[Country]
				,[GlobalIdentifier]
				,[CreationMethod]
				,[Udprn]
				,[AccountWideFavourite]
				,[LookupDate]
				,[SubAccountID]
				,[Latitude]
				,[Longitude]
				,[CreatedOn]
				,[CreatedBy]
				,[AddressNameLookup]
				,[Line1Lookup]
				,[Line2Lookup]
				,[CityLookup]
				,[PostcodeLookup]
				)
			VALUES (
				@Archived
				,@AddressName
				,@Line1
				,@Line2
				,@Line3
				,@City
				,@County
				,@Postcode
				,@Country
				,@GlobalIdentifier
				,@CreationMethod
				,@Udprn
				,@AccountWideFavourite
				,@LookupDate
				,@SubAccountID
				,@Latitude
				,@Longitude
				,@CreatedOn
				,@CreatedBy
				,dbo.CleanLookup(@AddressName)
				,dbo.CleanLookup(@Line1)
				,dbo.CleanLookup(@Line2)
				,dbo.CleanLookup(@City)
				,dbo.CleanLookup(@Postcode)
				)

			SET @AddressID = SCOPE_IDENTITY();
			
			INSERT @zeroAddressIds
				SELECT ISNULL(@ESRAddressID, @ESRLocationID), @EsrAddressType, @AddressID
		END
		ELSE
		BEGIN
			UPDATE [dbo].[addresses]
			SET [Archived] = @Archived
				,[AddressName] = @AddressName
				,[Line1] = @Line1
				,[Line2] = @Line2
				,[Line3] = @Line3
				,[City] = @City
				,[County] = @County
				,[Postcode] = @Postcode
				,[Country] = @Country
				,[GlobalIdentifier] = @GlobalIdentifier
				,[CreationMethod] = @CreationMethod
				,[Udprn] = @Udprn
				,[AccountWideFavourite] = @AccountWideFavourite
				,[LookupDate] = @LookupDate
				,[SubAccountID] = @SubAccountID
				,[Latitude] = @Latitude
				,[Longitude] = @Longitude
				,[ModifiedOn] = @ModifiedOn
				,[ModifiedBy] = @ModifiedBy
				,[AddressNameLookup] = dbo.CleanLookup(@AddressName)
				,[Line1Lookup] = dbo.CleanLookup(@Line1)
				,[Line2Lookup] = dbo.CleanLookup(@Line2)
				,[CityLookup] = dbo.CleanLookup(@City)
				,[PostcodeLookup] = dbo.CleanLookup(@Postcode)
			WHERE [AddressID] = @AddressID
			
			INSERT @zeroAddressIds
				SELECT ISNULL(@ESRAddressID, @ESRLocationID), @EsrAddressType, @AddressID
		END

		IF NOT EXISTS (SELECT * FROM AddressEsrAllocation WHERE [AddressId] = @AddressID AND ISNULL(ESRLocationID, 0) = ISNULL(@ESRLocationID, 0) AND ISNULL(ESRAddressID, 0) = ISNULL(@ESRAddressID, 0))
		BEGIN
			INSERT INTO AddressEsrAllocation ([AddressId], ESRLocationID, ESRAddressID) VALUES (@AddressID, @ESRLocationId, @ESRAddressId)
		END
		ELSE
		BEGIN
			UPDATE AddressEsrAllocation SET ESRLocationID = @ESRLocationId, @ESRAddressId = ESRAddressID WHERE [AddressId] = @AddressID
		END

		SET @index = @index + 1
	END

     SELECT * FROM @zeroAddressIds;
     
	RETURN 0;
END