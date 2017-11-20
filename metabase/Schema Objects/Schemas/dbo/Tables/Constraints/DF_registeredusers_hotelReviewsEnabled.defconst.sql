ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_hotelReviewsEnabled] DEFAULT ((1)) FOR [hotelReviewsEnabled];

