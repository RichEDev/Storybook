ALTER TABLE [dbo].[purchaseOrderDeliveries]
    ADD CONSTRAINT [FK_purchaseOrderDeliveries_purchaseOrders] FOREIGN KEY ([purchaseOrderID]) REFERENCES [dbo].[purchaseOrders] ([purchaseOrderID]) ON DELETE CASCADE ON UPDATE CASCADE;

