ALTER TABLE [dbo].[contract_productdetails]
    ADD CONSTRAINT [FK_contract_productdetails_product_details] FOREIGN KEY ([productId]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

