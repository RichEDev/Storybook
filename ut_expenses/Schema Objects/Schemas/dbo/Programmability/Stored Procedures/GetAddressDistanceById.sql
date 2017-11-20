CREATE PROCEDURE [dbo].[GetAddressDistanceById]
	@AddressDistanceID INT
AS SELECT * FROM addressDistances WHERE AddressDistanceID = @AddressDistanceID
GO
