CREATE PROCEDURE [dbo].[GetRelatedAddressDistanceIds]
(
	@AddressID INT
)
AS
BEGIN
	SELECT [AddressDistanceID] ,[CustomDistance]
		FROM [dbo].[addressDistances]
		WHERE [AddressIDA] = @AddressID OR [AddressIDB] = @AddressID;
END