
CREATE VIEW [dbo].[hotelsView]
AS
SELECT     hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy
FROM         [$(metabaseExpenses)].dbo.hotels

