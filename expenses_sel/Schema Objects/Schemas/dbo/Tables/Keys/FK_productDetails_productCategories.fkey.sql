ALTER TABLE [dbo].[productDetails]
    ADD CONSTRAINT [FK_productDetails_productCategories] FOREIGN KEY ([productCategoryId]) REFERENCES [dbo].[productCategories] ([categoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

