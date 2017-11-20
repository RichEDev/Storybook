ALTER TABLE [dbo].[contract_productinformation]
    ADD CONSTRAINT [FK_contract_productinformation_contract_productdetails] FOREIGN KEY ([contractProductId]) REFERENCES [dbo].[contract_productdetails] ([contractProductId]) ON DELETE CASCADE ON UPDATE NO ACTION;

