ALTER TABLE [dbo].[userdefinedSupplierDetails]
    ADD CONSTRAINT [FK_supplier_details_userdefinedSupplierDetails] FOREIGN KEY ([supplierid]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE CASCADE ON UPDATE NO ACTION;

