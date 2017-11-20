CREATE PROCEDURE [dbo].[GetRelatedAddressDistancesApi]
(
	@AddressID INT
)
AS
BEGIN
	SELECT [AddressDistanceID], [AddressIDA], [AddressIDB], [CustomDistance]
		FROM [dbo].[addressDistances]
		WHERE [AddressIDA] = @AddressID OR [AddressIDB] = @AddressID;
END