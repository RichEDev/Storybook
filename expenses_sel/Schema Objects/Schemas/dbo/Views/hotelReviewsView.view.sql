﻿
CREATE VIEW [dbo].[hotelReviewsView]
AS
SELECT     reviewid, hotelid, rating, review, displaytype, amountpaid, reviewdate, standardrooms, hotelfacilities, valuemoney, performancestaff, location
FROM         [$(metabaseExpenses)].dbo.hotel_reviews

