ALTER TABLE [dbo].[contract_forecastproducts]
    ADD CONSTRAINT [FK_contract_forecastproducts_product_details] FOREIGN KEY ([productId]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

