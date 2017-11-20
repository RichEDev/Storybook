CREATE PROCEDURE [dbo].[GetHotelsByName] @hotelName NVARCHAR(50)
AS
SELECT top 10 hotelid
	,hotelname
	,address1
	,address2
	,city
	,county
	,postcode
	,country
	,rating
	,telno
	,email
	,CreatedOn
	,CreatedBy
FROM hotels
WHERE hotelname LIKE @hotelName + '%'