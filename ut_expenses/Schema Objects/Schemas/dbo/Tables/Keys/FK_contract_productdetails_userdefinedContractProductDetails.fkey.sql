ALTER TABLE [dbo].[userdefinedContractProductDetails]
    ADD CONSTRAINT [FK_contract_productdetails_userdefinedContractProductDetails] FOREIGN KEY ([contractproductid]) REFERENCES [dbo].[contract_productdetails] ([contractProductId]) ON DELETE CASCADE ON UPDATE NO ACTION;

