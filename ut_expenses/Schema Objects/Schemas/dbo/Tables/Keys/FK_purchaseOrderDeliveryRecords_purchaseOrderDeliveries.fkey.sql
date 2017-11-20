ALTER TABLE [dbo].[purchaseOrderDeliveryRecords]
    ADD CONSTRAINT [FK_purchaseOrderDeliveryRecords_purchaseOrderDeliveries] FOREIGN KEY ([deliveryID]) REFERENCES [dbo].[purchaseOrderDeliveries] ([deliveryID]) ON DELETE CASCADE ON UPDATE CASCADE;

