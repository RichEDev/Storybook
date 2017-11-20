CREATE PROCEDURE [dbo].[GetRelatedAddressDistances]
(
	@AddressID INT
)
AS
BEGIN
	SELECT ad.[AddressDistanceID]
		,ad.[AddressIDB] as AddressID
		,ad.[CustomDistance]
		,ad.[PostcodeAnywhereFastestDistance]
		,ad.[PostcodeAnywhereShortestDistance]
		,CAST(1 AS BIT) AS [Outbound]
		,a.[AddressName]
		,a.[Line1]
		,a.[City]
		,a.[Postcode]
		FROM [dbo].[addressDistances] AS ad
			LEFT JOIN [dbo].[addresses] AS a ON a.AddressID = ad.[AddressIDB]
		WHERE [AddressIDA] = @AddressID

	UNION ALL

	SELECT ad.[AddressDistanceID]
		,ad.[AddressIDA] as AddressID
		,ad.[CustomDistance]
		,ad.[PostcodeAnywhereFastestDistance]
		,ad.[PostcodeAnywhereShortestDistance]
		,CAST(0 AS BIT) AS [Outbound]
		,a.[AddressName]
		,a.[Line1]
		,a.[City]
		,a.[Postcode]
		FROM [dbo].[addressDistances] AS ad
			LEFT JOIN [dbo].[addresses] AS a ON a.AddressID = ad.[AddressIDA]
		WHERE [AddressIDB] = @AddressID;
END