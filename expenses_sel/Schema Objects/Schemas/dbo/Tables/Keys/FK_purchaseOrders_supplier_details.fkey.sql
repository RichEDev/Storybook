ALTER TABLE [dbo].[purchaseOrders]
    ADD CONSTRAINT [FK_purchaseOrders_supplier_details] FOREIGN KEY ([supplierID]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE CASCADE ON UPDATE CASCADE;

