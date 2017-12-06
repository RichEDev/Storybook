CREATE PROCEDURE [dbo].[GetAddressToAddressDistance]--Used for International only.
(
 @OriginAddressID INT,
 @DestinationAddressID INT,
 @ReturnJourney BIT

)
AS
BEGIN

IF EXISTS (SELECT AddressDistanceId FROM AddressDistances WHERE ([AddressIDA] = @OriginAddressID AND [AddressIDB] = @DestinationAddressId) OR ([AddressIDA] = @DestinationAddressId AND [AddressIDB] = @OriginAddressID))
BEGIN
	IF @ReturnJourney = 1
	BEGIN
		SELECT [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], CAST(1 AS BIT) AS [Outbound]
		FROM [dbo].[addressDistances]
		WHERE [AddressIDA] = @OriginAddressID AND [AddressIDB] = @DestinationAddressID
 
		UNION ALL
 
		SELECT [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], CAST(0 AS BIT) AS [Outbound]
		FROM [dbo].[addressDistances]
		WHERE [AddressIDA] = @DestinationAddressID AND [AddressIDB] = @OriginAddressID 
	END
	ELSE
	BEGIN
		SELECT [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], CAST(1 AS BIT) AS [Outbound]
		FROM [dbo].[addressDistances]
		WHERE [AddressIDA] = @OriginAddressID AND [AddressIDB] = @DestinationAddressID
	END

END
  
END
GO


