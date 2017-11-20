ALTER TABLE [dbo].[invoices]
    ADD CONSTRAINT [FK_invoices_purchaseOrders] FOREIGN KEY ([purchaseOrderID]) REFERENCES [dbo].[purchaseOrders] ([purchaseOrderID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

