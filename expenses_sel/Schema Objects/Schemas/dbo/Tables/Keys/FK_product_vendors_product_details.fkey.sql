ALTER TABLE [dbo].[product_suppliers]
    ADD CONSTRAINT [FK_product_vendors_product_details] FOREIGN KEY ([productId]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

