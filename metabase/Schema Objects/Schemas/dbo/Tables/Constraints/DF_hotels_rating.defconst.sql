ALTER TABLE [dbo].[hotels]
    ADD CONSTRAINT [DF_hotels_rating] DEFAULT ((0)) FOR [rating];

