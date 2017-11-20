CREATE PROCEDURE [dbo].[GetAddressById]
	@AddressID INT
AS SELECT * FROM addresses WHERE AddressID = @AddressID
