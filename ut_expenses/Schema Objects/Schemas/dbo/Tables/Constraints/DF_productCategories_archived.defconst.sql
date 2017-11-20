ALTER TABLE [dbo].[productCategories]
    ADD CONSTRAINT [DF_productCategories_archived] DEFAULT ((0)) FOR [archived];

