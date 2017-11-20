ALTER TABLE [dbo].[invoices]
    ADD CONSTRAINT [FK_invoices_supplier_details] FOREIGN KEY ([supplierID]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

