ALTER TABLE [dbo].[supplier_categories]
    ADD CONSTRAINT [DF_supplier_categories_archived] DEFAULT ((0)) FOR [archived];

