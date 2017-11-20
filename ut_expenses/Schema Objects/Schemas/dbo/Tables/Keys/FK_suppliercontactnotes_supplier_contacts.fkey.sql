ALTER TABLE [dbo].[supplierContactNotes]
    ADD CONSTRAINT [FK_suppliercontactnotes_supplier_contacts] FOREIGN KEY ([contactid]) REFERENCES [dbo].[supplier_contacts] ([contactid]) ON DELETE CASCADE ON UPDATE NO ACTION;

