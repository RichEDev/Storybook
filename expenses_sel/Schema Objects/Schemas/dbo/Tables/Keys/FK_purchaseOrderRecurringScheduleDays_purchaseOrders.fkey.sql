ALTER TABLE [dbo].[purchaseOrderRecurringScheduleDays]
    ADD CONSTRAINT [FK_purchaseOrderRecurringScheduleDays_purchaseOrders] FOREIGN KEY ([purchaseOrderId]) REFERENCES [dbo].[purchaseOrders] ([purchaseOrderID]) ON DELETE CASCADE ON UPDATE NO ACTION;

