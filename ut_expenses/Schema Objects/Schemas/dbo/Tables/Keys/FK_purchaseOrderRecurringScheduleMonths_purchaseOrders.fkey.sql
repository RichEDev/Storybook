ALTER TABLE [dbo].[purchaseOrderRecurringScheduleMonths]
    ADD CONSTRAINT [FK_purchaseOrderRecurringScheduleMonths_purchaseOrders] FOREIGN KEY ([purchaseOrderId]) REFERENCES [dbo].[purchaseOrders] ([purchaseOrderID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

