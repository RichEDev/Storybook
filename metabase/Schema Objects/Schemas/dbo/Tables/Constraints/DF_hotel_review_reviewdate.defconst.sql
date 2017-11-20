ALTER TABLE [dbo].[hotel_reviews]
    ADD CONSTRAINT [DF_hotel_review_reviewdate] DEFAULT (getdate()) FOR [reviewdate];

