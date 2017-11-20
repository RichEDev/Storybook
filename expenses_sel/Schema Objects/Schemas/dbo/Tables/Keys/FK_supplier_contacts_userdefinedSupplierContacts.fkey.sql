ALTER TABLE [dbo].[userdefinedSupplierContacts]
    ADD CONSTRAINT [FK_supplier_contacts_userdefinedSupplierContacts] FOREIGN KEY ([contactid]) REFERENCES [dbo].[supplier_contacts] ([contactid]) ON DELETE CASCADE ON UPDATE NO ACTION;

