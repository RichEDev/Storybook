ALTER TABLE [dbo].[product_suppliers]
    ADD CONSTRAINT [FK_product_suppliers_supplier_details] FOREIGN KEY ([supplierId]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE CASCADE ON UPDATE NO ACTION;

