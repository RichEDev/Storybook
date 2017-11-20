ALTER TABLE [dbo].[purchaseOrderDeliveryRecords]
    ADD CONSTRAINT [FK_purchaseOrderDeliveryRecords_purchaseOrderProducts] FOREIGN KEY ([purchaseOrderProductID]) REFERENCES [dbo].[purchaseOrderProducts] ([purchaseOrderProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

