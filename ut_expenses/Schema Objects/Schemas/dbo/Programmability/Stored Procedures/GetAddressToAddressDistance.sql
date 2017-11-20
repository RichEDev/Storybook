CREATE PROCEDURE [dbo].[GetAddressToAddressDistance]
(
 @InternationalLookup BIT,
 @OriginAddressID INT,
 @DestinationAddressID INT
)
AS
BEGIN


IF (@InternationalLookup = 0)
BEGIN

	DECLARE @OriginAddressLocator NVARCHAR(15);
	DECLARE @DestinationAddressLocator NVARCHAR(15);
	SET @OriginAddressLocator = (SELECT PostcodeLookup FROM Addresses WHERE AddressId = @OriginAddressId);
	SET @DestinationAddressLocator = (SELECT PostcodeLookup FROM Addresses WHERE AddressId = @DestinationAddressId);

	SELECT TOP 1 [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], case when AddressIDA = AddressesFrom.AddressID and AddressIDB = AddressesTo.AddressID then CAST(1 AS BIT) else cast(0 as bit) end AS [Outbound]
	FROM [dbo].[addressDistances]
	INNER JOIN Addresses AS AddressesFrom ON AddressesFrom.PostcodeLookup = @OriginAddressLocator
	INNER JOIN Addresses AS AddressesTo	ON AddressesTo.PostcodeLookup = @DestinationAddressLocator
	WHERE ([AddressIDA] = AddressesFrom.AddressId AND [AddressIDB] = AddressesTo.AddressId) or ([AddressIDA] in (AddressesTo.AddressId) AND [AddressIDB] in (AddressesFrom.AddressId))


END
else
begin
	SELECT [AddressDistanceID], [CustomDistance], [PostcodeAnywhereFastestDistance], [PostcodeAnywhereShortestDistance], case when addressida = @OriginAddressID and addressidb = @DestinationAddressID then cast(1 as bit) else cast(0 as bit) end as Outbound
	FROM [dbo].[addressDistances]
	WHERE ([AddressIDA] = @OriginAddressID AND [AddressIDB] = @DestinationAddressID) or (AddressIDA = @DestinationAddressID and AddressIDB = @OriginAddressID)
end

  
END
