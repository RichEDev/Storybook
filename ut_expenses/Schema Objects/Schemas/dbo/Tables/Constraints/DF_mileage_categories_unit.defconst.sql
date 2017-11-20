ALTER TABLE [dbo].[mileage_categories]
    ADD CONSTRAINT [DF_mileage_categories_unit] DEFAULT ((0)) FOR [unit];

