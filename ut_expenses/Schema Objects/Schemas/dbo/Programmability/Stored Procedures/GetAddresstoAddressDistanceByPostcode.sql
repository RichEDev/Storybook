CREATE PROCEDURE [dbo].[GetAddresstoAddressDistanceByPostcode]
	@OriginAddressLocator nvarchar(50) ,
	@DestinationAddressLocator nvarchar(50),
    @ReturnJourney BIT
AS
	IF @ReturnJourney = 1
	BEGIN
		SELECT TOP 1 [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], CAST(1 AS BIT) AS [Outbound]
		FROM [dbo].[addressDistances]
		INNER JOIN Addresses AS AddressesFrom ON AddressesFrom.PostcodeLookup = @OriginAddressLocator
		INNER JOIN Addresses AS AddressesTo	ON AddressesTo.PostcodeLookup = @DestinationAddressLocator
		WHERE [AddressIDA] in (AddressesFrom.AddressId) AND [AddressIDB] in (AddressesTo.AddressId)

		UNION ALL
 
		SELECT TOP 1 [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], CAST(0 AS BIT) AS [Outbound]
		FROM [dbo].[addressDistances]
		INNER JOIN Addresses AS AddressesFrom ON AddressesFrom.PostcodeLookup = @OriginAddressLocator
		INNER JOIN Addresses AS AddressesTo	ON AddressesTo.PostcodeLookup = @DestinationAddressLocator
		WHERE [AddressIDA] in (AddressesTo.AddressId) AND [AddressIDB] in (AddressesFrom.AddressId)
	END
	ELSE
	BEGIN
		SELECT TOP 1 [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], CAST(1 AS BIT) AS [Outbound]
		FROM [dbo].[addressDistances]
		INNER JOIN Addresses AS AddressesFrom ON AddressesFrom.PostcodeLookup = @OriginAddressLocator
		INNER JOIN Addresses AS AddressesTo	ON AddressesTo.PostcodeLookup = @DestinationAddressLocator
		WHERE [AddressIDA] in (AddressesFrom.AddressId) AND [AddressIDB] in (AddressesTo.AddressId)
	END
RETURN 0
