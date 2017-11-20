ALTER TABLE [dbo].[productLicences]
    ADD CONSTRAINT [FK_product_details_product_licences] FOREIGN KEY ([productID]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE CASCADE ON UPDATE NO ACTION;

