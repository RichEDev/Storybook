ALTER TABLE [dbo].[invoiceLineItems]
    ADD CONSTRAINT [FK_invoiceLineItems_product_details] FOREIGN KEY ([productID]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE NO ACTION ON UPDATE CASCADE;

