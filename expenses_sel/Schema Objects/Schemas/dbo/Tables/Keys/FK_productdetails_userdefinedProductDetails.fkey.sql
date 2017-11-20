ALTER TABLE [dbo].[userdefinedProductDetails]
    ADD CONSTRAINT [FK_productdetails_userdefinedProductDetails] FOREIGN KEY ([productid]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE CASCADE ON UPDATE NO ACTION;

