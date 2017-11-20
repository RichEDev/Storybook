ALTER TABLE [dbo].[mileage_categories]
    ADD CONSTRAINT [DF_mileage_categories_catvalid] DEFAULT ((0)) FOR [catvalid];

