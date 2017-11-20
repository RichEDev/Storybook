ALTER TABLE [dbo].[contract_productdetails]
    ADD CONSTRAINT [DF_Contract_ProductDetails_PricePaid] DEFAULT ((0)) FOR [pricePaid];

