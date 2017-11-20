ALTER TABLE [dbo].[supplier_addresses]
    ADD CONSTRAINT [FK_supplier_addresses_supplier_details] FOREIGN KEY ([supplierid]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

