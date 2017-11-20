ALTER TABLE [dbo].[invoiceProductDetails]
    ADD CONSTRAINT [FK_invoice_productdetails_product_details] FOREIGN KEY ([productID]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

