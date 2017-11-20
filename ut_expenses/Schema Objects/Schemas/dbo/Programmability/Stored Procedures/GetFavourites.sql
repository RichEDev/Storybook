CREATE PROCEDURE [dbo].[GetFavourites]
AS
SELECT f.FavouriteID, f.EmployeeID, f.AddressID, a.GlobalIdentifier, a.AddressName, a.Line1, a.Postcode, a.City
FROM favourites AS f
INNER JOIN addresses AS a ON f.AddressID = a.AddressID 
WHERE a.Archived = 0;