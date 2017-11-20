ALTER TABLE [dbo].[supplierNotes]
    ADD CONSTRAINT [FK_suppliernotes_supplier_details] FOREIGN KEY ([supplierid]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE CASCADE ON UPDATE NO ACTION;

