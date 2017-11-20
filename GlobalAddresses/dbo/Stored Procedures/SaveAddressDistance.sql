CREATE PROCEDURE [dbo].[SaveAddressDistance]
	@Origin NVARCHAR(15),
	@Destination NVARCHAR(15),
	@FastestDistanceWithPostcodeCentres DECIMAL(18,2) = NULL,
	@ShortestDistanceWithPostcodeCentres DECIMAL(18,2) = NULL,
	@FastestDistanceWithRoads DECIMAL(18,2) = NULL,
	@ShortestDistanceWithRoads DECIMAL(18,2) = NULL
AS
BEGIN
	DECLARE @AddressLocatorDistanceID INT = 0;
	SELECT TOP 1 @AddressLocatorDistanceID = [AddressLocatorDistanceID] FROM [dbo].[addressLocatorDistances] WHERE [OriginLocator] = @Origin AND [DestinationLocator] = @Destination;
	
	IF @AddressLocatorDistanceID = 0
	BEGIN
		INSERT INTO [dbo].[AddressLocatorDistances] 
		(
			[OriginLocator]
			,[DestinationLocator]
			,[FastestDistanceWithPostcodeCentres]
			,[ShortestDistanceWithPostcodeCentres]
			,[FastestDistanceWithRoads]
			,[ShortestDistanceWithRoads]
			,[LookupDate]
		)
		VALUES 
		(
			@Origin
			,@Destination
			,@FastestDistanceWithPostcodeCentres
			,@ShortestDistanceWithPostcodeCentres
			,@FastestDistanceWithRoads
			,@ShortestDistanceWithRoads
			,GETDATE()
		);
	
		SET @AddressLocatorDistanceID = SCOPE_IDENTITY();
		
	END
	ELSE --Update existing entry
	BEGIN
		UPDATE [dbo].[AddressLocatorDistances]   
			SET 
			[FastestDistanceWithPostcodeCentres] = COALESCE(@FastestDistanceWithPostcodeCentres, FastestDistanceWithPostcodeCentres) 
			,[ShortestDistanceWithPostcodeCentres] = COALESCE(@ShortestDistanceWithPostcodeCentres, ShortestDistanceWithPostcodeCentres)
			,[FastestDistanceWithRoads] = COALESCE(@FastestDistanceWithRoads, FastestDistanceWithRoads)
			,[ShortestDistanceWithRoads] = COALESCE(@ShortestDistanceWithRoads, ShortestDistanceWithRoads)
			WHERE [AddressLocatorDistanceID] = @AddressLocatorDistanceID;

	END
	
	RETURN @AddressLocatorDistanceID;
END
GO


