ALTER TABLE [dbo].[hotel_reviews]
    ADD CONSTRAINT [FK_hotel_reviews_hotels] FOREIGN KEY ([hotelid]) REFERENCES [dbo].[hotels] ([hotelid]) ON DELETE CASCADE ON UPDATE CASCADE NOT FOR REPLICATION;

