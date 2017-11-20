ALTER TABLE [dbo].[mileage_categories]
    ADD CONSTRAINT [DF_mileage_categories_thresholdtype] DEFAULT ((0)) FOR [thresholdtype];

