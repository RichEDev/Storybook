ALTER TABLE [dbo].[supplier_contacts]
    ADD CONSTRAINT [FK_supplier_addr_supplier_contacts_home] FOREIGN KEY ([home_addressid]) REFERENCES [dbo].[supplier_addresses] ([addressid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

