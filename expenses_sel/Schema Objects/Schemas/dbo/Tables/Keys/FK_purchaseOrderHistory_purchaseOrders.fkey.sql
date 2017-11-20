ALTER TABLE [dbo].[purchaseOrderHistory]
    ADD CONSTRAINT [FK_purchaseOrderHistory_purchaseOrders] FOREIGN KEY ([purchaseOrderID]) REFERENCES [dbo].[purchaseOrders] ([purchaseOrderID]) ON DELETE CASCADE ON UPDATE NO ACTION;

