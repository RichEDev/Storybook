ALTER TABLE [dbo].[purchaseOrderProducts]
    ADD CONSTRAINT [FK_purchaseOrderProducts_purchaseOrders] FOREIGN KEY ([purchaseOrderID]) REFERENCES [dbo].[purchaseOrders] ([purchaseOrderID]) ON DELETE CASCADE ON UPDATE CASCADE;

