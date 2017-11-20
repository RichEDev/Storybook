ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [FK_supplier_addr_supplier_details] FOREIGN KEY ([primary_addressid]) REFERENCES [dbo].[supplier_addresses] ([addressid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

